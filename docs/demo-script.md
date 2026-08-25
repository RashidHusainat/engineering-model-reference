# Demo Script — 5 Minutes

## 1. Show trusted context

Open:
- `docs/business/work-management.md`
- `docs/architecture/overview.md`
- `docs/decisions/0002-cross-module-contracts.md`

Explain: the decision exists before the test; the test does not invent the architecture.

## 2. Show executable architecture

Open:
- `tests/EngineeringModel.ArchitectureTests/LayerDependencyTests.cs`
- `tests/EngineeringModel.ArchitectureTests/ModuleBoundaryTests.cs`

## 3. Show shared verification

Run:

```powershell
./eng/verify.ps1 -Profile PrePush
```

Then show that `.githooks/pre-push`, GitHub Actions, Azure Pipelines, and CLAUDE.md all route into that same script.

## 4. Break architecture on purpose

Run:

```powershell
./eng/demo-architecture-violation.ps1
```

The temporary Domain → Infrastructure dependency should be rejected. The script restores the files in a `finally` block.

Then run:

```powershell
./eng/verify.ps1 -Profile PrePush
```

Expected: green again.

## 5. Show AI remains optional

Open `CLAUDE.md` and `.claude/skills/verify-change/SKILL.md`.
Explain: Claude learns where truth lives and which shared command to call. It does not own the rule and it does not own the CI gate.
