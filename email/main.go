package main

import (
	"log"
	"os"
	"os/signal"
	"syscall"

	"email/mailing"
	"email/messaging"
)

func main() {
	config := getConfig()
	mailer := mailing.NewMailer(&config.Mailpit)

	provider := messaging.NewRabbitMQProvider(&config.RabbitMq)

	provider.AttachHandler("user-created", mailer.SendAccountRegistration)
	provider.AttachHandler("user-deleted", mailer.SendAccountRemoval)

	provider.Connect()

	if err := provider.Consume(); err != nil {
		failOnError(err, "failed to consume")
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
