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
	if err != nil {
		failOnError(err, "failed to read config file")
	}

	var cfg Config
	if err := json.Unmarshal(cfgFile, &cfg); err != nil {
		failOnError(err, "failed to parsed the config file")
	}

	return &cfg
}
