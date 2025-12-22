package mailing

import (
	"email/messaging"
	"time"
)

type AccountRegistrationBody struct {
	RegisteredAt time.Time
}

func (m *Mailer) SendAccountRegistration(data []byte) error {
	msg, err := messaging.UnwrapMessage[messaging.UserCreatedEvent](data)
	if err != nil {
		return err
	}

	msgBody := AccountRegistrationBody{msg.CreatedAt}

	mailMsg := MailMessage[AccountRegistrationBody]{
		recipient: msg.Email,
		subject:   "Account registration",
		body:      msgBody,
		template:  "user/registration",
	}

	return mailMsg.Send(m)
}

type AccountRemovalBody struct {
	DeletedAt time.Time
}

func (m *Mailer) SendAccountRemoval(data []byte) error {
	msg, err := messaging.UnwrapMessage[messaging.UserDeletedEvent](data)
	if err != nil {
		return err
	}

	msgBody := AccountRemovalBody{msg.DeletedAt}

	mailMsg := MailMessage[AccountRemovalBody]{
		recipient: msg.Email,
		subject:   "Account removal",
		body:      msgBody,
		template:  "user/removal",
	}

	return mailMsg.Send(m)
}
