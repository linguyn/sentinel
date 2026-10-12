from openai import AsyncOpenAI
from dotenv import load_dotenv
from models.schemas import ActionDecision, EntityObservation
import json

load_dotenv()

client = AsyncOpenAI()


async def choose_action(observations: list[EntityObservation]) -> ActionDecision: 

    observation_data = [
        obs.model_dump() for obs in observations
    ]

    response = await client.responses.parse(
        model='gpt-6-luna', 
        input=[
            {
                "role": "system",
                "content": """
                You are Sentinel, an autonomous game-testing agent.

                Your mission is to explore the game environment,
                investigate observable objects, and progress toward
                the level's finish point.

                Rules:
                - Analyze the current world observations.
                - Choose one executable action at a time.
                - Select targets using their existing entity IDs.
                - Never invent objects or entity IDs.
                - Only choose from the supported actions.
                - Investigate potentially incorrect gameplay behavior.
                - Do not claim a bug without sufficient evidence.
                - Return a decision matching the required schema.

                Currently supported action: MOVE.
                """
            },
            {
                "role": "user", 
                "content": json.dumps(observation_data)
            },
        ],
        text_format=ActionDecision   
        )

    decision = response.output_parsed

    if decision is None:
        raise RuntimeError("LLM returned no action decision")
    
    return decision; 
