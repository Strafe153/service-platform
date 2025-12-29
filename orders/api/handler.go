package api

import (
	app "orders/application"
	"orders/domain"
	inf "orders/infrastructure"
)

type Handler struct {
	productsService *app.ProductsService
	ordersService   *app.OrdersService
}

func GetHandler(dbConfig *inf.DatabaseConfig, msgProvider domain.MessageProvider) (*Handler, error) {
	ctx, err := inf.GetContext(dbConfig)
	if err != nil {
		return nil, err
	}

	productsRepo := inf.NewProductsRepository(ctx)
	productsService := app.NewProductsService(productsRepo)

	ordersRepo := inf.NewOrdersRepository(ctx)
	ordersService := app.NewOrdersService(ordersRepo, productsRepo, msgProvider)

	handler := &Handler{productsService, ordersService}

	return handler, nil
}
