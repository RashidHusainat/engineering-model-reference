# ADR 0001 — Use a Modular Monolith

- Status: Accepted
- Date: 2026-08-25

## Context

The PoC must demonstrate explicit business and technical boundaries in a system complex enough to be credible, without introducing distributed-system infrastructure that is unrelated to the engineering model.

## Decision

Use one deployable ASP.NET Core application containing independently structured Projects and WorkItems modules. Each module has Domain, Application, Contracts (where published), Infrastructure, and tests as applicable.

## Consequences

- Module boundaries are visible and mechanically testable.
- Deployment remains simple.
- Cross-module collaboration needs an explicit contract.
- A future split is possible but is not a goal of this PoC.
