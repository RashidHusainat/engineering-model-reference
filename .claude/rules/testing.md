---
paths:
  - "src/**/*.cs"
  - "tests/**/*.cs"
---
# Testing Rules

- Test business behavior and engineering boundaries, not implementation trivia.
- Prefer the smallest test type that proves the risk: unit for isolated behavior, integration for real boundaries, architecture tests for structural rules.
- Do not add tests only to increase coverage.
- Use the shared verification entry point; Skills and Claude sessions must not duplicate CI logic.
