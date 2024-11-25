from fastapi import APIRouter, HTTPException, Security
from app.auth_utils import validate_jwt, security

router = APIRouter()

@router.get("/admin-data")
def get_admin_data(token: str = Security(security)):
    user = validate_jwt(token.credentials)
    print(f"User: {user.get('role')}")
    if user.get("role") != "Admin":
        raise HTTPException(status_code=403, detail="Access forbidden: Admins only.")
    return {"message": f"Hello {user['name']}, welcome to the admin data endpoint."}
