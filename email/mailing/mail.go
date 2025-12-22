package mailing

import (
	"bytes"
	"fmt"
	"text/template"

	gomail "github.com/go-gomail/gomail"
)

type MailConfig struct {
	Host     string
	Port     int
	Sender   string
	User     string
	Password string
}

type Mailer struct {
	config *MailConfig
}

func NewMailer(config *MailConfig) *Mailer {
	return &Mailer{config}
}

func (m *Mailer) toDialer() *gomail.Dialer {
	return gomail.NewDialer(m.config.Host, m.config.Port, m.config.User, m.config.Password)
}

type MailMessage[T any] struct {
	recipient string
	subject   string
	body      T
	template  string
}

func (m *MailMessage[T]) Send(mailer *Mailer) error {
	body, err := getEmailBody(m)
	if err != nil {
		return err
	}

	msg := gomail.NewMessage()

	msg.SetHeader("From", mailer.config.Sender)
	msg.SetHeader("To", m.recipient)
	msg.SetHeader("Subject", m.subject)

	msg.SetBody("text/html", body.String())

	d := mailer.toDialer()

	return d.DialAndSend(msg)
}

func getEmailBody[T any](msg *MailMessage[T]) (*bytes.Buffer, error) {
	tmpPath := fmt.Sprintf("./mailing/templates/%s.html", msg.template)

	tmp, err := template.ParseFiles(tmpPath)
	if err != nil {
		return nil, err
	}

	var msgBody bytes.Buffer
	if err := tmp.Execute(&msgBody, msg.body); err != nil {
		return nil, err
	}

	return &msgBody, nil
}
