package api

import (
	"log"
	"orders/infra"
)

type Handler struct {
	ordersRepository   *infra.OrdersRepository
	productsRepository *infra.ProductsRepository
}

func GetHandler(config *infra.DatabaseConfig) *Handler {
	ctx, err := infra.GetContext(config)
	if err != nil {
		log.Fatal(err)
	}

	ordersRepo := infra.NewOrdersRepository(ctx)
	productsRepo := infra.NewProductsRepository(ctx)

	return &Handler{ordersRepo, productsRepo}
}
