package api

import (
	"encoding/json"
	"net/http"
)

type problemDetails struct {
	Type     string `json:"type"`
	Title    string `json:"title"`
	Status   int    `json:"status"`
	Detail   string `json:"detail"`
	Instance string `json:"instance"`
}

func writeProblem(w http.ResponseWriter, r *http.Request, status int, err error) {
	w.Header().Set("Content-Type", "application/problem+json")
	w.WriteHeader(status)

	rfcType := getType(status)
	title := getTitle(status)

	details := problemDetails{
		Type:     rfcType,
		Title:    title,
		Status:   status,
		Detail:   err.Error(),
		Instance: r.URL.Path,
	}

	json.NewEncoder(w).Encode(details)
}

func getTitle(status int) string {
	switch status {
	case http.StatusBadRequest:
		return "Bad Request"
	case http.StatusUnauthorized:
		return "Unauthorized"
	case http.StatusForbidden:
		return "Forbidden"
	case http.StatusNotFound:
		return "Not Found"
	}

	return "Internal Server Error"
}

func getType(status int) string {
	switch status {
	case http.StatusBadRequest:
		return "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1"
	case http.StatusUnauthorized:
		return "https://www.rfc-editor.org/rfc/rfc7235#section-3.1"
	case http.StatusForbidden:
		return "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.3"
	case http.StatusNotFound:
		return "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.4"
	}

	return "https://www.rfc-editor.org/rfc/rfc7231#section-6.6.1"
}
