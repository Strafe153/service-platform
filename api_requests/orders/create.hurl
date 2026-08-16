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

POST http://localhost:3333/orders
Authorization: Bearer {{token}}
{
	"userId": "d6b9d37c-c652-4ce1-85ea-7403506f83be",
	"products": [
		{
			"id": "69d7c3f426729c5fe5331b2d",
			"count": 555
		},
		{
			"id": "69d7c40a26729c5fe5331b2e",
			"count": 555
		}
	]
}