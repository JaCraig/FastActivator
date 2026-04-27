# FastActivator Agent Guide

This file gives coding agents the minimum repository context needed to work safely and quickly.

## First Commands To Run

```powershell
dotnet restore FastActivator.sln
dotnet build FastActivator.sln -c Release
dotnet test FastActivator.PropertyTests/FastActivator.PropertyTests.csproj -c Release
```

If you touched behavior in the constructor cache or activation path, also run:

```powershell
dotnet test FastActivator.Tests/FastActivator.Tests.csproj -c Release
```

## Repository Map

- `FastActivator/`: main library (`Fast.Activator` package).
- `FastActivator/Utils/`: constructor resolution, delegate creation, and cache internals.
- `FastActivator.Tests/`: unit and concurrency tests for activation and cache correctness.
- `FastActivator.PropertyTests/`: property-based tests.
- `FastActivator.Benchmarks/`: BenchmarkDotNet performance comparisons.
- `docfx_project/`: API and article documentation source.

## Important Behavior Contracts

- `FastActivator.CreateInstance(Type, ...)` throws `ArgumentNullException` for null `type`.
- Failed activations generally return `null` rather than throwing.
- Constructor matching and default-value behavior live in `FastActivator/Utils/ConstructorList.cs`.
- Constructor cache implementation lives in `FastActivator/Utils/ConstructorCache.cs` and is intentionally striped (32 stripes) for concurrent access.

## Local Hooks And Commit Rules

- Pre-commit hook runs `dotnet format --include ${staged}` for staged `*.cs` files.
- Commit message hook enforces conventional commits (`build|feat|ci|chore|docs|fix|perf|refactor|revert|style|test`) with:
    - max 90 chars on header line
    - `type(scope): subject` shape
    - subject length >= 4
    - no trailing period/whitespace

## Pitfalls

- CI workflows are centralized via reusable workflows under `JaCraig/Centralized-Workflows`; avoid assuming all CI logic is visible only in this repo.

## Existing Project Docs (Link, Don't Duplicate)

- Overview and usage: [README.md](README.md)
- Contribution expectations: [CONTRIBUTING.md](CONTRIBUTING.md)
- API/docs site source: [docfx_project/index.md](docfx_project/index.md)
- Performance article: [docfx_project/articles/speed.md](docfx_project/articles/speed.md)
