package mailing

import (
	"email/messaging"
	"encoding/json"
)

type AccountRegistrationBody struct {
	TimeStamp string
}

func SendAccountRegistration(data []byte) error {
	var wrapper messaging.MessageWrapper[messaging.UserCreatedEvent]

	if err := json.Unmarshal(data, &wrapper); err != nil {
		return err
	}

	msgBody := AccountRegistrationBody{TimeStamp: wrapper.Message.CreatedAt}

	msg := MailMessage[AccountRegistrationBody]{
		Recipient: wrapper.Message.Email,
		Subject:   "Account registration",
		Body:      msgBody,
		Template:  "registration",
	}

	return msg.Send()
}
