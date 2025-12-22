package application

import (
	"context"
	"encoding/json"
	"orders/domain"
	"orders/messaging"
	"time"

	"github.com/go-playground/validator/v10"
	"go.mongodb.org/mongo-driver/v2/bson"
)

type OrdersService struct {
	ordersRepo   domain.OrdersRepository
	productsRepo domain.ProductsRepository
	msgProvider  messaging.MessageProvider
}

func NewOrdersService(
	o domain.OrdersRepository,
	p domain.ProductsRepository,
	m messaging.MessageProvider,
) *OrdersService {
	return &OrdersService{o, p, m}
}

func (s *OrdersService) Get(c context.Context) ([]*domain.OrderResponse, error) {
	orders, err := s.ordersRepo.GetAll(c)
	if err != nil {
		return nil, err
	}

	responses := make([]*domain.OrderResponse, len(orders))
	for i, o := range orders {
		response := o.ToResponse()
		responses[i] = &response
	}

	return responses, nil
}

func (s *OrdersService) GetById(id string, c context.Context) (*domain.OrderResponse, error) {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return nil, err
	}

	order, err := s.ordersRepo.Get(oId, c)
	if err != nil {
		return nil, err
	}

	response := order.ToResponse()

	return &response, nil
}

func (s *OrdersService) GetByUserId(id string, c context.Context) ([]*domain.OrderResponse, error) {
	orders, err := s.ordersRepo.GetByUserId(id, c)
	if err != nil {
		return nil, err
	}

	responses := make([]*domain.OrderResponse, len(orders))
	for i, o := range orders {
		response := o.ToResponse()
		responses[i] = &response
	}

	return responses, nil
}

func (s *OrdersService) Create(
	request *domain.CreateOrderRequest,
	c context.Context,
) (*domain.OrderResponse, error) {
	validate := validator.New()
	if err := validate.Struct(request); err != nil {
		return nil, err
	}

	products, err := s.getProducts(request, c)
	if err != nil {
		return nil, err
	}

	order := request.ToOrder()
	order.Products = products
	order.Status = domain.ActiveOrder

	id, err := s.ordersRepo.Create(&order, c)
	if err != nil {
		return nil, err
	}

	response := order.ToResponse()
	response.Id = id

	if err := publishOrderCreated(s, &response); err != nil {
		return nil, err
	}

	return &response, nil
}

func publishOrderCreated(s *OrdersService, response *domain.OrderResponse) error {
	err := s.msgProvider.Connect()
	if err != nil {
		return err
	}
	defer s.msgProvider.Close()

	event := domain.OrderCreatedEvent{
		Email:       "test@test.com",
		OrderNumber: response.Id,
		CreatedAt:   time.Now().UTC(),
	}

	wrapper, err := messaging.WrapMessage(event)
	if err != nil {
		return err
	}

	msgData, err := json.Marshal(wrapper)
	if err != nil {
		return err
	}

	return s.msgProvider.Publish("order.created", msgData)
}

func (s *OrdersService) Cancel(id string, c context.Context) error {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return err
	}

	return s.ordersRepo.Cancel(oId, c)
}

func (s *OrdersService) getProducts(
	request *domain.CreateOrderRequest,
	c context.Context,
) ([]domain.Product, error) {
	productIds := make([]bson.ObjectID, len(request.Products))
	for i, product := range request.Products {
		oId, err := bson.ObjectIDFromHex(product.Id)
		if err != nil {
			return nil, err
		}

		productIds[i] = oId
	}

	return s.productsRepo.GetByIds(productIds, c)
}
