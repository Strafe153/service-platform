package main

import (
	inf "orders/infrastructure"
	"os"

	"gopkg.in/yaml.v3"
)

type Config struct {
	Database inf.DatabaseConfig `yaml:"database"`
	RabbitMQ inf.RabbitMQConfig `yaml:"rabbit_mq"`
	Keycloak inf.KeycloakConfig `yaml:"keycloak"`
}

func getConfig() (*Config, error) {
	cfgFile, err := os.ReadFile("config.yml")
	if err != nil {
		return nil, err
	}

	var cfg Config
	if err := yaml.Unmarshal(cfgFile, &cfg); err != nil {
		return nil, err
	}

	return &cfg, nil
}
