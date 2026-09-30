# Contributing

Issues and pull requests are welcome.

- Run `dotnet build` and `dotnet test` before opening a PR. Warnings are errors.
- Keep the dependency rule: Domain has no dependencies, Application depends on Domain, Infrastructure on Application. Architecture tests enforce it.
- Wire DTOs stay `internal`. Map to domain types in `Platsbanken.Infrastructure/Mapping`.
- Add a test for every mapper fix. Real API payloads often differ from the OpenAPI spec.
