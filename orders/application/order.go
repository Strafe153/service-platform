package application

import (
	"context"
	"encoding/json"
	"orders/domain"
	inf "orders/infrastructure"
	"time"

	"github.com/go-playground/validator/v10"
	"go.mongodb.org/mongo-driver/v2/bson"
)

type OrdersService struct {
	ordersRepo   domain.OrdersRepository
	productsRepo domain.ProductsRepository
	msgProvider  domain.MessageProvider
}

func NewOrdersService(
	o domain.OrdersRepository,
	p domain.ProductsRepository,
	m domain.MessageProvider,
) *OrdersService {
	return &OrdersService{o, p, m}
}

func (s *OrdersService) Get(page domain.Page, c context.Context) ([]domain.OrderResponse, error) {
	orders, err := s.ordersRepo.GetAll(page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]domain.OrderResponse, len(orders))
	for i, o := range orders {
		responses[i] = o.ToResponse()
	}

	return responses, nil
}

func (s *OrdersService) GetById(id string, c context.Context) (*domain.OrderResponse, *domain.AppError) {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return nil, domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	order, err := s.ordersRepo.Get(oId, c)
	if err != nil {
		return nil, domain.NewAppError(domain.ErrNotFound, err.Error())
	}

	isAdmin := c.Value(inf.IsAdmin).(bool)
	userId := c.Value(inf.UserIdClaim)

	if !isAdmin && order.UserId != userId {
		return nil, domain.NewAppError(domain.ErrForbidden, "Access forbidden")
	}

	response := order.ToResponse()

	return &response, nil
}

func (s *OrdersService) GetByUserId(
	id string,
	page domain.Page,
	c context.Context,
) ([]domain.OrderResponse, error) {
	orders, err := s.ordersRepo.GetByUserId(id, page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]domain.OrderResponse, len(orders))
	for i, o := range orders {
		responses[i] = o.ToResponse()
	}

	return responses, nil
}

func (s *OrdersService) Create(
	r *domain.CreateOrderRequest,
	c context.Context,
) (*domain.OrderResponse, *domain.AppError) {
	isAdmin := c.Value(inf.IsAdmin).(bool)
	userId := c.Value(inf.UserIdClaim)

	if !isAdmin && r.UserId != userId {
		return nil, domain.NewAppError(domain.ErrForbidden, "Cannot create an order for the user")
	}

	validate := validator.New()
	if err := validate.Struct(r); err != nil {
		return nil, domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	products, err := s.getProducts(r, c)
	if err != nil {
		return nil, domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	order := r.ToOrder()
	order.Products = products
	order.Status = domain.ActiveOrder

	id, err := s.ordersRepo.Create(&order, c)
	if err != nil {
		return nil, domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	response := order.ToResponse()
	response.Id = id

	publishOrderCreated(s, c, &response)

	return &response, nil
}

func (s *OrdersService) Cancel(id string, c context.Context) error {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return err
	}

	return s.ordersRepo.Cancel(oId, c)
}

func (s *OrdersService) getProducts(
	r *domain.CreateOrderRequest,
	c context.Context,
) ([]domain.Product, error) {
	productIds := make([]bson.ObjectID, len(r.Products))
	for i, product := range r.Products {
		oId, err := bson.ObjectIDFromHex(product.Id)
		if err != nil {
			return nil, err
		}

		productIds[i] = oId
	}

	return s.productsRepo.GetByIds(productIds, c)
}

func publishOrderCreated(s *OrdersService, c context.Context, r *domain.OrderResponse) error {
	err := s.msgProvider.Connect()
	if err != nil {
		return err
	}
	defer s.msgProvider.Close()

	email := c.Value(inf.EmailClaim).(string)

	event := domain.OrderCreatedEvent{
		Email:       email,
		OrderNumber: r.Id,
		CreatedAt:   time.Now().UTC(),
	}

	wrapper, err := domain.WrapMessage(event)
	if err != nil {
		return err
	}

	msgData, err := json.Marshal(wrapper)
	if err != nil {
		return err
	}

	return s.msgProvider.Publish("order.created", msgData)
}
