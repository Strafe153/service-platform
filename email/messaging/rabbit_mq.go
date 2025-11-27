package messaging

import (
	"fmt"
	"os"
	"strconv"

	amqp "github.com/rabbitmq/amqp091-go"
)

const (
	rabbitMQPort     = "RABBITMQ_PORT"
	rabbitMQHost     = "RABBITMQ_HOST"
	rabbitMQUser     = "RABBITMQ_USER"
	rabbitMQPassword = "RABBITMQ_PASSWORD"
)

type rabbitMQConfig struct {
	host     string
	port     int
	user     string
	password string
}

func readConfig() (*rabbitMQConfig, error) {
	port, err := strconv.Atoi(os.Getenv(rabbitMQPort))

	if err != nil {
		return nil, err
	}

	host := os.Getenv(rabbitMQHost)
	user := os.Getenv(rabbitMQUser)
	password := os.Getenv(rabbitMQPassword)

	config := &rabbitMQConfig{
		host,
		port,
		user,
		password,
	}

	return config, nil
}

type RabbitMQProvider struct {
	conn  *amqp.Connection
	ch    *amqp.Channel
	queue string
}

func NewProvider(queue string) MessageProvider {
	return &RabbitMQProvider{queue: queue}
}

func declareQueue(ch *amqp.Channel, name string) (amqp.Queue, error) {
	return ch.QueueDeclare(name, true, false, false, false, nil)
}

func bindQueue(ch *amqp.Channel, queueName string, exchange string) error {
	return ch.QueueBind(queueName, "", exchange, false, nil)
}

func (p *RabbitMQProvider) Connect() error {
	config, err := readConfig()

	if err != nil {
		return err
	}

	connString := fmt.Sprintf("amqp://%s:%s@%s:%d", config.user, config.password, config.host, config.port)
	conn, err := amqp.Dial(connString)

	if err != nil {
		return err
	}

	ch, err := conn.Channel()

	if err != nil {
		return err
	}

	queue, err := declareQueue(ch, "email")

	if err != nil {
		return err
	}

	if err := bindQueue(ch, queue.Name, "Users.Domain.Events:UserCreatedEvent"); err != nil {
		return err
	}

	p.conn = conn
	p.ch = ch

	return nil
}

func (p *RabbitMQProvider) Consume(handler ConsumeHandler) error {
	delivery, err := p.ch.Consume(p.queue, "", true, false, false, false, nil)

	if err != nil {
		return err
	}

	go func() {
		for m := range delivery {
			handler(m.Body)
		}
	}()

	return nil
}

func (p *RabbitMQProvider) Close() error {
	if p.ch != nil {
		err := p.ch.Close()

		if err != nil {
			return err
		}
	}

	if p.conn != nil {
		return p.conn.Close()
	}

	return nil
}
