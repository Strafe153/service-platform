package infrastructure

import (
	"context"
	"time"

	"go.mongodb.org/mongo-driver/v2/mongo"
	"go.mongodb.org/mongo-driver/v2/mongo/options"
)

type DatabaseConfig struct {
	Connection         string `yaml:"connection"`
	Database           string `yaml:"database"`
	OrdersCollection   string `yaml:"ordersCollection"`
	ProductsCollection string `yaml:"productsCollection"`
}

type DatabaseContext struct {
	config *DatabaseConfig
	client *mongo.Client
}

func GetContext(cfg *DatabaseConfig) (*DatabaseContext, error) {
	client, err := mongo.Connect(options.Client().ApplyURI(cfg.Connection))
	if err != nil {
		return nil, err
	}

	context := &DatabaseContext{cfg, client}

	return context, nil
}

func configureMongoContext(c context.Context) (context.Context, context.CancelFunc) {
	return context.WithTimeout(c, 5*time.Second)
}

func (c *DatabaseContext) getDatabase() *mongo.Database {
	return c.client.Database(c.config.Database)
}

func (c *DatabaseContext) getOrdersCollection() *mongo.Collection {
	return c.getDatabase().Collection(c.config.OrdersCollection)
}

func (c *DatabaseContext) getProductsCollection() *mongo.Collection {
	return c.getDatabase().Collection(c.config.ProductsCollection)
}
