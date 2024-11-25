import pytest
from fastapi.testclient import TestClient
from app.main import app
from app.auth_utils import generate_jwt, validate_jwt

client = TestClient(app)

@pytest.fixture
def valid_admin_token():
    """Generate a valid token for an admin user."""
    return generate_jwt({"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "Admin User", 
                      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin"})

@pytest.fixture
def valid_user_token():
    """Generate a valid token for a non-admin user."""
    return generate_jwt({"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": "Regular User", 
                      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "User"})

@pytest.fixture
def invalid_token():
    """Generate an invalid token."""
    return "invalid token"

def test_public_data_access():
    """Verify public endpoint is accessible without authentication."""
    response = client.get("/public-data")
    assert response.status_code == 200
    assert response.json() == {"message": "This is public data accessible to everyone."}

def test_protected_data_valid_token(valid_user_token):
    """Verify protected endpoint is accessible with a valid token."""
    response = client.get(
        "/protected-data", headers={"Authorization": f"Bearer {valid_user_token}"}
    )
    assert response.status_code == 200
    assert "message" in response.json()
    assert response.json()["message"].startswith("Hello Regular User")

def test_protected_data_invalid_token(invalid_token):
    """Verify protected endpoint rejects an invalid token."""
    response = client.get(
        "/protected-data", headers={"Authorization": f"Bearer {invalid_token}"}
    )
    assert response.status_code == 401
    assert response.json() == {"detail": "Invalid token"}

def test_admin_data_with_admin_token(valid_admin_token):
    """Verify admin endpoint is accessible with a valid admin token."""
    response = client.get(
        "/admin-data", headers={"Authorization": f"Bearer {valid_admin_token}"}
    )
    assert response.status_code == 200
    assert "message" in response.json()
    assert response.json()["message"].startswith("Hello Admin User")

def test_admin_data_with_user_token(valid_user_token):
    """Verify admin endpoint denies access to a non-admin user."""
    response = client.get(
        "/admin-data", headers={"Authorization": f"Bearer {valid_user_token}"}
    )
    assert response.status_code == 403
    assert response.json() == {"detail": "Access forbidden: Admins only."}

def test_admin_data_invalid_token(invalid_token):
    """Verify admin endpoint rejects an invalid token."""
    response = client.get(
        "/admin-data", headers={"Authorization": f"Bearer {invalid_token}"}
    )
    assert response.status_code == 401
    assert response.json() == {"detail": "Invalid token"}
