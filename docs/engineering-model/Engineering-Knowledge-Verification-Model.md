# Engineering Knowledge & Verification Model
### A lightweight, risk-based engineering system — with Claude Code as a thin accelerator, not a parallel platform

## 1. Problem

As systems and teams grow, important business and technical knowledge becomes scattered across people, tickets, documents, code, tests, configuration, and review habits. This creates inconsistent implementation and verification quality. AI-assisted development makes the same problem more visible: an AI can write technically plausible code while missing an unwritten business rule, contract, or architecture boundary.

The problem is therefore engineering discoverability and verification first. Claude Code is one participant that benefits from solving it, not the reason the system exists.

## 2. Goal

Build a lightweight engineering environment where developers and Claude Code use the same trusted context, important requirements and constraints are verified through appropriate engineering mechanisms, objective critical rules are enforced consistently, and implementation remains flexible everywhere else.

### Non-goals

- No custom replacement for Claude Code.
- No mandatory multi-agent workflow.
- No formal plan for trivial work.
- No mandatory TDD/BDD for every change.
- No attempt to convert every preference into a CI gate.
- No requirement to install every testing or analysis tool.
- AI is never the Source of Truth.
- Do not duplicate one rule across documentation, prompts, Skills, analyzers, and tests.

## 3. Core principles

1. **Normal engineering first.** The repository must remain correct and usable without Claude Code.
2. **Business defines what is correct; technical governance defines the boundaries.**
3. **Process scales with risk.**
4. **Make important truths executable where the value justifies the maintenance cost.**
5. **Deterministic rules live in engineering mechanisms, not prompts alone.**
6. **Implementation freedom is the default.**
7. **Claude Code is an accelerator inside the engineering system, not a parallel platform.**

## 4. Core engineering loop

```text
Business + Technical Source of Truth
                ↓
Executable Specs / Policies where valuable
                ↓
          Implementation
                ↓
     Verification & Enforcement
                ↓
               CI
                ↓
     Risk-Appropriate Review
                ↓
        Merge / Production
                ↓
             Feedback
                └──────────→ Source of Truth
```

Feedback updates the engineering asset that should have prevented the gap: a missing business rule updates business context; an architecture decision becomes an ADR; a regression becomes a test; a recurring forbidden dependency becomes an architecture test or analyzer; an integration gap becomes a contract test.

## 5. Source of Truth

Keep business and technical knowledge distinct because they change for different reasons.

### Business

- context and glossary
- requirements and business rules
- workflows
- actors and permissions
- decision tables
- important business contracts

### Technical

- architecture and engineering strategies
- ADRs
- API / event contracts
- module documentation
- engineering conventions
- validation, data-access, exception, logging, testing, API, security, and caching strategies where an actual decision exists

### Scope

```text
Organization → Repository → Module / Domain → Feature / Change
```

Do not require a document at every scope. A more specific scope may refine implementation detail but must not contradict higher-level security, compliance, or architecture strategy.

Executable artifacts can be the Source of Truth for facts they define better than prose—for example OpenAPI, analyzer configuration, or a contract test. Existing implementation code is evidence of current behavior, not automatically authority for intended business or architecture design.

## 6. Executable specifications and policies

| Intent | Executable mechanism |
|---|---|
| Business behavior | Unit / BDD tests |
| Business invariant | Property tests such as FsCheck |
| Architecture decision | NetArchTest / ArchUnitNET |
| API contract | OpenAPI / Pact / integration verification |
| Security policy | Analyzer / scanner / security test |

Existing compiler, analyzer, architecture, security, and contract checks stay active even when a task does not add a new executable specification.

Three useful engineering levels:

```text
Architecture → structural boundaries → Architecture Tests
Design       → contracts/invariants  → Contract/Integration Tests
Behavior     → business behavior     → Unit/BDD/Property Tests
```

This is a lens, not another workflow.

## 7. Implementation freedom and task risk

The allowed solution space is:

```text
Business Correctness + Critical Technical Boundaries
                       ↓
              Allowed Solution Space
                       ↓
       Developer / Claude implementation choice
                       ↓
                  Verification
```

Objective, critical, mechanically testable rules are candidates for deterministic enforcement. Important subjective decisions belong in review. Preferences belong in documentation/examples unless there is a stronger reason.

### Risk model

| Risk | Typical flow |
|---|---|
| **A — Trivial** | Implement → fast verification |
| **B — Normal** | Understand context → implement → tests → review |
| **C — Significant** | Plan → developer approval → Architect if triggered → implement → strong verification → independent review |
| **D — Critical** | Plan → human Architect mandatory → approval → controlled implementation → deep verification → CI gates → independent + human review |

Architect triggers include cross-module design, new integration boundaries, reusable infrastructure, important data-contract decisions, security architecture, new platform capabilities, or significant changes to architecture strategy.

TDD, BDD, FsCheck, Pact, mutation testing, and subagents are capabilities selected by risk—not mandatory stages.

## 8. Testing maturity toolbox

- **Unit tests:** core for isolated behavior.
- **Integration tests:** core for real boundaries.
- **Architecture tests:** core when objective architecture boundaries exist and are worth protecting mechanically.
- **BDD / characterization / golden tests / FsCheck / Pact:** risk-based.
- **Stryker.NET:** targeted or scheduled when test-quality risk justifies it, not every PR by default.
- **NDepend:** optional periodic architecture, dependency, complexity, and maintainability analysis.

