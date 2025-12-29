package api

import (
	"net/http"
	"orders/domain"
)

func translateAppError(err *domain.AppError) int {
	switch err.Type {
	case domain.ErrBadRequest:
		return http.StatusBadRequest
	case domain.ErrUnauthorized:
		return http.StatusUnauthorized
	case domain.ErrForbidden:
		return http.StatusForbidden
	case domain.ErrNotFound:
		return http.StatusNotFound
	}

	return http.StatusInternalServerError
}
