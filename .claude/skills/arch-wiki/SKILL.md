---
name: arch-wiki
description: >
  Scans the codebase as observed system reality, compares it with architecture
  documentation and ADRs, reports code-vs-docs drift/conflicts with exact
  locations, synchronizes the living architecture map, and regenerates an
  interactive dashboard with C4 views.
---

# arch-wiki — Living Architecture, Drift Detection & C4 Dashboard

> Framework-agnostic architecture documentation Skill. The codebase is the
> **observed reality**. Architecture docs/ADRs are the recorded design baseline.
> When they disagree, capture and explain the conflict before synchronizing the
> living map from code.

## When to invoke

Use after:

- adding/changing a module, endpoint, middleware, guard, or core service;
- infrastructure, Docker, port, topology, queue, cache, or database changes;
- permission or permission-to-endpoint/page mapping changes;
- SQL/repository/query changes;
- OpenAPI/Swagger changes;
- workspace/package/service changes;
- architecture-significant dependency or boundary changes;
- a full architecture parity/drift audit.

## Operating model

The Skill keeps two views at the same time:

1. **Observed architecture — Code**
   - What currently exists in the implementation.
   - Examples: project/package references, routes, middleware, SQL access,
     permissions, ports, infrastructure, containers and module dependencies.

2. **Recorded architecture — Docs / ADRs**
   - What architecture documentation currently says should exist or what an
     explicit decision says is intended.

`docs/architecture/architecture.json` is the **living observed architecture map**.
It may be synchronized from code. ADRs and explicit architecture decisions are
not silently rewritten just because code drifted.

## Drift classification

Every comparison is one of:

| Status | Meaning |
|---|---|
| `MATCH` | Code and documentation describe the same architecture fact. |
| `CODE_ONLY` | Present in code but absent from documentation. |
| `DOCS_ONLY` | Documented but not observed in code. |
| `CONFLICT` | Both exist but disagree materially. |
| `INTENTIONAL` | Reviewed conflict intentionally remains temporarily. |

### Severity

- `HIGH` — security, cross-module/layer boundary, data ownership, external
  contract, or production topology conflict.
- `MEDIUM` — endpoint, permission, middleware, infrastructure/configuration or
  important component mismatch.
- `LOW` — naming, metadata, descriptions, or low-impact documentation drift.

Always explain why a severity was chosen.

## Smart conflict record

For every conflict store enough evidence to answer:

**Expected → Observed → Where → Why → Impact → Resolution**

Example:

```json
{
  "id": "drift-001",
  "status": "CONFLICT",
  "severity": "HIGH",
  "category": "Cross-module dependency",
  "c4Level": "C3",
  "module": "WorkItems",
  "component": "Application",
  "expected": "WorkItems.Application -> Projects.Contracts",
  "observed": "WorkItems.Application -> Projects.Infrastructure",
  "codeLocations": [
    {
      "path": "src/Modules/WorkItems/.../CreateWorkItemHandler.cs",
      "line": 42,
      "evidence": "Direct dependency on Projects.Infrastructure"
    }
  ],
  "docLocations": [
    {
      "path": "docs/architecture/architecture.json",
      "pointer": "$.c4.components"
    },
    {
      "path": "docs/decisions/0002-cross-module-contracts.md",
      "section": "Decision"
    }
  ],
  "impact": "Bypasses the published module contract and increases coupling.",
  "suggestedActions": [
    "Change code to consume Projects.Contracts.",
    "Or review/update the architecture decision if the change is intentional."
  ]
}
```

Never report only “code differs from docs”.

## C4 model

Use C4 as the standard visualization language:

- **C1 — System Context:** always maintain.
- **C2 — Containers:** maintain whenever deployable processes/data stores are meaningful.
- **C3 — Components:** maintain for each important module/service.
- **C4 — Code:** generate only on demand for one focused component.

Map each drift/conflict to the C4 level where its impact is easiest to understand.
Do not create permanent code-level diagrams for the whole codebase.

## Workflow

### STEP 1 — Snapshot recorded architecture before sync

Read the relevant architecture material first:

- `docs/architecture/architecture.json` if present;
- architecture Markdown;
- relevant ADRs/decisions;
- API/permission/infrastructure documentation when applicable.

Keep this as the **before-sync baseline**.

### STEP 2 — Scan observed code

For an incremental change, scan only relevant paths. For a full audit, inspect:

