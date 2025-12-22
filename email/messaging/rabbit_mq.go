package messaging

import (
	"fmt"
	"log"

	amqp "github.com/rabbitmq/amqp091-go"
)

type RabbitMQQueue struct {
	Name       string `json:"name"`
	RoutingKey string `json:"routingKey"`
}

type RabbitMQExchange struct {
	Name   string          `json:"name"`
	Kind   string          `json:"kind"`
	Queues []RabbitMQQueue `json:"queues"`
}

type RabbitMQConfig struct {
	User      string             `json:"user"`
	Password  string             `json:"password"`
	Host      string             `json:"host"`
	Port      int                `json:"port"`
	Exchanges []RabbitMQExchange `json:"exchanges"`
}

func (c *RabbitMQConfig) getConnectionString() string {
	return fmt.Sprintf("amqp://%s:%s@%s:%d", c.User, c.Password, c.Host, c.Port)
}

type RabbitMQProvider struct {
	conn     *amqp.Connection
	config   *RabbitMQConfig
	handlers map[string]ConsumeHandler
	channels []*amqp.Channel
}

func declareQueue(ch *amqp.Channel, name string) (amqp.Queue, error) {
	return ch.QueueDeclare(name, true, false, false, false, nil)
}

func configureTopology(p *RabbitMQProvider, conn *amqp.Connection) error {
	for _, ex := range p.config.Exchanges {
		ch, err := conn.Channel()
		if err != nil {
			return err
		}

		p.channels = append(p.channels, ch)
		if err := ch.ExchangeDeclare(ex.Name, ex.Kind, true, false, false, false, nil); err != nil {
			return err
		}

		for _, q := range ex.Queues {
			if _, err := declareQueue(ch, q.Name); err != nil {
				return err
			}

			if err := ch.QueueBind(q.Name, q.RoutingKey, ex.Name, false, nil); err != nil {
				return err
			}
		}
	}

	return nil
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

func (p *RabbitMQProvider) AttachHandler(queue string, handler ConsumeHandler) error {
	p.handlers[queue] = handler
	return nil
}

func (p *RabbitMQProvider) Consume() error {
	for _, ex := range p.config.Exchanges {
		for _, q := range ex.Queues {
			handler, ok := p.handlers[q.Name]
			if !ok {
				return fmt.Errorf("no handler registered for queue %s", q.Name)
			}

			ch, err := p.conn.Channel()
			if err != nil {
				return err
			}

			delivery, err := ch.Consume(q.Name, "", true, false, false, false, nil)
			if err != nil {
				return err
			}

			go func(queue string, handler ConsumeHandler) {
				for msg := range delivery {
					if err := handler(msg.Body); err != nil {
						log.Printf("queue %s handler error %v", queue, err)
					}
				}
			}(q.Name, handler)
		}
	}

	return nil
}

func (p *RabbitMQProvider) Close() error {
	if len(p.handlers) > 0 {
		for _, c := range p.channels {
			// error is ignored due to the channel potentially already being closed
			// and not to return from the function to close other channels even if
			// one of them fails
			c.Close()
		}
	}

	if p.conn != nil {
		return p.conn.Close()
	}

	return nil
}

func NewRabbitMQProvider(config *RabbitMQConfig) MessageProvider {
	return &RabbitMQProvider{
		config:   config,
		handlers: make(map[string]ConsumeHandler),
	}
}
