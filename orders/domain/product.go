package domain

import (
	"context"

	"go.mongodb.org/mongo-driver/v2/bson"
)

type Product struct {
	Id          bson.ObjectID `bson:"_id,omitempty"`
	Name        string        `bson:"name"`
	Price       float64       `bson:"price"`
	Count       int           `bson:"count"`
	IsAvailable bool          `bson:"isAvailable"`
}

type ProductsRepository interface {
	GetAll(page Page, c context.Context) ([]Product, error)
	GetByIds(ids []bson.ObjectID, c context.Context) ([]Product, error)
	Get(id bson.ObjectID, c context.Context) (*Product, error)
	Create(product *Product, c context.Context) (string, error)
	Update(id bson.ObjectID, product *Product, c context.Context) error
	Discontinue(id bson.ObjectID, c context.Context) error
}
