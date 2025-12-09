package mailing

import (
	"email/messaging"
	"encoding/json"
)

type AccountRegistrationBody struct {
	TimeStamp string
}

func (m *Mailer) SendAccountRegistration(data []byte) error {
	var wrapper messaging.MessageWrapper[messaging.UserCreatedEvent]
	if err := json.Unmarshal(data, &wrapper); err != nil {
		return err
	}

	msgBody := AccountRegistrationBody{TimeStamp: wrapper.Message.CreatedAt}

	msg := MailMessage[AccountRegistrationBody]{
		recipient: wrapper.Message.Email,
		subject:   "Account registration",
		body:      msgBody,
		template:  "registration",
	}

	return msg.Send(m)
}

type AccountRemovalBody struct {
	TimeStamp string
}

func (m *Mailer) SendAccountRemoval(data []byte) error {
	var wrapper messaging.MessageWrapper[messaging.UserDeletedEvent]
	if err := json.Unmarshal(data, &wrapper); err != nil {
		return err
	}

	msgBody := AccountRemovalBody{TimeStamp: wrapper.Message.DeletedAt}

	msg := MailMessage[AccountRemovalBody]{
		recipient: wrapper.Message.Email,
		subject:   "Account removal",
		body:      msgBody,
		template:  "removal",
	}

	return msg.Send(m)
}
