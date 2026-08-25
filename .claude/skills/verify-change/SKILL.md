---
name: verify-change
description: Verify a code change using the repository-owned verification profiles and report concise evidence. Use after implementation or before review.
---
# Verify Change

1. Inspect `git diff --stat` and `git diff` to understand affected areas.
2. Select the lightest profile that provides sufficient evidence:
   - `PreCommit` for very small/local feedback.
   - `PrePush` for normal implementation work.
   - `Pr` for review-ready evidence.
3. Run `./eng/verify.ps1 -Profile <Profile>`.
4. Do not reimplement build/test/architecture checks inside this Skill.
5. Report the profile, result, and any failing engineering rule.
