POST http://localhost:8080/realms/users/protocol/openid-connect/token

[Form]
grant_type: password
client_id: user
client_secret: {{CLIENT_SECRET}}
username: {{USERNAME}}
password: {{PASSWORD}}
scope: roles

HTTP 200

[Asserts]
jsonpath "$.access_token" matches /[a-zA-Z0-9]+\.[a-zA-Z0-9]+\.[a-zA-Z0-9]+/
jsonpath "$.expires_in" > 0
jsonpath "$.refresh_expires_in" > 0
jsonpath "$.refresh_token" matches /[a-zA-Z0-9]+\.[a-zA-Z0-9]+\.[a-zA-Z0-9]+/
jsonpath "$.token_type" == "Bearer"
jsonpath "$.not-before-policy" == 0
jsonpath "$.session_state" isUuid
jsonpath "$.scope" == "email profile"