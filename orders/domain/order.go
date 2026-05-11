package domain

import (
	"context"
	"time"

	"go.mongodb.org/mongo-driver/v2/bson"
)

type OrderStatus int

const (
	ActiveOrder    OrderStatus = 1
	CompletedOrder OrderStatus = 2
	CancelledOrder OrderStatus = 3
)

type Order struct {
	Id       bson.ObjectID `bson:"_id,omitempty"`
	UserId   string        `bson:"userId"`
	Products []Product     `bson:"product"`
	Status   OrderStatus   `bson:"status"`
}

type OrderProduct struct {
	Id    string `json:"id" validate:"required,alphanum,min=24,max=24"`
	Count int    `json:"count" validate:"required,min=1,max=9999"`
}

type OrdersRepository interface {
	GetAll(page Page, c context.Context) ([]Order, error)
	GetByUserId(id string, page Page, c context.Context) ([]Order, error)
	Get(id bson.ObjectID, c context.Context) (*Order, error)
	Create(order *Order, c context.Context) (string, error)
	Cancel(id bson.ObjectID, c context.Context) error
	Complete(id bson.ObjectID, c context.Context) error
}

type OrderCreatedEvent struct {
	Email       string    `json:"email"`
	OrderNumber string    `json:"orderNumber"`
	CreatedAt   time.Time `json:"createdAt"`
}

type OrderCompletedEvent struct {
	OrderId     string    `json:"orderId"`
	CompletedAt time.Time `json:"completedAt"`
}
