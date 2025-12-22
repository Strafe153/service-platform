package main

import (
	"email/mailing"
	"email/messaging"
	"encoding/json"
	"os"
)

type Config struct {
	RabbitMq messaging.RabbitMQConfig `json:"rabbit_mq"`
	Mailpit  mailing.MailConfig       `json:"mailpit"`
}

func getConfig() *Config {
	cfgFile, err := os.ReadFile("config.json")
	failOnError(err, "failed to read the config file")

	var cfg Config
	err = json.Unmarshal(cfgFile, &cfg)

	failOnError(err, "failed to parse the config file")

	return &cfg
}
