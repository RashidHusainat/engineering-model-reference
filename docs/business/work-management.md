# Work Management — Business Source of Truth

This small domain exists only to demonstrate the engineering model with real behavior. It is intentionally simpler than a production work-management product.

## Concepts

### Project
A project groups work items.

States:
- `Draft`
- `Active`
- `Closed`

Rules:
1. A project name is required and cannot exceed 120 characters.
2. A new project starts in `Draft`.
3. Only a `Draft` project can be activated.
4. Only an `Active` project can be closed.

### Work Item
A work item belongs to exactly one project.

States:
- `Open`
- `Completed`

Rules:
1. A work-item title is required and cannot exceed 200 characters.
2. A work item can be created only for an **Active** project.
3. A new work item starts in `Open`.
4. A completed work item cannot be completed again.

## Cross-module business rule

`WorkItems` may ask `Projects` whether a project exists and is active, but it must do so through the published `Projects.Contracts` contract. It must not reach into the Projects domain or database directly.
