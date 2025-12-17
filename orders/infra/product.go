package infra

import (
	"context"
	"fmt"
	"orders/domain"

	"go.mongodb.org/mongo-driver/v2/bson"
)

type ProductsRepository struct {
	context *DatabaseContext
}

func NewProductsRepository(c *DatabaseContext) *ProductsRepository {
	return &ProductsRepository{c}
}

func (r *ProductsRepository) GetAll(c context.Context) ([]*domain.Product, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()

	cursor, err := collection.Find(ctx, bson.D{})
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var products []*domain.Product
	for cursor.Next(ctx) {
		var product domain.Product
		if err := cursor.Decode(&product); err != nil {
			return nil, err
		}

		products = append(products, &product)
	}

	return products, nil
}

func (r *ProductsRepository) GetByIds(ids []bson.ObjectID, c context.Context) ([]domain.Product, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()
	filter := bson.D{{Key: "_id", Value: bson.D{{Key: "$in", Value: ids}}}}

	cursor, err := collection.Find(ctx, filter)
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var products []domain.Product
	for cursor.Next(ctx) {
		var product domain.Product
		if err := cursor.Decode(&product); err != nil {
			return nil, err
		}

		products = append(products, product)
	}

	return products, nil
}

func (r *ProductsRepository) Get(id bson.ObjectID, c context.Context) (*domain.Product, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()

	var product domain.Product
	if err := collection.FindOne(ctx, bson.D{{Key: "_id", Value: id}}).Decode(&product); err != nil {
		return nil, err
	}

	return &product, nil
}

func (r *ProductsRepository) Create(product *domain.Product, c context.Context) (string, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()

	result, err := collection.InsertOne(ctx, product)
	if err != nil {
		return "", err
	}

	id, ok := result.InsertedID.(bson.ObjectID)
	if !ok {
		return "", fmt.Errorf("failed to cast the ID")
	}

	return id.Hex(), nil
}

func (r *ProductsRepository) Update(id bson.ObjectID, product *domain.Product, c context.Context) error {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()
	_, err := collection.ReplaceOne(ctx, bson.D{{Key: "_id", Value: id}}, product)

	return err
}

func (r *ProductsRepository) Discontinue(id bson.ObjectID, c context.Context) error {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getProductsCollection()

	_, err := collection.UpdateOne(
		ctx,
		bson.D{{Key: "_id", Value: id}},
		bson.D{{Key: "$set", Value: bson.D{{Key: "isAvailable", Value: false}}}})

	return err
}
