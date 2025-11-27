package messaging

import "github.com/google/uuid"

type ConsumeHandler = func([]byte) error

type MessageProvider interface {
	Connect() error
	Consume(handler ConsumeHandler) error
	Close() error
}

type MessageWrapper[T any] struct {
	MessageId     uuid.UUID
	CorrelationId uuid.UUID
	Message       T
}
