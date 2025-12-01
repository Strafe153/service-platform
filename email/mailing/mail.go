package mailing

import (
	"bytes"
	"fmt"
	"os"
	"strconv"
	"text/template"

	gomail "github.com/go-gomail/gomail"
)

const (
	mailpitPort     = "MAILPIT_PORT"
	mailpitHost     = "MAILPIT_HOST"
	mailpitSender   = "MAILPIT_SENDER"
	mailpitUser     = "MAILPIT_USER"
	mailpitPassword = "MAILPIT_PASSWORD"
)

type mailConfig struct {
	host     string
	port     int
	sender   string
	user     string
	password string
}

func readConfig() (*mailConfig, error) {
	port, err := strconv.Atoi(os.Getenv(mailpitPort))

	if err != nil {
		return nil, err
	}

	host := os.Getenv(mailpitHost)
	sender := os.Getenv(mailpitSender)
	user := os.Getenv(mailpitUser)
	password := os.Getenv(mailpitPassword)

	config := &mailConfig{
		host,
		port,
		sender,
		user,
		password,
	}

	return config, nil
}

type MailMessage[T any] struct {
	Recipient string
	Subject   string
	Body      T
	Template  string
}

func getEmailBody[T any](msg *MailMessage[T]) (*bytes.Buffer, error) {
	tmpPath := fmt.Sprintf("./mailing/templates/%s.html", msg.Template)
	tmp, err := template.ParseFiles(tmpPath)

	if err != nil {
		return nil, err
	}

	var msgBody bytes.Buffer

	if err := tmp.Execute(&msgBody, msg.Body); err != nil {
		return nil, err
	}

	return &msgBody, nil
}

func (m *MailMessage[T]) Send() error {
	cfg, err := readConfig()

	if err != nil {
		return err
	}

	body, err := getEmailBody(m)

	if err != nil {
		return err
	}

	msg := gomail.NewMessage()

	msg.SetHeader("From", cfg.sender)
	msg.SetHeader("To", m.Recipient)
	msg.SetHeader("Subject", m.Subject)

	msg.SetBody("text/html", body.String())

	dialer := gomail.NewDialer(cfg.host, cfg.port, cfg.user, cfg.password)

	return dialer.DialAndSend(msg)
}
