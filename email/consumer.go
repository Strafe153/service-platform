package main

import (
	"encoding/json"
	"fmt"
	"os"
	"strconv"

	amqp "github.com/rabbitmq/amqp091-go"
)

type RabbitMqConfiguration struct {
	host     string
	port     int
	user     string
	password string
}

func readRabbitMqConfig() (*RabbitMqConfiguration, error) {
	port, err := strconv.Atoi(os.Getenv("RABBITMQ_PORT"))

	if err != nil {
		return nil, err
	}

	host := os.Getenv("RABBITMQ_HOST")
	user := os.Getenv("RABBITMQ_USER")
	password := os.Getenv("RABBITMQ_PASSWORD")

	config := &RabbitMqConfiguration{
		host,
		port,
		user,
		password,
	}

	return config, nil
}

func connectRabbitMq() <-chan amqp.Delivery {
	config, err := readRabbitMqConfig()

	if err != nil {
		failOnError(err, "Failed to load RabbitMQ configuration")
	}

	url := fmt.Sprintf("amqp://%s:%s@%s:%d", config.user, config.password, config.host, config.port)
	conn, err := amqp.Dial(url)
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	q, err := ch.QueueDeclare("email", true, false, false, false, nil)
	failOnError(err, "Failed to declare a queue")

	err = ch.QueueBind(q.Name, "", "UsersService.Messaging.Events:UserCreatedEvent", false, nil)
	failOnError(err, "Failed to bind a queue")

	delivery, err := ch.Consume(q.Name, "", true, false, false, false, nil)
	failOnError(err, "Failed to consume")

	return delivery
}

func consume(delivery <-chan amqp.Delivery) {
	for event := range delivery {
		var wrapper MessageWrapper[UserCreatedEvent]

		if err := json.Unmarshal(event.Body, &wrapper); err == nil {
			sendEmail(wrapper.Message)
		} else {
			failOnError(err, "Failed to unmarshal")
		}
	}
}
