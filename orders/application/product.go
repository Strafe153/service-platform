package application

import (
	"context"
	"orders/domain"

	"github.com/go-playground/validator/v10"
	"go.mongodb.org/mongo-driver/v2/bson"
)

type CreateProductRequest struct {
	Name  string  `json:"name" validate:"required,min=3,max=100"`
	Price float64 `json:"price" validate:"required,gte=1.0,lte=999999.99"`
	Count int     `json:"count" validate:"required,min=1,max=99999"`
}

func (r *CreateProductRequest) ToProduct() domain.Product {
	return domain.Product{
		Name:  r.Name,
		Price: r.Price,
		Count: r.Count,
	}
}

type UpdateProductRequest struct {
	Name  string  `json:"name" validate:"required,min=3,max=100"`
	Price float64 `json:"price" validate:"required,gte=1.0,lte=999999.99"`
	Count int     `json:"count" validate:"required,min=1,max=99999"`
}

func (r *UpdateProductRequest) ToProduct() domain.Product {
	return domain.Product{
		Name:  r.Name,
		Price: r.Price,
		Count: r.Count,
	}
}

type ProductResponse struct {
	Id          string  `json:"id"`
	Name        string  `json:"name"`
	Price       float64 `json:"price"`
	Count       int     `json:"count"`
	IsAvailable bool    `json:"isAvailable"`
}

type ProductsService struct {
	repository domain.ProductsRepository
}

func NewProductsService(repository domain.ProductsRepository) *ProductsService {
	return &ProductsService{repository: repository}
}

func (s *ProductsService) Get(page domain.Page, c context.Context) ([]ProductResponse, error) {
	products, err := s.repository.GetAll(page, c)
	if err != nil {
		return nil, err
	}

	responses := make([]ProductResponse, len(products))
	for i, p := range products {
		responses[i] = newProductResponse(&p)
	}

	return responses, nil
}

func (s *ProductsService) GetById(id string, c context.Context) (*ProductResponse, error) {
	oId, err := bson.ObjectIDFromHex(id)
	if err != nil {
		return nil, err
	}

	product, err := s.repository.Get(oId, c)
	if err != nil {
		return nil, err
	}

	response := newProductResponse(product)

	return &response, nil
}

func (s *ProductsService) Create(
	request *CreateProductRequest,
	c context.Context,
) (*ProductResponse, error) {
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

	response := newProductResponse(&product)
	response.Id = id

	return &response, nil
}

func (s *ProductsService) Update(
	id string,
	request *UpdateProductRequest,
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

func newProductResponse(p *domain.Product) ProductResponse {
	return ProductResponse{
		Id:          p.Id.Hex(),
		Name:        p.Name,
		Price:       p.Price,
		Count:       p.Count,
		IsAvailable: p.IsAvailable,
	}
}
