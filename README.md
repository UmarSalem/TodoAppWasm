# TodoAppWasm

A full-stack .NET 8 portfolio project with:
- **Blazor WebAssembly** front-end (`BlazorApp`)
- **ASP.NET Core Web API** back-end (`WebAPI`)

## Deployment plan used in this repository

- **Front-end:** GitHub Pages (automated with GitHub Actions)
- **Back-end:** configurable public API URL (set via `API_BASE_URL` secret)

> You requested Vercel for backend. See the deployment playbook for practical constraints and alternatives.

## CI/CD

- Generic CI: `.github/workflows/ci.yml`
- Development CI + container publish: `.github/workflows/development-ci.yml`
- GitHub Pages deploy (Blazor): `.github/workflows/blazor-github-pages.yml`

## Front-end deployment (GitHub Pages)

1. Add repository secret:
   - `API_BASE_URL=https://<your-public-api-domain>/`
2. Push to `main`.
3. Workflow publishes Blazor app to GitHub Pages.

## Back-end configuration for cross-origin calls

Configure the API with explicit allowed origins:

```bash
AllowedOrigins=https://<your-username>.github.io
```

Configure the deployed database path or connection string through environment variables:

```bash
ConnectionStrings__TodoDatabase=Data Source=/app/data/Todo.db
```

The API also exposes `/health` for deployment health checks.

## Documentation

- Portfolio strategy: [`docs/PORTFOLIO_SHOWCASE_GUIDE.md`](docs/PORTFOLIO_SHOWCASE_GUIDE.md)
- Deployment details and Vercel clarification: [`docs/DEPLOYMENT_PLAYBOOK.md`](docs/DEPLOYMENT_PLAYBOOK.md)
- Project roadmap and next tasks: [`docs/PROJECT_ROADMAP.md`](docs/PROJECT_ROADMAP.md)
