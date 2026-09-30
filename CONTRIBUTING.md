# Contributing

Issues and pull requests are welcome.

- Run `dotnet build` and `dotnet test` before opening a PR. Warnings are errors.
- Keep the dependency rule: Domain has no dependencies, Application depends on Domain, Infrastructure on Application. Architecture tests enforce it.
- Wire DTOs stay `internal`. Map to domain types in `Platsbanken.Infrastructure/Mapping`.
- Add a test for every mapper fix. Real API payloads often differ from the OpenAPI spec.

## Releasing

Tag a commit on `main` (`git tag v0.1.0 && git push origin v0.1.0`). `.github/workflows/release.yml` tests, packs all four packages with that version, pushes them to nuget.org and creates a GitHub release.

Publishing uses NuGet trusted publishing (OIDC), so no API key is stored in GitHub. One-time setup:

1. On nuget.org, open your account menu, then *Trusted Publishing*, and add a policy: repository owner `Nacorpio`, repository `Platsbanken.NET`, workflow file `release.yml`.
2. In this repository, add an Actions variable `NUGET_USER` containing your nuget.org username (not your email).
3. After the first push, request the `Platsbanken.` package ID prefix reservation on nuget.org.
