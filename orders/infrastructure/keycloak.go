package infrastructure

const (
	AdminRole   = "admin"
	UserIdClaim = "userId"
	EmailClaim  = "email"
	IsAdmin     = "isAdmin"
)

type KeycloakConfig struct {
	Url   string `json:"url"`
	Realm string `json:"realm"`
}

type ResourceAccessUser struct {
	Roles []string `json:"roles"`
}

type ResourceAccess struct {
	User ResourceAccessUser `json:"user"`
}

type KeycloakClaims struct {
	Email          string         `json:"email"`
	UserId         string         `json:"user_id"`
	ResourceAccess ResourceAccess `json:"resource_access"`
}
