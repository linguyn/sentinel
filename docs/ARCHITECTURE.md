# Sentinel architecture 

Overall Sentinel Architecture

## Product scope and decisions

Sentinel is an AI-assisted autonomous gameplay testing platform for Unity developers. Its long-term goal is to understand available mechanics, plan tests, execute gameplay actions, verify outcomes, and produce evidence-based QA reports.

The intended experience is install, authorize inspection, review generated integration, validate, run, and inspect results. AI reduces manual integration work for supported projects; The top priority is maximizing automatic compatibility across diverse Unity projects through AI-generated adapters, while minimizing manual integration. Sentinel must explicitly identify unsupported mechanics, unresolved dependencies, and integration failures rather than assuming universal compatibility.

The product goal includes both an installable integration for developers' own worlds and a deployed interactive link where interviewers can try a prepared demo. These are separate delivery surfaces. Browser execution, hosting, deployment infrastructure, and supported Unity configurations are not selected or validated.

## Five-stage product vision 

### 1. UNDERSTAND

UNDERSTAND analyzes developer-authorized scripts, scene objects, and runtime information to discover actions and controls, observable entities and states, constraints, and relationships. Its output is a structured description of capabilities, including evidence and uncertainty. Source inspection alone cannot determine intended gameplay behavior, and source code is only sent to external services with the developer's authorization.

### 2. CONNECT

An AI integration agent automatically generates game-specific adapters based on the gameplay systems, actions, and observable states discovered during the UNDERSTAND stage.

The generated adapters connect the developer's existing Unity systems to Sentinel's standardized interfaces, allowing Sentinel to interact with different games without requiring developers to manually implement each adapter.

All AI-generated code is presented for review before it is applied. Original gameplay scripts are only modified with explicit developer authorization.

Sentinel then automatically validates the integration through interface compatibility checks, compilation, actual action execution, correct state observation, and intended-to-actual target mapping.

If validation fails, the agent analyzes the errors, proposes corrections, and retries within approved boundaries.

Output: A validated, AI-generated integration that connects the developer's game to Sentinel's autonomous testing engine.

### 3. PLAN

PLAN uses the available mechanics and validated requirements to propose executable tests. Each test records its objective, preconditions, relevant entities and actions, expected outcomes, and the evidence needed to judge the result. Developer-confirmed requirements are kept distinct from AI-generated hypotheses. Choosing the next exploration action is not a complete test plan.

### 4. PLAY & CHECK

PLAY & CHECK executes actions and observes the resulting gameplay state. It tracks three separate concepts:

1. Execution: Coordinates action dispatch, lifecycle, completion, cancellation, timeout, and failure. Requests and results are associated with their originating test session. Stale responses must not modify a stopped or replaced session.
2. Actual gameplay state changes, including timing and target identity.
3. Test verification: PASS, FAIL, or INCONCLUSIVE against an explicit expectation.

A movement failure or blocked interaction is not automatically a bug. A locked-door opening can be blocked while the test passes. Missing evidence, uncertain requirements, or invalid setup can make a test inconclusive. The design includes environment reset, recovery, and reproducibility.

### 5. REPORT

A report contains objectives and outcomes, expected versus actual behavior, action sequences/history, supporting evidence, reproduction steps, limitations, and inconclusive findings. Suspected bugs are distinguished from sufficiently evidenced failures; a failed action or an AI suggestion alone does not confirm a bug.

## Responsibilities and boundaries

### SETUP / INTEGRATION 
```text
                         Authorized Project Inspection
                                   |
                                   v
                              UNDERSTAND (AI)
                                   |
                              Capability Discovery
                                   |
                                   v
                              CONNECT (AI)
                                   |
                              Generate Adapters
                                   |
                              Developer Review
                                   |
                              Integration Validation
                                   |
                                   v
                              Validated Integration
```
### RUNTIME / TESTING 
```text

                                   UNITY GAME
                                        ↕
                              Game-specific Adapters
                                        ↕
                                 SENTINEL RUNTIME
                                  /           \
                                 /             \
                         Action Registry    World Observation
                                 \             /
                                  \           /
                                   FASTAPI + LLM
                                        |
                                   Action Decision
                                        |
                                   Generic Executor
                                        |
                                Registry → Action Adapter
                                        |
                                    Unity Game
                                        |
                                  Execution Result
                                        |
                                  Test Verification
                                        |
                                    QA Report
```

- **Observation:** exposes entities and state; it does not decide whether they constitute a bug.
- **AI decisions/planning:** selects from supported actions within the game's rules and distinguishes hypotheses from requirements.
- **Adapters:** translate generic requests into a game's actual gameplay systems, including target resolution and feedback. Games integrate without adopting the demo's input architecture.
- **Capability Registry:** Stores validated, executable actions and their parameter schemas, target requirements, and execution constraints. Only validated capabilities are exposed to the testing agent. The core engine must not hard-code game-specific actions.
- **Execution:** coordinates dispatch, completion, cancellation, and failure. Game-specific action logic lives in integrations rather than the reusable engine.
- **Tracking:** records status, timing, and reason. Tracking does not independently inspect the game or verify expectations.
- **Verification:** compares observed evidence with explicit expectations, independently of dispatch status.
- **Reporting:** preserves evidence and uncertainty. Debug text alone is not a reproducible QA report.
- **Transport:** Unity and FastAPI share aligned request/response contracts.


## Intended contracts

These describe the shape of the contracts Sentinel is built around.

- **Observation:** each observable entity reports an identifier, a type, a position, and key/value state supplied by game-specific state providers.
- **Capability declaration:** the game side declares which actions are available and what each one needs; the decision backend only chooses from declared actions.
- **Decision:** an action name, an optional target entity ID, and an optional destination. A destination is a coordinate and a target is an entity reference; one is never substituted for the other.
- **Action adapter:** a game-specific implementation of a named action. It is started with a decision, advanced over time, can be cancelled, and reports its own completion or failure with a reason.
- **History and evidence:** each finished action is recorded with its outcome and reason, and the record grows to include the destination, state changes, expectations, and verification results needed for reports.
