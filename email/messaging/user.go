package messaging

type UserCreatedEvent struct {
	Email     string
	CreatedAt string
}

type UserDeletedEvent struct {
	Email     string
	DeletedAt string
}
