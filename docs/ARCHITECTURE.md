# Sentinel architecture 

## Product scope and decisions

Sentinel is an AI-assisted autonomous gameplay testing platform for Unity developers. Its long-term goal is to understand available mechanics, plan tests, execute gameplay actions, verify outcomes, and produce evidence-based QA reports.

The intended experience is install, authorize inspection, review generated integration, validate, run, and inspect results. AI should reduce manual integration work for supported projects; automatic compatibility with every Unity project is not promised. Unsupported mechanics and uncertainty must be explicit. A narrow Unity-focused MVP takes priority over every genre or engine.

The product goal includes both an installable integration for developers' own worlds and a deployed interactive link where interviewers can try a prepared demo. These are separate delivery surfaces. Browser execution, hosting, deployment infrastructure, and supported Unity configurations are not selected or validated.

## Five-stage product vision — intended, not implemented end to end

### 1. UNDERSTAND

Analyze developer-authorized scripts, scene objects, and runtime information to discover actions and controls, observable entities and states, constraints, and relationships. Output a structured description of capabilities, including evidence and uncertainty. Source inspection alone cannot determine intended gameplay behavior. Local file access is not permission to upload source to external services.

### 2. CONNECT

Generate or configure game-specific adapters against standard Sentinel contracts. Make generated code reviewable before application; avoid changing original gameplay scripts without explicit authorization. Validate interface compatibility, compilation, actual action execution, correct state observation, and intended-to-actual target mapping. Output a validated integration, not merely plausible generated code.

### 3. PLAN

Use available mechanics and validated requirements to propose executable tests. Record objectives, preconditions, relevant entities and actions, expected outcomes, and evidence needed to judge results. Distinguish developer-confirmed requirements from AI-generated hypotheses. Choosing the next exploration action is not a complete test plan.

### 4. PLAY & CHECK

Execute actions and observe resulting gameplay state. Keep three concepts separate:

1. Action execution status: dispatched, running, completed, failed, or blocked as supported by the eventual contract.
2. Actual gameplay state changes, including timing and target identity.
3. Test verification: PASS, FAIL, or INCONCLUSIVE against an explicit expectation.

These are future semantics, not additional enum values already implemented. A movement failure or blocked interaction is not automatically a bug. A locked-door opening can be blocked while the test passes. Missing evidence, uncertain requirements, or invalid setup can make a test inconclusive. The long-term design must support environment reset, recovery, and reproducibility; do not assume those already exist.

### 5. REPORT

Produce objectives and outcomes, expected versus actual behavior, action sequences/history, supporting evidence, reproduction steps, limitations, and inconclusive findings. Distinguish suspected bugs from sufficiently evidenced failures. Never claim a confirmed bug solely because an action failed or an AI suggested one.

## Responsibilities and boundaries

```text
Authorized inspection -> capability description
    -> reviewable generated/configured adapters -> validation -> test plan

Game <-> game-specific adapters <-> reusable Sentinel runtime
                                      | observations / results
                                      v
                                 decision backend
                                      | decisions
                                      v
                       execution -> observation -> verification
                                      -> evidence and reports
```

- **Observation:** expose entities and state; do not decide whether they constitute a bug.
- **AI decisions/planning:** select supported actions and distinguish hypotheses from requirements. Do not bypass game rules.
- **Adapters:** translate generic requests into actual gameplay systems, including target resolution and feedback. External controllers must not adopt the demo input architecture merely to integrate.
- **Execution:** coordinate dispatch, completion, cancellation, and failure. Game-specific action branches belong in integrations rather than the reusable engine.
- **Tracking:** record status, timing, and reason. Tracking does not independently inspect the game or verify expectations.
- **Verification:** compare observed evidence with explicit expectations independently of dispatch status.
- **Reporting:** preserve evidence and uncertainty. Debug text alone is not a reproducible QA report.
- **Transport:** align Unity and FastAPI request/response contracts. Changes need approval and coordinated migration; preserve compatibility where feasible.

Reuse working components incrementally. Do not introduce dependencies or replace the architecture to resemble a multi-agent diagram. MCP and separate specialized agents are optional future choices, not current requirements.

## Current repository and implementation status

No project README was found in the inspected source/document inventory. `backend/` contains FastAPI; `unity-client/` is a full Unity project with sample gameplay and Sentinel scripts; `docs/devlog/` records historical progress. Historical entries do not guarantee current behavior. `docs/roadmap.md` is deleted in the working tree; this task does not restore it.

### Current execution flow

```text
SentinelUIController -> SessionManager.StartSession
  -> ControlMode selects AI input
  -> WorldObservationManager gathers ObservableEntity snapshots
       -> IObservableStateProvider implementations, e.g. DoorStateProvider
  -> ApiClients sends observations + history to POST /action
       -> backend/routes/action.py -> service/openai_service.py
       -> structured ActionDecision -> validated ActionResponse
  -> AIIntent reads the response
       MOVE: CommonIntent -> PlayerMovement; AIIntent checks progress
       INTERACT: PlayerInteraction.TryInteract -> current IInteractable.Interact
  -> ActionTracker stores completion/failure
  -> SessionManager records ActionHistory and repeats
```

Paths below are relative to `unity-client/Assets/Scripts/` unless prefixed with `backend/`.

