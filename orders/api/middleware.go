package api

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	inf "orders/infrastructure"
	"slices"
	"strings"

	"github.com/Nerzal/gocloak/v13"
	"github.com/golang-jwt/jwt/v5"
)

type AuthorizationPolicy interface {
	setClaims(*inf.KeycloakClaims)
	verify(*http.Request) error
}

type AdminOnlyPolicy struct {
	claims *inf.KeycloakClaims
}

func (p *AdminOnlyPolicy) setClaims(claims *inf.KeycloakClaims) {
	p.claims = claims
}

func (p *AdminOnlyPolicy) verify(r *http.Request) error {
	if !isAdmin(&p.claims.ResourceAccess.User) {
		return fmt.Errorf("User is not an admin")
	}

	return nil
}

type AdminOrSameUserPolicy struct {
	claims *inf.KeycloakClaims
}

func (p *AdminOrSameUserPolicy) setClaims(claims *inf.KeycloakClaims) {
	p.claims = claims
}

func (p *AdminOrSameUserPolicy) verify(r *http.Request) error {
	isAdmin := isAdmin(&p.claims.ResourceAccess.User)

	if isAdmin {
		return nil
	}

	id := r.PathValue("id")
	if id != "" && id != p.claims.UserId {
		return fmt.Errorf("Interaction with the user is not allowed")
	}

	return nil
}

func isAdmin(user *inf.ResourceAccessUser) bool {
	return slices.Contains(user.Roles, inf.AdminRole)
}

func convertClaims(c *jwt.MapClaims) (*inf.KeycloakClaims, int, error) {
	claimsBytes, err := json.Marshal(c)
	if err != nil {
		return nil, http.StatusUnauthorized, err
	}

	var claims inf.KeycloakClaims
	if err := json.Unmarshal(claimsBytes, &claims); err != nil {
		return nil, http.StatusUnauthorized, fmt.Errorf("Failed to parse claims")
	}

	return &claims, http.StatusOK, nil
}

func readClaims(cfg *inf.KeycloakConfig, r *http.Request) (*inf.KeycloakClaims, int, error) {
	bearer := r.Header.Get("Authorization")
	if bearer == "" {
		return nil, http.StatusUnauthorized, fmt.Errorf("No \"Authorization\" header provided")
	}

	splitBearer := strings.Split(bearer, " ")
	if len(splitBearer) < 2 {
		return nil, http.StatusUnauthorized, fmt.Errorf("Invalid token")
	}

	client := gocloak.NewClient(cfg.Url)

	_, claimsMap, err := client.DecodeAccessToken(r.Context(), splitBearer[1], cfg.Realm)
	if err != nil {
		return nil, http.StatusUnauthorized, err
	}

	return convertClaims(claimsMap)
}

func populateRequestWithClaims(r *http.Request, claims *inf.KeycloakClaims) *http.Request {
	c := context.WithValue(r.Context(), inf.EmailClaim, claims.Email)
	c = context.WithValue(c, inf.UserIdClaim, claims.UserId)
	c = context.WithValue(c, inf.IsAdmin, isAdmin(&claims.ResourceAccess.User))

	return r.WithContext(c)
}

func AuthMiddleware(
	policy AuthorizationPolicy,
	cfg *inf.KeycloakConfig,
	next http.HandlerFunc,
) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		if policy != nil {
			claims, code, err := readClaims(cfg, r)
			if err != nil {
				writeProblem(w, r, code, err)
				return
			}

			r = populateRequestWithClaims(r, claims)
			policy.setClaims(claims)

			if err := policy.verify(r); err != nil {
				writeProblem(w, r, http.StatusForbidden, err)
				return
			}
		}

		next.ServeHTTP(w, r)
	}
}
