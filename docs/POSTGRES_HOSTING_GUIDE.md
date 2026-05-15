# PostgreSQL hosting guide

This project supports two database providers:

- `Sqlite` for local development
- `Postgres` for hosted deployment

## Why PostgreSQL for deployment?

PostgreSQL is a better hosted database choice for this Todo app because the data is relational:

- Users own todos.
- Todos belong to users.
- Roles and future authentication features fit naturally into relational tables.

SQLite is simple for local learning, but a hosted backend on Render should use PostgreSQL if you want data to survive redeploys and restarts.

## Migration structure

The project keeps migrations separate by database provider:

```text
EfcDataAccess/Migrations              # SQLite migrations
EfcDataAccess.Postgres/Migrations     # PostgreSQL migrations
```

This matters because EF Core migrations contain provider-specific column types. For example, SQLite and PostgreSQL do not describe identity columns and booleans in exactly the same way.

## Render environment variables

For PostgreSQL deployment, set these on the Render Web Service:

```text
DatabaseProvider=Postgres
ApplyMigrationsOnStartup=true
ConnectionStrings__TodoDatabase=<your-postgres-connection-string>
AllowedOrigins=https://<your-github-username>.github.io
Jwt__Key=<long-random-secret-at-least-32-characters>
Jwt__Issuer=TodoAppWasm.WebAPI
Jwt__Audience=TodoAppWasm.BlazorApp
Jwt__TokenLifetimeMinutes=60
```

`ApplyMigrationsOnStartup=true` lets the API create/update the hosted database schema when the app starts. That is convenient for this portfolio demo. In larger production systems, migrations are usually applied as a separate deployment step.

## Creating a new PostgreSQL migration

When the model changes, create a PostgreSQL migration with:

```powershell
$env:DatabaseProvider="Postgres"
$env:ConnectionStrings__TodoDatabase="Host=localhost;Database=todoapp_migrations;Username=postgres;Password=postgres"

dotnet ef migrations add <MigrationName> `
  --project EfcDataAccess.Postgres\EfcDataAccess.Postgres.csproj `
  --startup-project WebAPI\WebAPI.csproj `
  --context TodoContext `
  --output-dir Migrations
```

Create SQLite migrations in the original `EfcDataAccess` project only when the local SQLite schema changes.

## Applying migrations manually

For a hosted PostgreSQL database, you can also apply migrations manually:

```powershell
$env:DatabaseProvider="Postgres"
$env:ConnectionStrings__TodoDatabase="<your-postgres-connection-string>"

dotnet ef database update `
  --project EfcDataAccess.Postgres\EfcDataAccess.Postgres.csproj `
  --startup-project WebAPI\WebAPI.csproj `
  --context TodoContext
```

Do not commit real connection strings or passwords.
