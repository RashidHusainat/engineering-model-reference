# Claude Code — Project Operating Context

This repository is a reference implementation of the **Engineering Knowledge & Verification Model**.
Claude Code is an accelerator inside the engineering system, not a separate source of truth or an enforcement authority.

## Start here

Before changing code, read only the context relevant to the task:

- Business rules: `docs/business/work-management.md`
- Architecture: `docs/architecture/overview.md`
- Living architecture / drift dashboard: `docs/architecture/ARCH-WIKI.md`
- Verification map: `docs/architecture/verification-map.md`
- Decisions: `docs/decisions/`
- Full engineering model: `docs/engineering-model/Engineering-Knowledge-Verification-Model.md`

## Non-negotiable boundaries

- The intended architecture comes from approved architecture documentation and ADRs, **not from existing code**.
- Module internals are private. Cross-module interaction goes through the other module's `Contracts` assembly only.
- Domain assemblies must not depend on Application, Infrastructure, API, or another module.
- Application assemblies must not depend on Infrastructure or API.
- Do not introduce architecture, frameworks, patterns, or infrastructure that the task does not require.
- Preserve implementation freedom inside explicit business and technical boundaries.

## Task risk

- A/B: implement directly after reading relevant context; keep process light.
- C: create `specs/<change>/plan.md`, obtain developer approval, involve Architect only if a trigger applies.
- D: plan and human Architect approval are mandatory before implementation.

Architect triggers include cross-module design, a new integration boundary, reusable infrastructure, an important data-contract decision, security-sensitive architecture, or a change to architecture strategy.

## Verification

Use the repository-owned verification entry point. Never create a Claude-only verification path.

```powershell
./eng/verify.ps1 -Profile PrePush
```

Before claiming a change is complete, run the profile appropriate to the change. CI remains authoritative.

## Optional architecture visibility — arch-wiki

`arch-wiki` may scan the codebase as **observed reality** and synchronize the living
`docs/architecture/architecture.json` inventory. This does not mean accidental code
becomes the intended architecture.

When code disagrees with architecture docs or ADRs:

1. capture the conflict before synchronization;
2. show Expected vs Observed;
3. include exact code and documentation locations;
4. explain Why / Impact / Resolution;
5. map the conflict to C4 (C1/C2/C3; C4 Code only on demand);
6. preserve unresolved drift even after the living manifest is refreshed from code.

Architecture tests and CI remain the deterministic enforcement path.

## Change discipline

- Do not push, merge, change branch protection, or use credentials unless explicitly asked.
- Report material deviation from an approved plan before proceeding.
- If a rule is objective and critical, prefer an executable engineering mechanism over another prompt instruction.
