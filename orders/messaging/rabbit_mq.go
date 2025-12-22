package messaging

import (
	"fmt"

	amqp "github.com/rabbitmq/amqp091-go"
)

type RabbitMQConfig struct {
	User     string `json:"user"`
	Password string `json:"password"`
	Host     string `json:"host"`
	Port     int    `json:"port"`
	Exchange string `json:"exchange"`
}

func (c *RabbitMQConfig) getConnectionString() string {
	return fmt.Sprintf("amqp://%s:%s@%s:%d", c.User, c.Password, c.Host, c.Port)
}

type RabbitMQProvider struct {
	conn   *amqp.Connection
	config *RabbitMQConfig
}

func NewRabbitMQProvider(config *RabbitMQConfig) MessageProvider {
	return &RabbitMQProvider{config: config}
}

func (p *RabbitMQProvider) Connect() error {
	connStr := p.config.getConnectionString()

	conn, err := amqp.Dial(connStr)
	if err != nil {
		return err
	}

	p.conn = conn

	return nil
}

func (p *RabbitMQProvider) Publish(key string, msg []byte) error {
	ch, err := p.conn.Channel()
	if err != nil {
		return err
	}

	publish := amqp.Publishing{ContentType: "application/json", Body: msg}

	err = ch.Publish(p.config.Exchange, key, false, false, publish)
	if err != nil {
		return err
	}

	return nil
}

func (p *RabbitMQProvider) Close() error {
	if p.conn != nil {
		return p.conn.Close()
	}

	return nil
}
