package api

import (
	"encoding/json"
	"fmt"
	"net/http"
	"orders/domain"

	"go.mongodb.org/mongo-driver/v2/bson"
)

func RegisterOrderEndpoints(mux *http.ServeMux, h *Handler) {
	mux.HandleFunc("GET /orders", h.getOrders)
	mux.HandleFunc("GET /orders/{id}", h.getOrder)
	mux.HandleFunc("POST /orders", h.createOrder)
	mux.HandleFunc("POST /orders/{id}/cancel", h.cancelOrder)
}

func (h *Handler) getOrders(w http.ResponseWriter, r *http.Request) {
	orders, err := h.ordersRepository.GetAll(r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	json.NewEncoder(w).Encode(orders)
}

func (h *Handler) getOrder(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	order, err := h.ordersRepository.Get(oId, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusNotFound, err)
		return
	}

	json.NewEncoder(w).Encode(order)
}

func (h *Handler) createOrder(w http.ResponseWriter, r *http.Request) {
	var request domain.CreateOrderRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	products, err := getProducts(r, h, request)
	if err != nil {
		writeProblem(w, r, http.StatusNotFound, err)
		return
	}

	order := request.ToOrder()
	order.Products = products
	order.Status = domain.ActiveOrder

	id, err := h.ordersRepository.Create(&order, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	response := order.ToResponse()
	response.Id = id

	w.Header().Set("Location", fmt.Sprintf("/orders/%s", id))
	w.WriteHeader(http.StatusCreated)

	json.NewEncoder(w).Encode(&response)
}

func (h *Handler) cancelOrder(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	if err := h.ordersRepository.Cancel(oId, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}

func getProducts(
	r *http.Request,
	h *Handler,
	request domain.CreateOrderRequest,
) ([]domain.Product, error) {
	productIds := make([]bson.ObjectID, len(request.Products))
	for i, product := range request.Products {
		oId, err := bson.ObjectIDFromHex(product.Id)
		if err != nil {
			return nil, err
		}

		productIds[i] = oId
	}

	return h.productsRepository.GetByIds(productIds, r.Context())
}
