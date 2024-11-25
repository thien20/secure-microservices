from fastapi import APIRouter

router = APIRouter()

@router.get("/public-data")
def get_public_data():
    return {"message": "This is public data accessible to everyone."}
