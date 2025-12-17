package api

import (
	"encoding/json"
	"fmt"
	"net/http"
	"orders/domain"

	"go.mongodb.org/mongo-driver/v2/bson"
)

func RegisterProductEndpoints(mux *http.ServeMux, h *Handler) {
	mux.HandleFunc("GET /products", h.getProducts)
	mux.HandleFunc("GET /products/{id}", h.getProduct)
	mux.HandleFunc("POST /products", h.createProduct)
	mux.HandleFunc("PUT /products/{id}", h.updateProduct)
	mux.HandleFunc("POST /products/{id}/discontinue", h.discontinueProduct)
}

func (h *Handler) getProducts(w http.ResponseWriter, r *http.Request) {
	products, err := h.productsRepository.GetAll(r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	responses := make([]domain.ProductResponse, len(products))
	for i, p := range products {
		responses[i] = p.ToResponse()
	}

	json.NewEncoder(w).Encode(&responses)
}

func (h *Handler) getProduct(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	product, err := h.productsRepository.Get(oId, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusNotFound, err)
		return
	}

	response := product.ToResponse()

	json.NewEncoder(w).Encode(&response)
}

func (h *Handler) createProduct(w http.ResponseWriter, r *http.Request) {
	var request domain.CreateProductRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	product := request.ToProduct()
	product.IsAvailable = true

	id, err := h.productsRepository.Create(&product, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	response := product.ToResponse()
	response.Id = id

	w.Header().Set("Location", fmt.Sprintf("/products/%s", id))
	w.WriteHeader(http.StatusCreated)

	json.NewEncoder(w).Encode(&response)
}

func (h *Handler) updateProduct(w http.ResponseWriter, r *http.Request) {
	var request domain.UpdateProductRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	id := r.PathValue("id")
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	_, err = h.productsRepository.Get(oId, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusNotFound, err)
		return
	}

	product := request.ToProduct()
	product.IsAvailable = true

	if err := h.productsRepository.Update(oId, &product, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}

func (h *Handler) discontinueProduct(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	if err := h.productsRepository.Discontinue(oId, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}
