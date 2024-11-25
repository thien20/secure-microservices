from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.endpoint_routers import public, protected, admin

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include endpoint routers
app.include_router(public.router)
app.include_router(protected.router)
app.include_router(admin.router)


# if __name__ == "__main__":
#     import uvicorn
#     uvicorn.run(app, host="0.0.0.0", port=8000)