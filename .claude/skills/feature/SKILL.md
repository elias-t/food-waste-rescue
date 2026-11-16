---
name: feature
description: Spec-first workflow for building a feature or non-trivial fix end to end - spec, plan, implement, verify, review. Use when asked to build, add or implement anything bigger than a one-line change, or when invoked as /feature.
---

# Feature workflow

Follow these phases in order. Do not skip the spec for anything that touches more than ~2 files.

## 1. Spec
- If the request is a GitHub issue, read it: `gh issue view <number> --comments`.
- Write `docs/specs/<yyyy-mm-dd>-<short-name>.md` containing:
  - **Goal** (1-3 sentences) and **non-goals**
  - **Acceptance criteria** as a checklist that can be verified by a test or a command
  - **Design**: affected layers/files, API or data-model changes, migrations
  - **Risks / open questions**
- In an interactive session: stop and ask the user to approve the spec. In a headless/CI run: proceed, and list your assumptions in the spec.

## 2. Plan
- Break the work into small steps, each ending in a state where build + tests pass.
- Write the tests for each acceptance criterion first where practical.

## 3. Implement
- One step at a time. Follow CLAUDE.md conventions. Keep the diff focused on the spec.
- Hooks will lint/format each edit and run build + tests when you stop; treat their output as mandatory to fix.

## 4. Verify
- Run the full check commands listed in CLAUDE.md yourself and confirm they pass.
- Tick each acceptance criterion in the spec and note how it was verified.

## 5. Review
- Run the `reviewer` subagent on the diff. Fix every CHANGES REQUIRED finding, then re-run it.

## 6. Hand off
- Commit on a branch named `feat/<short-name>` (or `fix/...`) with a clear message. Never commit to `main`, never push unless asked (in CI, the workflow handles pushing).
- Summarise: what changed, how it was verified, anything left open.
- If you hit a mistake a rule could have prevented, propose one line to add to CLAUDE.md.
