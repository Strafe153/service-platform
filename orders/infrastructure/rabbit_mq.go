package infrastructure

import (
	"fmt"
	"orders/domain"

	amqp "github.com/rabbitmq/amqp091-go"
)

type RabbitMQExchange struct {
	Name string `yaml:"name"`
	Kind string `yaml:"kind"`
}

type RabbitMQConfig struct {
	User     string           `yaml:"user"`
	Password string           `yaml:"password"`
	Host     string           `yaml:"host"`
	Port     int              `yaml:"port"`
	Exchange RabbitMQExchange `yaml:"exchange"`
}

func (c *RabbitMQConfig) getConnectionString() string {
	return fmt.Sprintf("amqp://%s:%s@%s:%d", c.User, c.Password, c.Host, c.Port)
}

type RabbitMQProvider struct {
	conn   *amqp.Connection
	config *RabbitMQConfig
}

func NewRabbitMQProvider(config *RabbitMQConfig) domain.MessageProvider {
	return &RabbitMQProvider{config: config}
}

func (p *RabbitMQProvider) Connect() error {
	connStr := p.config.getConnectionString()

	conn, err := amqp.Dial(connStr)
	if err != nil {
		return err
	}

	if err := configureTopology(p, conn); err != nil {
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

	err = ch.Publish(p.config.Exchange.Name, key, false, false, publish)
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

func configureTopology(p *RabbitMQProvider, conn *amqp.Connection) error {
	ch, err := conn.Channel()
	if err != nil {
		return err
	}

	name := p.config.Exchange.Name
	kind := p.config.Exchange.Kind

	if err := ch.ExchangeDeclare(name, kind, true, false, false, false, nil); err != nil {
		return err
	}

	return nil
}
