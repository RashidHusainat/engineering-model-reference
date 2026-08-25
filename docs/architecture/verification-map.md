# Verification Map

This file connects important truth to executable evidence. It is intentionally short.

| Source-of-Truth decision | Executable evidence | Stage |
|---|---|---|
| Project name and lifecycle rules | Projects unit tests | PrePush / PR |
| Work-item title and completion rules | WorkItems unit tests | PrePush / PR |
| Work item requires an active project | Application unit test + API integration test | PrePush / PR |
| Domain must not depend on Infrastructure | NetArchTest architecture tests | PrePush / PR |
| WorkItems may consume Projects only through Contracts | Architecture assembly-reference test | PrePush / PR |
| HTTP composition works with real SQLite boundary | API integration test | PR / Main |

CI is authoritative. Local Git hooks and Claude Code call the same `eng/verify.ps1` implementation for fast feedback.
