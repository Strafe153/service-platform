package main

import (
	"log"
	"os"
	"os/signal"
	"syscall"

	"email/mailing"
	"email/messaging"

	"github.com/joho/godotenv"
)

const messageQueue = "MESSAGE_QUEUE"

func main() {
	if err := godotenv.Load(); err != nil {
		failOnError(err, "Failed to load .env file")
	}

	queue := os.Getenv(messageQueue)
	provider := messaging.NewProvider(queue)

	if err := provider.Connect(); err != nil {
		failOnError(err, "Failed to connect")
	}

	if err := provider.Consume(mailing.SendAccountRegistration); err != nil {
		failOnError(err, "Failed to consume")
	}

	waitForShutdown()

	if err := provider.Close(); err != nil {
		log.Println(err)
	}
}

func waitForShutdown() {
	sigs := make(chan os.Signal, 1)

	signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)

	<-sigs
}

func failOnError(err error, message string) {
	if err != nil {
		log.Panicf("%s: %s", message, err)
	}
}
