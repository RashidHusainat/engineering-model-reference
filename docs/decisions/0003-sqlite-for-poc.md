# ADR 0003 — Use SQLite with Direct ADO.NET Access for the PoC

- Status: Accepted
- Date: 2026-08-25

## Context

The PoC needs a real persistence boundary for integration testing, while remaining self-contained and easy to run on a developer machine or CI agent.

## Decision

Use `Microsoft.Data.Sqlite` directly. No ORM, database server, container, or external service is required.

## Consequences

- Integration tests exercise real SQL and I/O with minimal setup.
- Persistence code stays explicit.
- This is a PoC implementation choice, not a rule that future templates must avoid an ORM or production database.
