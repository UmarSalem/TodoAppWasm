# Frontend live backend checklist

Use this checklist after the Render backend URL is available.

## 1. Confirm the backend is live

Open the Render health endpoint:

```text
https://<your-render-service>.onrender.com/health
```

Expected response:

```json
{
  "status": "Healthy"
}
```

Then open Swagger:

```text
https://<your-render-service>.onrender.com/swagger
```

## 2. Configure backend CORS

On Render, set:

```text
AllowedOrigins=https://<your-github-username>.github.io
```

If the frontend is hosted at:

```text
https://<your-github-username>.github.io/TodoAppWasm/
```

the origin is still:

```text
https://<your-github-username>.github.io
```

Do not include the project path in `AllowedOrigins`.

## 3. Configure GitHub Pages frontend

In GitHub repository secrets, set:

```text
API_BASE_URL=https://<your-render-service>.onrender.com/
```

The Blazor GitHub Pages workflow writes this value into:

```text
appsettings.Production.json
```

during deployment.

## 4. Optional manual deployment test

The GitHub Pages workflow also supports a manual input named:

```text
api_base_url
```

Use it from **Actions -> Deploy Blazor to GitHub Pages -> Run workflow** if you want to test a Render URL before saving it permanently as a repository secret.

## 5. Verify the deployed frontend

Open:

```text
https://<your-github-username>.github.io/TodoAppWasm/
```

Test:

1. Open the app.
2. Create a demo user.
3. Log in.
4. Create a todo.
5. Load todos.
6. Edit a todo.
7. Delete a completed todo.

If the frontend cannot reach the backend, check:

- `API_BASE_URL` points to the Render URL and ends with `/`.
- Render `AllowedOrigins` contains `https://<your-github-username>.github.io`.
- Browser DevTools does not show CORS or mixed-content errors.
- Render service is awake; free tier can cold start after being idle.
