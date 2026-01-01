package infrastructure

import (
	"orders/domain"

	"go.mongodb.org/mongo-driver/v2/mongo/options"
)

func getPageOptions(page domain.Page) *options.FindOptionsBuilder {
	skip := int64((page.Number - 1) * page.Size)
	limit := int64(page.Size)

	opts := options.Find().SetSkip(skip).SetLimit(limit)

	return opts
}