A green suite and high coverage do not guarantee useful verification; tests must be capable of detecting meaningful regressions.

## 9. Deterministic enforcement and CI

```text
Engineering Decision
        ↓
Best Enforcement Mechanism
        ↓
Fast Local Feedback
        ↓
CI — authoritative shared gate
```

Architecture tools verify **explicit decisions**; they do not discover the team's intended architecture from existing code.

Example:

```text
Decision: Domain must not depend on Infrastructure
        ↓
Executable architecture test
        ↓
NetArchTest inspects actual assemblies
        ↓
PASS / FAIL
```

For legacy systems, new rules should not force a big-bang rewrite: detect current violations, baseline them where necessary, block new violations, and remove existing debt incrementally.

### Claude Hooks vs Git Hooks vs CI

- Claude Hooks: Claude Code lifecycle convenience.
- Git Hooks: local developer feedback; bypassable.
- CI: shared authoritative enforcement.

Critical guarantees must never exist only in a Claude Hook or Git Hook.

### Shared Verification Entry Point

```text
Developer CLI ─┐
Git Hook ──────┤
Claude Code ───┼──> eng/verify --<profile>
CI ────────────┘
```

There is one repository-owned implementation of verification. Callers do not maintain separate copies of build/test/architecture logic.

Suggested staging:

- **Pre-commit:** formatting, compiler/analyzer checks that are fast enough.
- **Pre-push:** build, unit tests, architecture tests, fast contract checks.
- **PR CI:** build, analyzers, architecture tests, unit tests, fast contracts, secret scanning.
- **Main/integration:** integration, deeper contract/security checks.
- **Scheduled/deep:** mutation testing, NDepend, expensive security analysis.

## 10. Roles and review

- **Human Architect:** final authority for significant cross-cutting architecture decisions; not required for routine implementation.
- **Claude Architect/Planner:** explores, challenges, and proposes; no final authority and does not implement while acting in this role.
- **Developer:** owns judgment, approvals, and implementation responsibility.
- **Claude Implementer:** executes A/B tasks directly or approved C/D plans inside the allowed solution space; reports material deviation rather than silently redesigning.
- **Independent Reviewer:** challenges correctness, architecture, security, contracts, tests, assumptions, maintainability, and overengineering.

For C/D work, Author ≠ Reviewer. Review depth is determined by task risk; reviewer identity is determined by path/artifact ownership. Repository policy—not Claude—assigns required reviewers and enforces merge rules.

## 11. Thin Claude Code layer

Use native capability first. Claude consumes the same Source of Truth, code, tests, contracts, and verification commands as developers.

- `CLAUDE.md`: context, routing, operating boundaries—not enforcement.
- `.claude/rules`: scoped instructions.
- Skills: repeatable procedures that call existing engineering mechanisms instead of reimplementing them.
- Subagents: optional role isolation when worthwhile.
- Hooks: local Claude workflow convenience only.
- MCP: external-system integration when required.
- Auto Memory: operational memory, not Source of Truth; promote only genuinely reusable lessons.

Third-party productivity layers such as Superpowers remain optional and must never become architecture, governance, or correctness dependencies.

## 12. Actual working paths

### Normal developer

```text
Task → Relevant Source of Truth → Implement → eng/verify → Git/CI → Review
```

### Claude-assisted developer

```text
Task → Claude reads same Source of Truth → Implement → same eng/verify → Git/CI → Review
```

### Significant / critical change

```text
Task → Risk decision → Plan → Approval → Architect when required
     → Implementation → Strong verification → CI → Independent review → Merge
```

## 13. Main harness

```text
DEVELOPER / TEAM
      ↓
TASK / INTENT
      ↓
RELEVANT SOURCE OF TRUTH
      ↓
TASK-RISK DECISION A–D
      ↓
A/B: direct path                C/D: Plan → Approval
                                      ↓
                           C: Architect if triggered
                           D: Architect mandatory
                    \               /
                     IMPLEMENTATION
                  Developer | Claude
                           ↓
          Optional risk-based capabilities
                           ↓
            SHARED VERIFICATION ENTRY POINT
      Developer · Claude · Git Hooks · CI callers
                           ↓
 Unit / Integration / Contract / Architecture / Analyzers
                           ↓
                      CI authoritative
                           ↓
               Risk-Appropriate Review
                           ↓
                  Merge → Production
                           ↓
                       Feedback
                           └────→ Source of Truth
```

## 14. Maturity

Assess repository maturity independently across:

- Context & Documentation
- CI & Constraints
- Testing Depth
- Review Practice

Task Risk (one change), Repository Maturity (system over time), and Claude Autonomy (amount delegated) are separate axes. Higher maturity does not imply more AI autonomy.

## 15. Design check for future additions

Before adding a component, ask:

1. What real engineering problem does it solve?
2. Does Claude Code already provide it natively?
3. Can an existing engineering mechanism solve it more reliably?
4. Is it mandatory, risk-based, optional, or unnecessary?
5. Does it unnecessarily reduce implementation freedom?
6. What maintenance, context, or CI cost does it add?
7. Does it duplicate another Source of Truth?
8. Would the engineering model still make sense without Claude Code?

Classify additions as **KEEP / SIMPLIFY / OPTIONAL / DEFER / REMOVE**.

---

**Outcome:** a normal engineering system with discoverable truth, executable critical boundaries, one shared verification path, risk-appropriate review, and Claude Code as an optional accelerator inside the same system.
