# Software Architect Review Brief

## Review objective

Challenge whether this repository demonstrates a credible, lightweight engineering system rather than a collection of tools.

## Five claims to validate

1. **Normal engineering works without AI.** A developer can read Source of Truth, implement, run `eng/verify.ps1`, push, and rely on CI.
2. **Architecture intent is explicit before enforcement.** ADRs and architecture docs define intent; NetArchTest verifies selected objective rules.
3. **Module boundaries are real.** WorkItems collaborates with Projects only through Projects.Contracts.
4. **Verification has one owner.** Developer CLI, Git hook, Claude Code, GitHub Actions, and Azure Pipelines route to the same repository-owned script.
5. **Claude is thin.** CLAUDE.md and Skills route context and procedures; they do not duplicate business truth, architecture truth, or CI enforcement.

## Deliberate trade-offs

- Two modules instead of one so cross-module boundaries are testable.
- One SQLite file instead of external infrastructure so integration tests remain real but portable.
- Direct ADO.NET-style persistence instead of an ORM so persistence is visible and the PoC does not become an ORM demonstration.
- No MediatR/CQRS framework, event bus, Saga, Docker stack, or multi-agent orchestration because none is required to validate this model.
- No Stryker/FsCheck/Pact/NDepend in the first baseline; those remain risk-based maturity tools.

## Questions worth asking

- Which architecture rules are objective and stable enough to deserve mechanical enforcement?
- Is Projects.Contracts the right published surface, or should a different integration style be used for the target product?
- Which PR paths should require specialized owners in the real organization?
- Which verification profile is fast enough for pre-push in the real repository?
- Which C/D changes require human Architect approval, and can those triggers stay narrow?
- What would we remove before adding another tool?

## Expected outcome

If the core loop survives review and execution, promote this repository into a reusable starter template. Organization-specific security, CI, and ownership policies should be layered on only after the core stays understandable and independently executable.
