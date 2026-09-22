from fastapi import FastAPI
from pydantic import BaseModel

class Observation(BaseModel): 
    has_key: bool
    door_locked: bool

app = FastAPI() 

@app.get("/heath")
def get_health(): 
    return {"Status": "Running"}

@app.post('/action')
def send_action(act: Observation): 
    response = { 
        "action": "move",
        "position": "12.2 , 3.4, 5.4"
    }

    return response
