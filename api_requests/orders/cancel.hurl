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

POST http://localhost:3333/orders/{{ORDER_ID}}/cancel
Authorization: Bearer {{token}}