package messaging

import "time"

type UserCreatedEvent struct {
	Email     string    `json:"email"`
	CreatedAt time.Time `json:"createdAt"`
}

type UserDeletedEvent struct {
	Email     string    `json:"email"`
	DeletedAt time.Time `json:"deletedAt"`
}
