package api

import (
	"orders/application"
	"orders/infra"
	"orders/messaging"
)

type Handler struct {
	productsService *application.ProductsService
	ordersService   *application.OrdersService
}

func GetHandler(dbConfig *infra.DatabaseConfig, msgProvider messaging.MessageProvider) (*Handler, error) {
	ctx, err := infra.GetContext(dbConfig)
	if err != nil {
		return nil, err
	}

	productsRepo := infra.NewProductsRepository(ctx)
	productsService := application.NewProductsService(productsRepo)

	ordersRepo := infra.NewOrdersRepository(ctx)
	ordersService := application.NewOrdersService(ordersRepo, productsRepo, msgProvider)

	handler := &Handler{productsService, ordersService}

	return handler, nil
}
