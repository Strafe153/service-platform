package api

import (
	"encoding/json"
	"fmt"
	"net/http"
	"orders/domain"
	inf "orders/infrastructure"
)

func RegisterProductEndpoints(mux *http.ServeMux, h *Handler, cfg *inf.KeycloakConfig) {
	adminOnlyPolicy := &AdminOnlyPolicy{}

	mux.Handle("GET /products", AuthMiddleware(adminOnlyPolicy, cfg, h.getProducts))
	mux.Handle("GET /products/{id}", AuthMiddleware(adminOnlyPolicy, cfg, h.getProduct))
	mux.Handle("POST /products", AuthMiddleware(adminOnlyPolicy, cfg, h.createProduct))
	mux.Handle("PUT /products/{id}", AuthMiddleware(adminOnlyPolicy, cfg, h.updateProduct))
	mux.Handle("POST /products/{id}/discontinue", AuthMiddleware(adminOnlyPolicy, cfg, h.discontinueProduct))
}

func (h *Handler) getProducts(w http.ResponseWriter, r *http.Request) {
	products, err := h.productsService.Get(r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	json.NewEncoder(w).Encode(products)
}

func (h *Handler) getProduct(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	product, err := h.productsService.GetById(id, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusNotFound, err)
		return
	}

	json.NewEncoder(w).Encode(product)
}

func (h *Handler) createProduct(w http.ResponseWriter, r *http.Request) {
	var request domain.CreateProductRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	response, err := h.productsService.Create(&request, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.Header().Set("Location", fmt.Sprintf("/products/%s", response.Id))
	w.WriteHeader(http.StatusCreated)

	json.NewEncoder(w).Encode(response)
}

func (h *Handler) updateProduct(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	var request domain.UpdateProductRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	if err := h.productsService.Update(id, &request, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}

func (h *Handler) discontinueProduct(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	if err := h.productsService.Discontinue(id, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}
