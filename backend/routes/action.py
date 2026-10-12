from fastapi import APIRouter
from service.openai_service import choose_action

from models.schemas import (
    ActionResponse,
    EntityObservation,
    Position,
)


router = APIRouter()

@router.post("/action", response_model=ActionResponse)
async def get_action(observation: list[EntityObservation]):

    print("Observation List: ", observation)
    response = await choose_action(observation)

    for obs in observation: 
        if response.target_id == obs.id:    
            return ActionResponse(
                action=response.action,
                position=obs.position
            )

    return; 