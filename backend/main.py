from fastapi import FastAPI
from backend.routes.action import router as action_router

app = FastAPI() 

app.include_router(action_router)

