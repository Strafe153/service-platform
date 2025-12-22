package main

import (
	"log"
	"net/http"
	"orders/api"
	"orders/messaging"
)

func main() {
	cfg, err := getConfig()
	failOnError(err, "failed to read the config file")

	provider := messaging.NewRabbitMQProvider(&cfg.RabbitMQ)

	handler, err := api.GetHandler(&cfg.Database, provider)
	failOnError(err, "failed to create the handler")

	mux := http.NewServeMux()
	api.RegisterOrderEndpoints(mux, handler)
	api.RegisterProductEndpoints(mux, handler)

	http.ListenAndServe(":3333", mux)
}

func failOnError(err error, message string) {
	if err != nil {
		log.Panicf("%s: %s", message, err)
	}
}
