from fastapi import FastAPI
from pydantic import BaseModel

class Position(BaseModel):
    x: float
    y: float
    z: float


class StateEntry(BaseModel):
    key: str
    value: str


class EntityObservation(BaseModel):
    id: str
    type: str
    position: Position
    state: list[StateEntry]

class ActionResponse(BaseModel):
    action: str
    position: Position | None = None


app = FastAPI() 


@app.post("/action", response_model=ActionResponse)
async def get_action(observation: list[EntityObservation]):

    for obs in observation: 
        print("ID:", obs.id)
        print("Type:", obs.type)
        print("Position:", obs.position)

        for state in obs.state:
            print(state.key, "=", state.value)

    return ActionResponse(
        action="move",
        position=Position(
            x=3.0,
            y=0.0,
            z=3.0
        )
    )
