from fastapi import APIRouter, HTTPException
from backend.service.openai_service import choose_action

from backend.models.schemas import (
    ActionResponse,
    EntityObservation,
    Position,
    ActionRequest,
    ActionType
)


router = APIRouter()

class NullPointerException(Exception):
    pass

@router.post("/action", response_model=ActionResponse)
async def get_action(req: ActionRequest):

    decision = await choose_action(
        req.observations, 
        req.history
    )

    if decision is None: 
        raise HTTPException(
            status_code=502,
            detail="LLM returned no decision"
        )

    if decision.action == "move" and decision.destination is None: 
        raise HTTPException(
            status_code=502,
            detail="Destination is not assigned"
        )

    if decision.action not in ActionType: 
        raise HTTPException(
            status_code=502,
            detail="Action is not supported"
        )


    if decision.target_id is not None:
        target_exists = any(
            obs.id == decision.target_id
            for obs in req.observations
        )

        if not target_exists:
            raise HTTPException(
                status_code=502,
                detail="Target entity does not exist"
            )

    return ActionResponse(
        action=decision.action, 
        position=decision.destination, 
        target_id=decision.target_id
    )
    

