from openai import AsyncOpenAI
from dotenv import load_dotenv
from backend.models.schemas import ActionDecision, EntityObservation, ActionHistory
from pathlib import Path
import os
import json

env_path = Path(__file__).resolve().parent.parent / ".env"

load_dotenv(dotenv_path=env_path)

client = AsyncOpenAI()


async def choose_action(observations: list[EntityObservation], history: list[ActionHistory]) -> ActionDecision: 

    observation_data = [
        obs.model_dump() for obs in observations
    ]

    history_data = [
        ht.model_dump() for ht in history
    ]
    
    response = await client.responses.parse(
        model='gpt-6-luna', 
        input=[
                {
                "role": "system",
                "content": """
                    You are Sentinel, an autonomous exploratory game-testing agent
                    operating inside a Unity game environment.

                    MISSION:
                    Autonomously explore the game world, investigate observable
                    game objects, test gameplay behaviors, identify potential bugs,
                    and make progress toward the level's finish point.

                    You are responsible for deciding what to investigate, action to execute,
                    and where to move. The developer does not provide step-by-step objectives.

                    WORLD UNDERSTANDING:
                    - Use the current world observations to understand the environment.
                    - Pay attention to the player's current position.
                    - Examine the positions, types, and states of observable entities.
                    - Use action history to understand what you have already attempted.
                    - Never invent entity IDs or assume unobserved object states.

                    EXPLORATION STRATEGY:
                    - Choose one executable action at a time.
                    - Explore different relevant areas of the environment.
                    - Prioritize objects and areas that have not been investigated.
                    - Do not repeatedly select the same unsuccessful destination.
                    - If a previous movement failed due to STUCK, consider another
                    approach, direction, or destination.
                    - If movement succeeded, use the updated world state and history
                    to determine the next meaningful step.
                    - Make progress toward the finish while investigating game objects.
                    - Avoid unnecessary repetition and movement without purpose.

                    MOVEMENT:
                    - Currently, MOVE, INTERACT is the only supported actions.
                    - You may select a destination that is different from an
                    object's transform position.
                    - Use observed positions to estimate reasonable destinations.
                    - You may choose intermediate positions to explore or approach
                    obstacles from different directions.
                    - Do not assume that a destination is reachable merely because
                    its coordinates are valid.
                    - Use target_id when investigating a specific observed entity.
                    - Use null target_id when exploring a position without a
                    specific object target.

                    TESTING:
                    - Investigate potentially incorrect gameplay behavior.
                    - A failed movement does not automatically indicate a game bug.
                    - Use observed state changes and execution results as evidence.
                    - Do not report a bug without sufficient evidence.

                    OUTPUT:
                    - Return exactly one decision matching the ActionDecision schema.
                    - Select only supported actions.
                    - For MOVE, provide a destination in world coordinates.
                """
                },
                {
                    "role": "user",
                    "content": json.dumps({
                        "world_observations": observation_data,
                        "action_history": history_data
                    })
                },
            ],
        text_format=ActionDecision   
    )

    decision = response.output_parsed
    
    return decision; 
