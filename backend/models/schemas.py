from pydantic import BaseModel
from enum import Enum
from typing import Literal

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
    target_id: str | None = None

class ActionDecision(BaseModel):
    action: ActionType
    target_id: str | None
    destination: Position | None

class ActionHistory(BaseModel): 
    action: str
    target_id: str | None = None
    action_status: Literal["succeeded", "failed"]
    reason: str | None = None

class ActionRequest(BaseModel): 
    observations: list[EntityObservation]
    history: list[ActionHistory]