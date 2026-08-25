---
name: plan-change
description: Plan a significant or critical change against the repository Source of Truth before implementation. Use for C/D-risk work or when architecture triggers are present.
---
# Plan Change

1. Read the relevant business rules, architecture docs, ADRs, contracts, code, and existing tests.
2. Classify task risk A-D using the project engineering model.
3. If A/B, say that no formal plan is required and stop unless the user explicitly wants one.
4. If C/D, create or propose `specs/<change>/plan.md` with Goal, Scope, Out of Scope, Source of Truth, affected modules/contracts, implementation steps, verification strategy, risks, and open questions.
5. Identify Architect triggers explicitly.
6. Do not implement until the required approval exists.
