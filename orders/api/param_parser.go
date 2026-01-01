package api

import (
	"net/http"
	"net/url"
	"orders/domain"
	"strconv"
)

func readRouteId(r *http.Request) string {
	return r.PathValue("id")
}

func readIntQueryParam(q url.Values, key string, defaultValue int) int {
	param := q.Get(key)
	parsed, err := strconv.Atoi(param)

	if err != nil {
		parsed = defaultValue
	}

	return parsed
}

func readPageParams(q url.Values) domain.Page {
	pageNumber := readIntQueryParam(q, "pageNumber", 1)
	pageSize := readIntQueryParam(q, "pageSize", 20)

	page := domain.Page{Number: pageNumber, Size: pageSize}

	return page
}
