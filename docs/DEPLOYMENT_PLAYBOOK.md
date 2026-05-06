# Deployment Playbook (GitHub Pages Front-end + Vercel Discussion)

## Your target

- Front-end: **GitHub Pages** ✅
- Back-end: **Vercel** (requested)

## Important technical clarification

For a .NET ASP.NET Core Web API, Vercel is usually **not** the simplest/official runtime target.

Vercel's primary backend runtimes are serverless-focused (Node.js, Python, Go, Rust, etc.), so deploying a full ASP.NET Core API directly is not the standard path.

### What this means for you

You have two realistic options:

1. **Recommended (fastest + least risk):**
   - Keep front-end on GitHub Pages.
   - Deploy .NET API on Render/Fly/Railway/Azure free-tier.
   - Point `ApiBase` to that public API URL.

2. **Vercel-only backend path (higher effort):**
   - Rebuild API endpoints as Vercel Functions in a supported runtime.
   - This is effectively a backend rewrite, not simple hosting.

## Implemented in this repository

This repo is now prepared for option #1 (front-end on GitHub Pages + configurable API URL):

- GitHub Pages workflow: `.github/workflows/blazor-github-pages.yml`
- Production API URL injection via GitHub secret: `API_BASE_URL`
- Blazor app config files:
  - `BlazorApp/wwwroot/appsettings.json`
  - `BlazorApp/wwwroot/appsettings.Production.json`
- Backend CORS now supports explicit origin allowlist via `AllowedOrigins` config in `WebAPI`.
- Backend Dockerfile targets Linux containers for hosts such as Render.
- Backend database path is configurable with `ConnectionStrings__TodoDatabase`.
- Backend exposes `/health` so a host can check whether the API is running.

## Back-end container settings

The WebAPI container listens on port `8080`.

For a first hosted demo with SQLite, set:

```bash
ConnectionStrings__TodoDatabase=Data Source=/app/data/Todo.db
AllowedOrigins=https://<your-username>.github.io
```

This SQLite file is useful only for a simple demo. On many free container hosts, local files are not permanent after restart or redeploy. For real user accounts and authentication, move the deployed database to PostgreSQL in the next backend task.

When PostgreSQL is added later, the same configuration idea stays the same: the connection string comes from the hosting platform's environment variables, not from source code.

## Step-by-step rollout

### 1) Enable GitHub Pages deployment

1. Push to `main`.
2. In repo settings, enable **GitHub Pages** using **GitHub Actions** source.
3. Add secret `API_BASE_URL` with your public API URL.

### 2) Deploy API

If you still want pure Vercel backend, expect rewrite work.
If you want fastest completion, deploy current `WebAPI` container on a .NET-friendly host.

### 3) Configure CORS for production

Set backend config:

- `AllowedOrigins=https://<your-username>.github.io`

If using project pages path, origin remains same host (`https://<your-username>.github.io`).

### 4) Verify recruiter experience

- Open GitHub Pages link.
- Create/read/update/delete one todo.
- Confirm no CORS or mixed-content (HTTP/HTTPS) errors.

## What I recommend for your portfolio deadline

- Use GitHub Pages now.
- Use a free .NET-friendly API host now.
- Mention in README: "Front-end hosted on GitHub Pages, API hosted on free-tier backend (may cold start)."
- Keep Vercel as optional future experiment only if you want to rewrite backend functions.