- workspaces/modules/projects and project/package references;
- routes/controllers/Minimal API groups/endpoints;
- middleware, guards, filters and request pipeline;
- permissions and endpoint/page mappings;
- repositories, SQL queries and table/data ownership;
- integrations and external contracts;
- databases, queues, caches and infrastructure;
- Docker/container topology and ports;
- OpenAPI/Swagger contracts;
- cross-module and cross-layer dependencies.

Use framework-specific conventions when needed, but normalize findings into the
framework-neutral architecture manifest.

### STEP 3 — Compare BEFORE overwrite

Produce:

- match count;
- code-only count/items;
- docs-only count/items;
- conflicts;
- High/Medium/Low counts.

For every conflict include exact code location, documentation/ADR location,
severity and C4 level.

### STEP 4 — Synchronize the living map from code

Update `docs/architecture/architecture.json` from the **observed codebase**.

Rules:

- current architecture inventory follows observed code;
- newly detected conflicts remain under `drift.conflicts`;
- unresolved conflict history is not erased merely because the manifest was synced;
- explicit ADRs are not silently edited to match code;
- update `meta.generatedAt` and `drift.observedAt`.

Recommended manifest sections:

```text
meta
workspaces
infrastructure
dockerDiagram
c4
  context
  containers
  components
modules
permissions
sqlQueries
swaggerSchemas
drift
  overallStatus
  observedAt
  summary
  conflicts
```

### STEP 5 — Generate dashboard

If `docs/architecture/build_html.py` does not exist, copy:

```text
.claude/skills/arch-wiki/templates/build_html.py
```

Then run:

```bash
python docs/architecture/build_html.py
```

For a presentation-only synthetic conflict without changing the manifest:

```bash
python docs/architecture/build_html.py --demo-conflict --output docs/architecture/architecture-conflict-demo.html
```

A synthetic card must be clearly labelled `DEMO CONFLICT`.

### STEP 6 — Verify dashboard UX

The dashboard must contain:

#### Architecture Health

```text
Overall Status: HEALTHY | DRIFT DETECTED | REVIEW REQUIRED
Matches | Conflicts | Code Only | Docs Only
High | Medium | Low
```

#### Architecture Drift

Each conflict shows:

- Severity + category;
- C4 level;
- module/component;
- Expected vs Observed side-by-side;
- Code Location;
- Documentation/ADR Location;
- Why / Impact;
- Suggested Resolution.

#### C4

- C1 System Context;
- C2 Containers when applicable;
- C3 Components;
- C4 Code on demand only.

#### Architecture inventory

Show available modules/endpoints, permissions, SQL, infrastructure/Docker and
OpenAPI metadata.

## Conflict dashboard contract

A high-severity conflict should read approximately like:

```text
Architecture Health
Overall Status: DRIFT DETECTED
Matches: 42 | Conflicts: 3 | Code Only: 2 | Docs Only: 1
High: 1 | Medium: 2 | Low: 3

HIGH CONFLICT — Cross-Module Dependency
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

Code:
src/Modules/WorkItems/.../CreateWorkItemHandler.cs:42

Docs:
docs/architecture/architecture.json
docs/decisions/0002-cross-module-contracts.md

Why / Impact:
Direct dependency bypasses the published module contract and increases coupling.

Suggested resolution:
1. Change code to Projects.Contracts
2. OR review/update the architecture decision if intentional
```

## Full parity & drift audit prompt

```text
Run arch-wiki as a full architecture parity and drift audit.

1. Snapshot architecture.json, architecture docs and ADRs.
2. Scan the actual codebase for modules, dependencies, endpoints,
   middleware, permissions, persistence/SQL, infrastructure, Docker and OpenAPI.
3. Compare code vs docs BEFORE synchronization.
4. Classify MATCH / CODE_ONLY / DOCS_ONLY / CONFLICT.
5. For every conflict report:
   Expected -> Observed -> Where -> Why -> Impact -> Resolution,
   including code location, docs location, severity and C4 level.
6. Synchronize architecture.json from observed code while preserving conflict records.
7. Refresh C1/C2/C3 C4 views.
8. Run python docs/architecture/build_html.py.
9. Report additions, changes and unresolved conflicts.
```

## Guardrails

- This is an architecture visibility/drift Skill, not an enforcement replacement.
  Objective critical boundaries remain in architecture tests/CI.
- Do not silently fix code merely because docs disagree.
- Do not silently rewrite ADRs merely because code drifted.
- Do not invent infrastructure, dependencies or endpoints not observed.
- Keep C4 Code diagrams on demand.
- Keep the Skill optional; normal developer workflow must continue without AI.
