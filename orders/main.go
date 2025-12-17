package main

import (
	"log"
	"net/http"
	"orders/api"
)

func main() {
	cfg, err := getConfig()
	if err != nil {
		log.Fatal(err)
	}

	handler := api.GetHandler(&cfg.Database)

	mux := http.NewServeMux()
	api.RegisterOrderEndpoints(mux, handler)
	api.RegisterProductEndpoints(mux, handler)

	http.ListenAndServe(":3333", mux)
}
