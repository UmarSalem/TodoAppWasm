# Project Roadmap

This roadmap explains the next portfolio tasks for the TodoAppWasm project. The goal is to make the backend reliable first, then connect and polish the Blazor WebAssembly frontend.

## Database Direction

For this project, PostgreSQL is the best next database to learn and use for deployment.

Reasons:

- The app has relational data: users own todos, todos belong to users, and future authentication will add login data.
- PostgreSQL works very well with EF Core, migrations, Docker, and cloud hosting.
- PostgreSQL is widely used in modern backend jobs and is especially useful beside .NET, Docker, and cloud deployment.
- MongoDB is useful to learn later, but it fits document-style data better than this app's current relational model.

For local development we can keep SQLite for a short time, but the deployed backend should use a hosted PostgreSQL database such as Supabase, Neon, or Render Postgres.

## Backend Tasks

### 032_backend_deployment_readiness

Prepare the ASP.NET Core WebAPI for Linux container deployment.

- Update Docker configuration for Linux deployment.
- Fix Docker build inputs so all referenced projects are copied.
- Make the database connection configurable from app settings or environment variables.
- Add a simple health endpoint for hosting checks.
- Add short comments explaining important deployment configuration.
- Verify `dotnet restore`, `dotnet build`, `dotnet test`, and Docker build if Docker is available.

### 033_backend_postgres_ready

Prepare EF Core for hosted PostgreSQL while keeping local development simple.

- Add PostgreSQL provider support.
- Configure local SQLite and production PostgreSQL through settings.
- Add EF Core migrations for the real database schema.
- Document how to create the hosted database and where to place the connection string.
- Verify the API can start with the selected database provider.

### 034_backend_auth_user_accounts

Add proper user login foundations.

- Add password hash storage to the user model/database.
- Add user registration validation.
- Hash passwords before saving them.
- Add login endpoint.
- Return a safe user response without password data.
- Add tests for registration and login failures.

### 035_backend_jwt_authorization

Protect user-specific endpoints with JWT authentication.

- Add JWT configuration through environment variables.
- Issue JWT tokens from the login endpoint.
- Require authorization for Todo endpoints.
- Use the logged-in user id instead of trusting `ownerId` from the request body.
- Add tests for authorized and unauthorized requests.

### 036_backend_production_polish

Make the backend easier to operate and review.

- Improve Swagger documentation for auth and common responses.
- Add seed data only for development/demo when useful.
- Add structured error responses consistently.
- Update README with backend deployment and database instructions.

## Frontend Tasks

### 037_blazor_api_configuration

Connect the Blazor app cleanly to different backend URLs.

- Read API base URL from app settings.
- Document local, staging, and production frontend configuration.
- Make frontend errors visible when the API is offline or cold-starting.

### 038_blazor_auth_flow

Add frontend login and registration.

- Build register and login pages.
- Store JWT safely enough for a portfolio WASM app.
- Attach JWT to protected API calls.
- Add logout behavior.

### 039_blazor_todo_user_experience

Polish the Todo workflow.

- Show only todos for the logged-in user.
- Add loading, empty, error, and success states.
- Add confirmation before delete.
- Improve form validation for title and description.

### 040_portfolio_release

Prepare the final recruiter-friendly release.

- Update README with live links, screenshots, architecture, and setup commands.
- Add badges for CI and container image.
- Add known limitations and future improvements.
- Merge tested work through `development` into `main`.
- Deploy frontend to GitHub Pages and backend to the selected host.

## Suggested Branch Flow

Use one branch per task:

```text
032_backend_deployment_readiness
033_backend_postgres_ready
034_backend_auth_user_accounts
035_backend_jwt_authorization
036_backend_production_polish
037_blazor_api_configuration
038_blazor_auth_flow
039_blazor_todo_user_experience
040_portfolio_release
```

Recommended merge path:

```text
feature branch -> development -> main
```

Use `development` for integration testing and `main` for portfolio-ready deployment.
