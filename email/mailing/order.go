package mailing

import (
	"email/messaging"
	"time"
)

type OrderCreationBody struct {
	OrderNumber string
	CreatedAt   time.Time
}

func (m *Mailer) SendOrderCreated(data []byte) error {
	msg, err := messaging.UnwrapMessage[messaging.OrderCreatedEvent](data)
	if err != nil {
		return err
	}

	msgBody := OrderCreationBody{msg.OrderNumber, msg.CreatedAt}

	mailMsg := MailMessage[OrderCreationBody]{
		recipient: msg.Email,
		subject:   "Order creation",
		body:      msgBody,
		template:  "order/creation",
	}

	return mailMsg.Send(m)
}
