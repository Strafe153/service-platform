package domain

import (
	"context"
	"time"

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
	Id    string `json:"id" validate:"required,alphanum,min=24,max=24"`
	Count int    `json:"count" validate:"required,min=1,max=9999"`
}

type CreateOrderRequest struct {
	UserId   string         `json:"userId" validate:"required,alphanum,min=26,max=26"`
	Products []OrderProduct `json:"products" validate:"required,min=1,max=999,dive"`
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

type OrdersRepository interface {
	GetAll(page Page, c context.Context) ([]Order, error)
	GetByUserId(id string, page Page, c context.Context) ([]Order, error)
	Get(id bson.ObjectID, c context.Context) (*Order, error)
	Create(order *Order, c context.Context) (string, error)
	Cancel(id bson.ObjectID, c context.Context) error
}

type OrderCreatedEvent struct {
	Email       string    `json:"email"`
	OrderNumber string    `json:"orderNumber"`
	CreatedAt   time.Time `json:"createdAt"`
}
