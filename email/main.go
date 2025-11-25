package main

import (
	"log"

	"github.com/google/uuid"
	"github.com/joho/godotenv"
)

type MessageWrapper[T any] struct {
	MessageId     uuid.UUID
	CorrelationId uuid.UUID
	Message       T
}

type UserCreatedEvent struct {
	Email     string
	CreatedAt string
}

func main() {
	err := godotenv.Load()

	if err != nil {
		failOnError(err, "Failed to load .env file")
	}

	delivery := connectRabbitMq()

	consume(delivery)
}

func failOnError(err error, message string) {
	if err != nil {
		log.Panicf("%s: %s", message, err)
	}
}
