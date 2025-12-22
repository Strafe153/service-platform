package messaging

import "time"

type OrderCreatedEvent struct {
	Email       string    `json:"email"`
	OrderNumber string    `json:"orderNumber"`
	CreatedAt   time.Time `json:"createdAt"`
}
