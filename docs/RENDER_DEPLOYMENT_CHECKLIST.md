# Render deployment checklist

This checklist is for deploying the `WebAPI` backend as a Render Web Service from the Docker image published to GitHub Container Registry.

## 1. Build and publish the image from GitHub

Merge the backend deployment branch into `development`.

The `development-ci.yml` workflow will:

1. Restore, build, and test the solution.
2. Build the WebAPI Docker image for `linux/amd64`.
3. Push two tags to GitHub Container Registry:
   - `ghcr.io/<github-user-or-org>/todoappwasm:<commit-sha>`
   - `ghcr.io/<github-user-or-org>/todoappwasm:development`

## 2. Make sure Render can pull the image

Render can deploy prebuilt Docker images from GitHub Container Registry.

If the GitHub package is public, Render can pull it directly.

If the GitHub package is private, create a Render container registry credential:

```text
Registry: GitHub Container Registry
Username: <your GitHub username>
Token: GitHub PAT with package read access
```

## 3. Create the Render Web Service

In Render:

1. Click **New**.
2. Choose **Web Service**.
3. Choose **Existing Image**.
4. Use this image URL:

```text
ghcr.io/<github-user-or-org>/todoappwasm:development
```

5. Select the free instance type for portfolio/demo use.
6. Set the health check path:

```text
/health
```

## 4. Set backend environment variables

For the first demo with SQLite:

```text
DatabaseProvider=Sqlite
ApplyMigrationsOnStartup=false
ConnectionStrings__TodoDatabase=Data Source=/app/data/Todo.db
AllowedOrigins=https://<your-github-username>.github.io
Jwt__Key=<long-random-secret-at-least-32-characters>
Jwt__Issuer=TodoAppWasm.WebAPI
Jwt__Audience=TodoAppWasm.BlazorApp
Jwt__TokenLifetimeMinutes=60
```

Render provides `PORT` automatically. The Docker entrypoint reads that value and starts ASP.NET Core on the correct port.

SQLite is acceptable for a temporary demo, but it is not the final database plan. For a real hosted app, use PostgreSQL.

For hosted PostgreSQL later:

```text
DatabaseProvider=Postgres
ApplyMigrationsOnStartup=true
ConnectionStrings__TodoDatabase=<hosted-postgres-connection-string>
```

`ApplyMigrationsOnStartup=true` tells the API to apply EF Core migrations when the Render service starts. Use it for the first hosted PostgreSQL demo database. For larger production systems, migrations are usually applied from a separate release step instead of inside the web app.

## 5. Add GitHub secrets for automatic Render deploys

In GitHub repository settings, add:

```text
RENDER_SERVICE_ID=<your Render service id>
RENDER_API_KEY=<your Render API key>
```

The workflow triggers Render with the exact image tag from the GitHub commit.

## 6. Verify the backend

After Render deploys, open:

```text
https://<your-render-service>.onrender.com/health
```

Expected response:

```json
{
  "status": "Healthy"
}
```

Then open:

```text
https://<your-render-service>.onrender.com/swagger
```

Test:

1. `POST /Users`
2. `POST /Users/login`
3. Copy the JWT token.
4. Click **Authorize** in Swagger.
5. Test a protected Todo endpoint.

## 7. Connect the frontend later

When the backend URL is working, add this GitHub secret for the Blazor GitHub Pages workflow:

```text
API_BASE_URL=https://<your-render-service>.onrender.com/
```

That is the next task after backend deployment.
