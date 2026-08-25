# arch-wiki — Living Architecture & Drift Dashboard

`arch-wiki` is an optional Claude Code Skill that keeps a living architecture
map close to the observed codebase while making **code-vs-docs drift visible
instead of silently hiding it**.

## Why it exists

The repository already has deterministic architecture tests for rules that must
fail mechanically. `arch-wiki` solves a different problem:

- inventory what the codebase currently contains;
- compare the observed state with architecture documentation and ADRs;
- show conflicts with exact evidence;
- synchronize the living architecture manifest from code;
- preserve unresolved conflict records for review;
- visualize the result with C4.

It does **not** replace NetArchTest or CI.

## Conflict-first synchronization

```text
Observed Code
     │
     ├──────────────┐
     │              │
     ▼              ▼
Compare          Recorded Docs / ADRs
     │
     ▼
MATCH | CODE_ONLY | DOCS_ONLY | CONFLICT
     │
     ▼
Capture exact conflict evidence
     │
     ▼
Synchronize architecture.json from code
     │
     ▼
Preserve unresolved drift in dashboard
```

The important behavior is that synchronization does not erase the evidence that
a conflict existed.

## Dashboard shape

```text
┌───────────────────────────────────────────────────────────────┐
│ Architecture Health                                          │
├───────────────────────────────────────────────────────────────┤
│ Overall Status: ⚠ DRIFT DETECTED                             │
│ Matches: 42   Conflicts: 3   Code Only: 2   Docs Only: 1    │
│ High: 1      Medium: 2       Low: 3                          │
└───────────────────────────────────────────────────────────────┘
```

A conflict card follows this contract:

```text
🔴 HIGH CONFLICT — Cross-Module Dependency
C4 Level: C3 — Component
Module: WorkItems / Application

EXPECTED
WorkItems.Application
    ↓
Projects.Contracts

OBSERVED IN CODE
WorkItems.Application
    ↓
Projects.Infrastructure

Code Location
src/Modules/WorkItems/.../CreateWorkItemHandler.cs:42

Documentation
- docs/architecture/architecture.json
- docs/decisions/0002-cross-module-contracts.md

Why / Impact
Direct dependency bypasses the published module contract and increases coupling.

Suggested Resolution
1. Change code to Projects.Contracts
2. OR review/update the architecture decision if intentional
```

The UX rule is simple:

**Expected → Observed → Where → Why → Impact → Resolution**

## C4 policy

- **C1 System Context** — always.
- **C2 Containers** — when meaningful.
- **C3 Components** — for important modules/services.
- **C4 Code** — on demand only.

Each drift item is mapped to the C4 level where its impact is easiest to
understand.

## Current reference manifest

`architecture.json` contains the current observed reference architecture for the
Projects and WorkItems PoC, including C1/C2/C3 views.

Generate the dashboard:

```bash
python docs/architecture/build_html.py
```

Generate a presentation-only conflict example without corrupting the live
manifest:

```bash
python docs/architecture/build_html.py \
  --demo-conflict \
  --output docs/architecture/architecture-conflict-demo.html
```

The demo card is explicitly labelled `DEMO CONFLICT`.

## Relationship to the Engineering Model

`arch-wiki` remains an **optional visibility layer**.

- Code provides observed architecture facts.
- `architecture.json` is the living observed map.
- ADRs/architecture decisions preserve intended decisions and are not silently
  rewritten because code drifted.
- Architecture tests/CI remain authoritative for deterministic boundaries.
- Normal developer workflow remains first-class without Claude or arch-wiki.
