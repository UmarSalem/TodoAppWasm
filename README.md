# TodoAppWasm

A full-stack .NET 8 portfolio project with:
- **Blazor WebAssembly** front-end (`BlazorApp`)
- **ASP.NET Core Web API** back-end (`WebAPI`)

## Deployment plan used in this repository

- **Front-end:** GitHub Pages (automated with GitHub Actions)
- **Back-end:** Render Web Service using a Docker image from GitHub Container Registry

Vercel was considered for the backend, but Render was selected because this WebAPI is deployed as a Docker container.

## CI/CD

- Generic CI: `.github/workflows/ci.yml`
- Development CI + container publish: `.github/workflows/development-ci.yml`
- GitHub Pages deploy (Blazor): `.github/workflows/blazor-github-pages.yml`

## Front-end deployment (GitHub Pages)

1. Add repository secret:
   - `API_BASE_URL=https://<your-render-api-domain>/`
2. Push to `main`.
3. Workflow publishes Blazor app to GitHub Pages.

Local Blazor development uses:

```bash
BlazorApp/wwwroot/appsettings.json
```

Production GitHub Pages uses the `API_BASE_URL` secret to generate `appsettings.Production.json`.

## Back-end configuration for cross-origin calls

Configure the API with explicit allowed origins:

```bash
AllowedOrigins=https://<your-username>.github.io
```

Configure the deployed database path or connection string through environment variables:

```bash
DatabaseProvider=Sqlite
ConnectionStrings__TodoDatabase=Data Source=/app/data/Todo.db
```

For a hosted PostgreSQL database, use:

```bash
DatabaseProvider=Postgres
ConnectionStrings__TodoDatabase=<connection-string-from-your-database-host>
```

The API also exposes `/health` for deployment health checks.

For JWT login in a hosted environment, override the development signing key:

```bash
Jwt__Key=<long-random-secret-at-least-32-characters>
Jwt__Issuer=TodoAppWasm.WebAPI
Jwt__Audience=TodoAppWasm.BlazorApp
```

Swagger can call protected Todo endpoints after login:

1. Call `POST /Users/login`.
2. Copy the returned token.
3. Click **Authorize** in Swagger.
4. Paste `Bearer <token>`.

## Documentation

- Portfolio strategy: [`docs/PORTFOLIO_SHOWCASE_GUIDE.md`](docs/PORTFOLIO_SHOWCASE_GUIDE.md)
- Deployment details: [`docs/DEPLOYMENT_PLAYBOOK.md`](docs/DEPLOYMENT_PLAYBOOK.md)
- Project roadmap and next tasks: [`docs/PROJECT_ROADMAP.md`](docs/PROJECT_ROADMAP.md)
