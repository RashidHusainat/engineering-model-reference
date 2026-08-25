# Path-Based Ownership & Review — Example

The engineering model separates **task risk** (how much review) from **ownership** (who is the appropriate reviewer).

This personal PoC repository does not activate fake CODEOWNERS teams. In a team repository, map paths to real owners using the host platform's policy mechanism.

| Path / artifact | Suggested owner |
|---|---|
| `docs/architecture/**` | Architecture owner |
| `docs/decisions/**` | Architecture owner / decision authority |
| `src/Modules/Projects/**` | Projects module owner |
| `src/Modules/WorkItems/**` | WorkItems module owner |
| `.github/workflows/**`, `azure-pipelines.yml`, `eng/**` | Engineering / DevOps owner |

GitHub implementation: CODEOWNERS + branch protection/rulesets.  
Azure DevOps implementation: Required Reviewers + path filters + branch policy.
