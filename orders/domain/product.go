package domain

import "go.mongodb.org/mongo-driver/v2/bson"

// Only admin should be able to add products
type Product struct {
	Id          bson.ObjectID `bson:"_id,omitempty"`
	Name        string        `bson:"name"`
	Price       float64       `bson:"price"`
	Count       int           `bson:"count"`
	IsAvailable bool          `bson:"isAvailable"`
}

func (p *Product) ToResponse() ProductResponse {
	return ProductResponse{
		Id:          p.Id.Hex(),
		Name:        p.Name,
		Price:       p.Price,
		Count:       p.Count,
		IsAvailable: p.IsAvailable,
	}
}

type CreateProductRequest struct {
	Name  string  `json:"name"`
	Price float64 `json:"price"`
	Count int     `json:"count"`
}

func (r *CreateProductRequest) ToProduct() Product {
	return Product{
		Name:  r.Name,
		Price: r.Price,
		Count: r.Count,
	}
}

type UpdateProductRequest struct {
	Name  string  `json:"name"`
	Price float64 `json:"price"`
	Count int     `json:"count"`
}

func (r *UpdateProductRequest) ToProduct() Product {
	return Product{
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
