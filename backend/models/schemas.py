from pydantic import BaseModel
from enum import Enum, IntEnum

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

class ActionType(str, Enum): 
    MOVE = "move"
    SPRINT = "sprint"
    CROUCH = "crouch"
    INTERACT = "interact"
    JUMP = "jump"

class ActionResponse(BaseModel):
    action: str
    position: Position | None = None

class ActionDecision(BaseModel): 
    action: ActionType
    target_id: str | None 

