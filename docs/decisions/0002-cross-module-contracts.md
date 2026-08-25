# ADR 0002 — Cross-Module Collaboration Through Published Contracts

- Status: Accepted
- Date: 2026-08-25

## Context

The WorkItems module needs to know whether a Project exists and is active. Direct access to Projects domain objects, repositories, infrastructure, or tables would couple module internals.

## Decision

Projects publishes `EngineeringModel.Modules.Projects.Contracts`. WorkItems.Application may depend on this contract and nothing else from Projects.

The concrete implementation is provided by Projects.Infrastructure and wired by the API composition root.

## Executable rule

Architecture tests reject references from WorkItems to Projects.Domain, Projects.Application, or Projects.Infrastructure.
