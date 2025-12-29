package domain

const (
	ErrBadRequest   = "BAD_REQUEST"
	ErrUnauthorized = "UNAUTHORIZED"
	ErrForbidden    = "FORBIDDEN"
	ErrNotFound     = "NOT_FOUND"
)

type AppError struct {
	Type    string
	message string
}

func NewAppError(errType string, message string) *AppError {
	return &AppError{errType, message}
}

func (e AppError) Error() string {
	return e.message
}
