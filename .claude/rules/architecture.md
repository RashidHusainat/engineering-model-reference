---
paths:
  - "src/**/*.cs"
  - "src/**/*.csproj"
---
# Architecture Rules

- Treat `docs/architecture/` and accepted ADRs as intended architecture.
- Never infer intended architecture from accidental dependencies in the current implementation.
- A module may reference another module only through that module's `Contracts` project.
- Domain must remain independent of Application, Infrastructure, API, and other modules.
- Application must remain independent of Infrastructure and API.
- Architecture rules that are already executable must be verified through `tests/EngineeringModel.ArchitectureTests`.
