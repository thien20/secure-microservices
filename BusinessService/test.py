from app.auth_utils import generate_jwt, validate_jwt

print(generate_jwt({"name": "Admin User", "role": "Admin"}))
token = generate_jwt({"name": "Admin User", "role": "Admin"})
decode = validate_jwt(token)
print(decode)

token = generate_jwt({"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "Admin User", 
                      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin"})

print("Generated Token:", token)

decoded = validate_jwt(token)
print("Decoded Token:", decoded)
