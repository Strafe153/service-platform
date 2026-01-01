package application

import (
	"context"
	"orders/domain"

	"github.com/go-playground/validator/v10"
	"go.mongodb.org/mongo-driver/v2/bson"
)

type ProductsService struct {
	repository domain.ProductsRepository
}

func NewProductsService(repository domain.ProductsRepository) *ProductsService {
	return &ProductsService{repository: repository}
}

func (s *ProductsService) Get(page domain.Page, c context.Context) ([]domain.ProductResponse, error) {
	products, err := s.repository.GetAll(page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]domain.ProductResponse, len(products))
	for i, p := range products {
		responses[i] = p.ToResponse()
	}

	return responses, nil
}

func (s *ProductsService) GetById(id string, c context.Context) (*domain.ProductResponse, error) {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return nil, err
	}

	product, err := s.repository.Get(oId, c)
	if err != nil {
		return nil, err
	}

	response := product.ToResponse()

	return &response, nil
}

func (s *ProductsService) Create(
	request *domain.CreateProductRequest,
	c context.Context,
) (*domain.ProductResponse, error) {
	validate := validator.New()
	if err := validate.Struct(request); err != nil {
		return nil, err
	}

	product := request.ToProduct()
	product.IsAvailable = true

	id, err := s.repository.Create(&product, c)
	if err != nil {
		return nil, err
	}

	response := product.ToResponse()
	response.Id = id

	return &response, nil
}

func (s *ProductsService) Update(
	id string,
	request *domain.UpdateProductRequest,
	c context.Context,
) error {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return err
	}

	validate := validator.New()
	if err := validate.Struct(request); err != nil {
		return err
	}

	_, err = s.repository.Get(oId, c)
	if err != nil {
		return err
	}

	product := request.ToProduct()
	product.IsAvailable = true

	return s.repository.Update(oId, &product, c)
}

func (s *ProductsService) Discontinue(id string, c context.Context) error {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return err
	}

	return s.repository.Discontinue(oId, c)
}