| Component | Present in code | Limits / status |
| --- | --- | --- |
| `Observation/ObservableEntity.cs`, `ObservationRegistry.cs`, `WorldObservationManager.cs` | Registration, position/type/state snapshots using `Core/Contracts/IObservableStateProvider.cs` | Instrumented objects only; no automatic mechanics discovery. IDs use runtime object identity; cross-run identity is not established. |
| `StateProvider/DoorStateProvider.cs` | Exposes locked/open states | Existing observation adapter tied to demo `AnimatedDoor`. |
| `API/ApiClients.cs`, `ApiModels.cs` | JSON observations/history, HTTP POST, response decoding | Hardcoded localhost endpoint; no configurable public service integration established. |
| `backend/main.py`, `routes/action.py`, `models/schemas.py` | Action endpoint, models, missing-decision/destination and target-existence checks | Fixed action enum; target existence does not prove reachability or correct mapping. |
| `backend/service/openai_service.py` | Sends observations/history to LLM, parses decision | Fixed prompt/model; no source inspection, adapter generation, capability negotiation, or persisted test plan. Loads a local environment file; do not expose it. |
| `Intent/AIIntent.cs` | Explicit move/interact branches | Demo dependencies; no extensible executor registry or generated-adapter contract. |
| `Core/ActionTracker.cs` | Idle/Running/Succeeded/Failed, timing, reason, last result | Updated by callers; not a verifier or persistent report store. |
| `Core/SessionManager.cs` | Observe/send/wait/record loop, in-memory history | History clears on start; no complete reset/recovery/cancellation or durable replay system. |
| `Player/PlayerInteraction.cs` | TryInteract reports delivery/no target; human and AI share it | Success is not proof of effect. Recent patch is not runtime-verified by this audit. |
| `Intent/CommonIntent.cs`, `HumanIntent.cs`, `Player/`, `Interaction/` | Sample controller, player mechanics, doors | Preserve as sample integration, not mandatory external controller architecture. |
| `SentinelUI/` | Session controls, debug status and graph | Not a web product or evidence-based QA reporting system. |
| `Observation/WorldObservationTest.cs` | Logs observations at startup | Diagnostic, not assertion-based automated tests. No project-owned automated suite found in the inspected source inventory. |

### Current API semantics

- Request: observations and history. Entities have id, type, position, and string key/value state entries.
- LLM decision: action, optional-valued target_id, and destination. The route maps destination to response position; C# consumes position.
- MOVE destination is a coordinate independent of optional target entity ID. Do not silently replace it with the target transform.
- History contains action, target ID when supplied, succeeded/failed, and optional reason. C# enums use camel-case string serialization.
- History does not contain destinations, before/after snapshots, expectations, or verification outcomes. Normal movement initialization omits target ID, losing that association.

### Known gaps and contradictions

- Python allows move/sprint/crouch/interact/jump; the prompt and AIIntent support move/interact. Unsupported decisions lack an executor completion path. Schema, prompt, and available executors must eventually agree.
- Movement uses horizontal distance and direct steering with stuck detection, not path planning or proof of gameplay bugs.
- Interaction records the requested target ID but chooses an object by proximity/raycast, without resolving that ID. Do not claim verified target correspondence.
- Interaction success means Interact returned without exception. Locked doors can return without opening. AnimatedDoor changes logical state before animation finishes, so logical open does not prove physical completion.
- Missing responses break the session loop without StopSession, potentially leaving running/UI state inconsistent. Network cancellation, restart races, and recovery are not established as reliable.
- AIIntent directly calling PlayerInteraction is demo coupling, not the intended SDK boundary. Preserve behavior when extracting an approved adapter; do not label the current patch as extensibility.
- A door-specific reset exists, but complete environment reset, reproducible replay, persistent evidence, PASS/FAIL/INCONCLUSIVE verification, and QA reports were not found.
- No installable Sentinel SDK release, generated-integration workflow, or hosted interactive product was found. Prompts and diagrams are not implementation evidence.

## Intended SDK direction — proposals requiring approved implementation

Retain observations, provider concepts, transport, tracking, and orchestration where useful. Extract demo movement/interaction into game-specific integrations rather than adding branches for every external action.

An extensible action contract/registry is proposed: integrations describe names, parameters, targets, availability, and execution/results. The AI selects validated capabilities. Categories can provide setup templates; capabilities describe shared behaviors across object types. Exact interface names and schemas are not finalized.

A future AI setup assistant inspects authorized information and generates adapter implementations for review. Validate real gameplay behavior and constraints, not just compilation. Prove portability in another project before advertising compatibility. Automatic inspection and generated integration are long-term capabilities, not current features.

## Historical decisions preserved and reconciled

The previous architecture diagram proposed Unity observations -> HTTP/FastAPI agent -> actions -> Unity execution -> logs -> database -> bug detector -> bug report. Its nine milestones were: backend boundary; Unity/backend connection; test environment; deterministic agent; AI agent; telemetry/database; bug detection; AI bug analysis; generalize beyond Unity.

This document replaces that numbering with the owner's five conceptual stages. It preserves the transport and evidence/reporting direction while marking storage/detection as future work. A deterministic policy remains a possible validation aid, not an implemented subsystem. Beyond-Unity generalization is deferred for the Unity MVP. Existing devlogs remain unchanged. The lowercase architecture filename is normalized to the requested canonical `docs/ARCHITECTURE.md`; maintain one document.

## Owner review before implementation

- Coding-agent instructions live in `CLAUDE.md` (renamed from `AGENTS.md`), which `.gitignore` excludes, so the instructions persist locally but do not travel with a clone. Git still records the architecture path in lowercase despite the on-disk casing change on Windows; record the case-only rename when preparing an approved commit.
- Select supported Unity versions/mechanics and the independent project for integration validation.
- Approve the action contract/registry and coordinated C#/Python changes; this documentation task does not authorize them.
- Define expected behaviors, evidence standards, completion semantics, and reset requirements for the first test.
- Select hosted-demo execution and deployment separately from the SDK.
- Define source-upload authorization and generated-code review/application. Low-effort setup does not remove these boundaries.

This document records direction and gaps, not permission to implement the five stages.
