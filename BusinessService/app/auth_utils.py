from fastapi import HTTPException
from fastapi.security import HTTPBearer
from jose import jwt
from jose.exceptions import JWTError
from dotenv import load_dotenv
from datetime import datetime, timedelta

import os

load_dotenv()
security = HTTPBearer()

JWT_SECRET = os.getenv("JWT_KEY") 
JWT_ALGORITHM = "HS256"
JWT_ISSUER = "AuthAPI"   # Issuer set by C# service
JWT_AUDIENCE = "BusinessAPI"  # Audience for the Python service

def validate_jwt(token: str):
    """
    Validate JWT token issued by the C# service.
    """
    try:
        payload = jwt.decode(
            token,
            JWT_SECRET,
            algorithms=[JWT_ALGORITHM],
            issuer=JWT_ISSUER,
            audience=JWT_AUDIENCE)
        
        # print(f"Token payload: {payload}")
        user = {
            "name": payload.get("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"),
            "role": payload.get("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"),
        }
        return user
    except JWTError as e:
        print(f"Error validation error: {e}")
        raise HTTPException(status_code=401, detail="Invalid token") from e

def test_validate_jwt(token: str):
    """
    Validate JWT token issued by the C# service.
    """
    try:
        payload = jwt.decode(
            token,
            JWT_SECRET,
            algorithms=[JWT_ALGORITHM],
            issuer=JWT_ISSUER,
            audience=JWT_AUDIENCE)
        
        user = {
            "name": payload.get("name"),
            "role": payload.get("role"),
        }
        return user
    except JWTError as e:
        print(f"Error validation error: {e}")
        raise HTTPException(status_code=401, detail="Invalid token") from e


def generate_jwt(payload: dict):
    """
    Generate a JWT token with the required claims.
    Args:
        payload (dict): The payload data for the JWT (e.g., name, role).
    Returns:
        str: Encoded JWT token.
    """
    payload.update({
        "exp": datetime.utcnow() + timedelta(hours=1),  # Expiration time
        "iss": JWT_ISSUER,  # Issuer
        "aud": JWT_AUDIENCE,  # Audience
    })
    return jwt.encode(payload, JWT_SECRET, algorithm=JWT_ALGORITHM)
