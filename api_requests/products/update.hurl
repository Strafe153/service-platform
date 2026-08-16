POST http://localhost:8080/realms/users/protocol/openid-connect/token

[Form]
grant_type: password
client_id: user
client_secret: {{CLIENT_SECRET}}
username: {{USERNAME}}
password: {{PASSWORD}}
scope: roles

HTTP 200

[Captures]
token: jsonpath "$.access_token" redact

PUT http://localhost:3333/products/{{PRODUCT_ID}}
Authorization: Bearer {{token}}
{
	"name": "updated",
	"price": 17.17,
	"count": 17
}