from fastapi import APIRouter, Security
from app.auth_utils import validate_jwt, security

router = APIRouter()

@router.get("/protected-data")
def get_protected_data(token: str = Security(security)):
    user = validate_jwt(token.credentials)
    return {"message": f"Hello {user['name']}, this is protected data."}
