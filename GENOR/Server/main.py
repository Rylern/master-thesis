from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from .routers import indicator, category




origins = [
    "https://rylern.itch.io/genor"
]


app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(indicator.router)
app.include_router(category.router)
