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

PUT http://localhost:5062/users/{{USER_ID}}/address
Authorization: Bearer {{token}}
{
	"country": "country",
	"state": "state",
	"city": "city",
	"zipCode": "zipCode",
	"street": "street"
}