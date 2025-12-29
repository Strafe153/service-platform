package api

import (
	"encoding/json"
	"fmt"
	"net/http"
	"orders/domain"
	inf "orders/infrastructure"
)

func RegisterOrderEndpoints(mux *http.ServeMux, h *Handler, cfg *inf.KeycloakConfig) {
	adminOrSameUserPolicy := &AdminOrSameUserPolicy{}

	mux.Handle("GET /orders", AuthMiddleware(&AdminOnlyPolicy{}, cfg, h.getOrders))
	mux.Handle("GET /orders/{id}", AuthMiddleware(adminOrSameUserPolicy, cfg, h.getOrder))
	mux.Handle("GET /orders/user/{id}", AuthMiddleware(adminOrSameUserPolicy, cfg, h.getOrdersByUserId))
	mux.Handle("POST /orders", AuthMiddleware(adminOrSameUserPolicy, cfg, h.createOrder))
	mux.Handle("POST /orders/{id}/cancel", AuthMiddleware(adminOrSameUserPolicy, cfg, h.cancelOrder))
}

func (h *Handler) getOrders(w http.ResponseWriter, r *http.Request) {
	orders, err := h.ordersService.Get(r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	json.NewEncoder(w).Encode(orders)
}

func (h *Handler) getOrder(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	order, err := h.ordersService.GetById(id, r.Context())
	if err != nil {
		code := translateAppError(err)
		writeProblem(w, r, code, err)

		return
	}

	json.NewEncoder(w).Encode(order)
}

func (h *Handler) getOrdersByUserId(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	orders, err := h.ordersService.GetByUserId(id, r.Context())
	if err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
	}

	json.NewEncoder(w).Encode(orders)
}

func (h *Handler) createOrder(w http.ResponseWriter, r *http.Request) {
	var request domain.CreateOrderRequest
	if err := json.NewDecoder(r.Body).Decode(&request); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	response, err := h.ordersService.Create(&request, r.Context())
	if err != nil {
		code := translateAppError(err)
		writeProblem(w, r, code, err)

		return
	}

	w.Header().Set("Location", fmt.Sprintf("/orders/%s", response.Id))
	w.WriteHeader(http.StatusCreated)

	json.NewEncoder(w).Encode(&response)
}

func (h *Handler) cancelOrder(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("id")

	if err := h.ordersService.Cancel(id, r.Context()); err != nil {
		writeProblem(w, r, http.StatusBadRequest, err)
		return
	}

	w.WriteHeader(http.StatusNoContent)
}
