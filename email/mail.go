package main

import (
	"fmt"
	"os"
	"strconv"

	gomail "github.com/go-gomail/gomail"
)

type MailConfiguration struct {
	host     string
	port     int
	sender   string
	user     string
	password string
}

func readConfiguration() (*MailConfiguration, error) {
	port, err := strconv.Atoi(os.Getenv("MAILPIT_PORT"))

	if err != nil {
		return nil, err
	}

	host := os.Getenv("MAILPIT_HOST")
	sender := os.Getenv("MAILPIT_SENDER")
	user := os.Getenv("MAILPIT_USER")
	password := os.Getenv("MAILPIT_PASSWORD")

	config := &MailConfiguration{
		host,
		port,
		sender,
		user,
		password,
	}

	return config, nil
}

func sendEmail(emailMessage UserCreatedEvent) {
	mailConfig, err := readConfiguration()

	if err != nil {
		fmt.Print(err)
	}

	mailMsg := gomail.NewMessage()

	mailMsg.SetHeader("From", mailConfig.sender)
	mailMsg.SetHeader("To", emailMessage.Email)
	mailMsg.SetHeader("Subject", "Account registration")

	msg := fmt.Sprintf("Your account has been registered at %s", emailMessage.CreatedAt)

	mailMsg.SetBody("text/plain", msg)

	dialer := gomail.NewDialer(
		mailConfig.host,
		mailConfig.port,
		mailConfig.user,
		mailConfig.password)

	if err := dialer.DialAndSend(mailMsg); err != nil {
		fmt.Print("message sent successfully")
	}
}
