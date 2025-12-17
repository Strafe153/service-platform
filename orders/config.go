package main

import (
	"encoding/json"
	"orders/infra"
	"os"
)

type Config struct {
	Database infra.DatabaseConfig `json:"database"`
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
