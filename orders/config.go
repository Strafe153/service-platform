package main

import (
	"encoding/json"
	inf "orders/infrastructure"
	"os"
)

type Config struct {
	Database inf.DatabaseConfig `json:"database"`
	RabbitMQ inf.RabbitMQConfig `json:"rabbit_mq"`
	Keycloak inf.KeycloakConfig `json:"keycloak"`
}

func getConfig() (*Config, error) {
	cfgFile, err := os.ReadFile("config.json")
	if err != nil {
		return nil, err
	}

	var cfg Config
	if err := json.Unmarshal(cfgFile, &cfg); err != nil {
		return nil, err
	}

	return &cfg, nil
}
