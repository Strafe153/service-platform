package domain

import "github.com/google/uuid"

type MessageProvider interface {
	Connect() error
	Publish(key string, msg []byte) error
	Close() error
}

type MessageWrapper[T any] struct {
	CorrelationId uuid.UUID
	Message       T
}

func WrapMessage[T any](message T) (*MessageWrapper[T], error) {
	id, err := uuid.NewUUID()
	if err != nil {
		return nil, err
	}

	wrapper := &MessageWrapper[T]{id, message}

	return wrapper, nil
}
