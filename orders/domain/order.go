package domain

import (
	"go.mongodb.org/mongo-driver/v2/bson"
)

type orderStatus int

const (
	ActiveOrder    orderStatus = 1
	CompletedOrder orderStatus = 2
	CancelledOrder orderStatus = 3
)

type Order struct {
	Id       bson.ObjectID `bson:"_id,omitempty"`
	UserId   string        `bson:"userId"`
	Products []Product     `bson:"product"`
	Status   orderStatus   `bson:"status"`
}

func (r *Order) ToResponse() OrderResponse {
	products := make([]ProductResponse, len(r.Products))
	for i, p := range r.Products {
		products[i] = p.ToResponse()
	}

	return OrderResponse{
		UserId:   r.UserId,
		Products: products,
		Status:   r.Status,
	}
}

type OrderProduct struct {
	Id    string
	Count int
}

type CreateOrderRequest struct {
	UserId   string         `json:"userId"`
	Products []OrderProduct `json:"products"`
}

func (r *CreateOrderRequest) ToOrder() Order {
	return Order{UserId: r.UserId}
}

type OrderResponse struct {
	Id       string            `json:"id"`
	UserId   string            `json:"userId"`
	Products []ProductResponse `json:"products"`
	Status   orderStatus       `json:"status"`
}
