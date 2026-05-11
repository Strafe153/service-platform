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

type CreateOrderRequest struct {
	UserId   string                `json:"userId" validate:"required,alphanum,min=26,max=26"`
	Products []domain.OrderProduct `json:"products" validate:"required,min=1,max=999,dive"`
}

func (r *CreateOrderRequest) ToOrder() domain.Order {
	return domain.Order{UserId: r.UserId}
}

type OrderResponse struct {
	Id       string             `json:"id"`
	UserId   string             `json:"userId"`
	Products []ProductResponse  `json:"products"`
	Status   domain.OrderStatus `json:"status"`
}

func newOrderResponse(r *domain.Order) OrderResponse {
	products := make([]ProductResponse, len(r.Products))
	for i, p := range r.Products {
		products[i] = newProductResponse(&p)
	}

	return OrderResponse{
		Id:       r.Id.Hex(),
		UserId:   r.UserId,
		Products: products,
		Status:   r.Status,
	}
}

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

func (s *OrdersService) Get(page domain.Page, c context.Context) ([]OrderResponse, error) {
	orders, err := s.ordersRepo.GetAll(page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]OrderResponse, len(orders))
	for i, o := range orders {
		responses[i] = newOrderResponse(&o)
	}

	return responses, nil
}

func (s *OrdersService) GetById(id string, c context.Context) (*OrderResponse, *domain.AppError) {
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

	response := newOrderResponse(order)

	return &response, nil
}

func (s *OrdersService) GetByUserId(
	id string,
	page domain.Page,
	c context.Context,
) ([]OrderResponse, error) {
	orders, err := s.ordersRepo.GetByUserId(id, page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]OrderResponse, len(orders))
	for i, o := range orders {
		responses[i] = newOrderResponse(&o)
	}

	return responses, nil
}

func (s *OrdersService) Create(
	r *CreateOrderRequest,
	c context.Context,
) (*OrderResponse, *domain.AppError) {
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

	response := newOrderResponse(&order)
	response.Id = id

	if err = publishOrderCreated(s, c, &response); err != nil {
		return nil, domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	return &response, nil
}

func (s *OrdersService) Cancel(id string, c context.Context) *domain.AppError {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	order, err := s.ordersRepo.Get(oId, c)
	if err != nil {
		return domain.NewAppError(domain.ErrNotFound, err.Error())
	}

	if order.Status == domain.CompletedOrder {
		return domain.NewAppError(domain.ErrBadRequest, "Completed order cannot be cancelled")
	}

	if err = s.ordersRepo.Cancel(oId, c); err != nil {
		return domain.NewAppError(domain.ErrBadRequest, "Failed to cancel the order")
	}

	return nil
}

func (s *OrdersService) Complete(id string, c context.Context) *domain.AppError {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return domain.NewAppError(domain.ErrBadRequest, err.Error())
	}

	order, err := s.ordersRepo.Get(oId, c)
	if err != nil {
		return domain.NewAppError(domain.ErrNotFound, err.Error())
	}

	if order.Status == domain.CancelledOrder {
		return domain.NewAppError(domain.ErrBadRequest, "Cancelled order cannot be completed")
	}

	if err = s.ordersRepo.Complete(oId, c); err != nil {
		return domain.NewAppError(domain.ErrBadRequest, "Failed to complete the order")
	}

	if err = publishOrderCompleted(s, order); err != nil {
		return domain.NewAppError(domain.ErrBadRequest, "Failed to publish order completed event")
	}

	return nil
}

func (s *OrdersService) getProducts(
	r *CreateOrderRequest,
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

	products, err := s.productsRepo.GetByIds(productIds, c)
	if err != nil {
		return nil, err
	}

	for i := range products {
		for _, r := range r.Products {
			product := &products[i]

			if product.Id.Hex() == r.Id {
				product.Count = r.Count
			}
		}
	}

	return products, nil
}

func publishOrderCreated(s *OrdersService, c context.Context, r *OrderResponse) error {
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

func publishOrderCompleted(s *OrdersService, r *domain.Order) error {
	err := s.msgProvider.Connect()
	if err != nil {
		return err
	}
	defer s.msgProvider.Close()

	event := domain.OrderCompletedEvent{
		OrderId:     r.Id.Hex(),
		CompletedAt: time.Now().UTC(),
	}

	wrapper, err := domain.WrapMessage(event)
	if err != nil {
		return err
	}

	msgData, err := json.Marshal(wrapper)
	if err != nil {
		return err
	}

	return s.msgProvider.Publish("order.completed", msgData)
}
