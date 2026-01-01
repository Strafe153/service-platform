package infrastructure

import (
	"context"
	"fmt"
	"orders/domain"

	"go.mongodb.org/mongo-driver/v2/bson"
)

type OrdersRepository struct {
	context *DatabaseContext
}

func NewOrdersRepository(c *DatabaseContext) *OrdersRepository {
	return &OrdersRepository{c}
}

func (r *OrdersRepository) GetAll(page domain.Page, c context.Context) ([]domain.Order, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getOrdersCollection()
	opts := getPageOptions(page)

	cursor, err := collection.Find(ctx, bson.D{}, opts)
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var orders []domain.Order
	for cursor.Next(ctx) {
		var order domain.Order
		if err := cursor.Decode(&order); err != nil {
			return nil, err
		}

		orders = append(orders, order)
	}

	return orders, nil
}

func (r *OrdersRepository) GetByUserId(id string, page domain.Page, c context.Context) ([]domain.Order, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getOrdersCollection()
	opts := getPageOptions(page)

	cursor, err := collection.Find(ctx, bson.D{{Key: "userId", Value: id}}, opts)
	if err != nil {
		return nil, err
	}
	defer cursor.Close(ctx)

	var orders []domain.Order
	for cursor.Next(ctx) {
		var order domain.Order
		if err := cursor.Decode(&order); err != nil {
			return nil, err
		}

		orders = append(orders, order)
	}

	return orders, nil
}

func (r *OrdersRepository) Get(id bson.ObjectID, c context.Context) (*domain.Order, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getOrdersCollection()

	var order domain.Order
	if err := collection.FindOne(ctx, bson.D{{Key: "_id", Value: id}}).Decode(&order); err != nil {
		return nil, err
	}

	return &order, nil
}

func (r *OrdersRepository) Create(order *domain.Order, c context.Context) (string, error) {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	ordersCollection := r.context.getOrdersCollection()

	res, err := ordersCollection.InsertOne(ctx, order)
	if err != nil {
		return "", err
	}

	id, ok := res.InsertedID.(bson.ObjectID)
	if !ok {
		return "", fmt.Errorf("failed to cast the ID")
	}

	return id.Hex(), nil
}

func (r *OrdersRepository) Cancel(id bson.ObjectID, c context.Context) error {
	ctx, cancel := configureMongoContext(c)
	defer cancel()

	collection := r.context.getOrdersCollection()

	_, err := collection.UpdateOne(
		ctx,
		bson.D{{Key: "_id", Value: id}},
		bson.D{{Key: "$set", Value: bson.D{{Key: "status", Value: domain.CancelledOrder}}}})

	return err
}
