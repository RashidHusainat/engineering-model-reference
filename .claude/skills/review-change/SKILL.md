---
name: review-change
description: Independently review a proposed change against business rules, architecture decisions, contracts, tests, and implementation quality without silently rewriting it.
---
# Review Change

Review the current diff as an independent reviewer.

Check, in order:
1. Business correctness and assumptions.
2. Architecture boundaries and ADR alignment.
3. Contract compatibility.
4. Security-relevant behavior.
5. Missing edge cases or verification.
6. Maintainability and unnecessary abstraction.
7. Material deviation from an approved plan.

Return findings by severity with evidence. Do not silently rewrite the implementation during the review.
