# BuildersPSE2 - Claude Context

## What This Project Is
A VB.NET Windows Forms application (.NET Framework 4.8) backed by SQL Server.
It is a **Pre-Sale Estimating (PSE) tool** for the lumber/construction industry.
Builders use it to create estimates, track project versions, compare actuals vs. estimates, and manage lumber price locks.

## Tech Stack
- **Language:** VB.NET, `Option Strict On` required on all files
- **Framework:** .NET Framework 4.8 (not .NET Core / .NET 5+)
- **UI:** Windows Forms
- **Database:** SQL Server — server `bwcosql01\cosprod`, database `BuildersPSE`, Windows Authentication
- **Reports:** RDLC (Microsoft Report Viewer) — `ProjectSummary.rdlc`, `InclusionsExclusions.rdlc`
- **External integrations:** Monday.com (API key stored encrypted in App.config), BisTrack ERP

## Project Structure
```
Models/          - POCO model classes (no logic)
DataAccess/      - SQL Server DA classes + Queries.vb (SQL constants) + TableOperations.vb
Forms/           - Windows Forms; Forms/PriceLock/ for price lock screens
Services/        - Business logic (VarianceGrid, RollupCalc, PriceHistory, Futures)
Utilities/       - SqlConnectionManager, SecurityHelper, MondaycomAccess, SpreadsheetParser
Controls/        - Custom WinForms controls
```

## Architecture Rules
- **`TableOperations.vb`** is the single source of truth for all INSERT/UPDATE parameter building. Schema changes go here only — never duplicate parameter lists in other DA files.
- **`Queries.vb`** holds all SQL query strings as `Public Const` values, organized by `#Region` per entity. All queries must be parameterized (`@ParamName`) — no string concatenation in SQL.
- **`SqlConnectionManager`** singleton handles all connections. Use `SqlConnectionManager.Instance` — never create ad-hoc `SqlConnection` objects outside of DA classes.
- Services call DataAccess; Forms call Services or DataAccess. Services do not call Forms.
- Models are plain data containers — no DB calls, no UI logic.

## Coding Conventions
- `Option Strict On` at the top of every file
- XML doc comments (`''' <summary>`) on all Public methods and classes
- `#Region` blocks to organize long files by feature area
- Nullable types for optional DB fields: `Decimal?`, `Integer?`, `Date?`
- Use `ToDb(value)` from `TableOperations` to convert nullables to `DBNull.Value`
- Use `GetDecimal()`, `GetString()`, etc. from `TableOperations` when reading DataRows
- Form naming: `frm` prefix (e.g. `frmCreateEditProject`)
- Model naming: `[Entity]Model` suffix (e.g. `ProjectModel`, `LevelModel`)
- DA class naming: `[Entity]DataAccess` suffix
- Namespace: `BuildersPSE2.DataAccess`, `BuildersPSE2.Utilities`, `BuildersPSE2.BuildersPSE.Models`

## Git Workflow
- **Main branch:** `main`
- **Working branches:** named by feature or bug batch (e.g. `Bug-fixes-February`)
- **Remote:** GitHub — `https://github.com/soup1977/BuildersPSE2`
- PRs merge into `main`
- Do not commit `App.config` changes with real connection strings or API tokens

## Namespace Consistency
- The correct model namespace is `BuildersPSE2.BuildersPSE.Models` — but inconsistencies exist in the codebase.
- When writing new code, use the correct namespace. When an inconsistency is spotted while working on a file, fix it in that file and call it out to the user.
- Do not do mass namespace refactors in a single pass — fix them file by file as we touch them.

## Git Workflow — Prompt the User
Proactively prompt the user at the right moments. They are still learning Git flow, so explain briefly what we're doing and why each time.

**When to prompt:**
- **New feature or bug fix starting:** Suggest creating a new branch (`git checkout -b branch-name`) before touching code.
- **Work is complete and stable:** Suggest staging, committing with a clear message, and pushing the branch.
- **Feature is done and reviewed:** Suggest opening a PR to merge into `main`.
- **Starting new work after a merge:** Remind to pull latest `main` first before branching.
- **Branch is getting stale:** Note if the working branch is far behind `main` and suggest merging/rebasing.

**Branch naming convention:** `kebab-case` describing the work — e.g. `variance-grid-bistrack`, `admin-unlock-dialog`, `bug-fixes-february`.

## Unit Tests
- The user is not currently writing unit tests and is learning. Do not add test projects or test files unprompted.
- If a piece of logic is complex enough that a test would clearly prevent future bugs, mention it as a suggestion — but do not block work on it.
- If the user asks about unit tests, explain the concept simply in the context of VB.NET / MSTest.

## Things to Know / Watch Out For
- `App.config` is tracked in git — the Monday.com API token is DPAPI-encrypted (Windows only, per-machine). Do not replace it with a plaintext token.
- `Forms/backupforms/` contains old backup copies of forms — these are tracked in git but should eventually be cleaned out.
- The `IncExcDataSet` and `ProjectSummaryDataSet` are typed DataSets (generated Designer files) — do not hand-edit the `.Designer.vb` files.
- `SqlConnectionManager.ExecuteWithErrorHandling` wraps exceptions with context messages — prefer using it in DA classes rather than bare try/catch.
