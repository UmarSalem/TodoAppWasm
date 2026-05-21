# Portfolio screenshots

This folder contains screenshots used by the main project README.

The screenshots show both sides of the portfolio project:

- The Blazor WebAssembly frontend
- The ASP.NET Core WebAPI through Swagger
- JWT login and protected Todo endpoints

## Screenshot catalog

| File | Area | What it shows |
| --- | --- | --- |
| `01-home.png` | Blazor UI | Portfolio-style home page and top navigation |
| `02-login.png` | Blazor UI | Login form before authentication |
| `03-users.png` | Blazor UI | Users overview page |
| `05-view-todos.png` | Blazor UI | Todo list after loading protected data |
| `09-TodoWithFilter.png` | Blazor UI | Todo filtering UI |
| `06-swagger-auth.png` | Swagger | Swagger authentication support |
| `07-swagger-loginWithToken.png` | Swagger | Login request returning a JWT token |
| `10-Swagger_AuthorizationWithtoken.png` | Swagger | JWT token pasted into Swagger Authorize |
| `11-Swagger_GetTodo_with_valid_Auth.png` | Swagger | Protected Todo endpoint working with valid JWT auth |
| `12_Todo_Table.png` | Internal reference | Local SQLite Todo table for development/debugging |
| `13_User_Table.png` | Internal reference | Local SQLite User table for development/debugging |

## Recommended README order

Use the screenshots in this order so the story is easy for recruiters or reviewers to follow:

1. `01-home.png` - show the finished app first.
2. `02-login.png` - show the authentication entry point.
3. `03-users.png` - show user management.
4. `05-view-todos.png` - show protected Todo data in Blazor.
5. `09-TodoWithFilter.png` - show filtering and interaction.
6. `06-swagger-auth.png` - show API documentation.
7. `07-swagger-loginWithToken.png` - show JWT login.
8. `10-Swagger_AuthorizationWithtoken.png` - show Swagger authorization.
9. `11-Swagger_GetTodo_with_valid_Auth.png` - show protected endpoint access.

## Markdown examples

Single screenshot:

```md
![TodoApp home](docs/screenshots/01-home.png)
```

Screenshot with a heading:

```md
### Login

![Login page](docs/screenshots/02-login.png)
```

## Screenshot guidance

- Use demo users only. Do not show private passwords or real personal data.
- Do not use database screenshots with password hashes in the public README.
- Keep browser zoom at 100%.
- Use a desktop width when possible, for example 1440 x 900.
- Refresh with `Ctrl + F5` before taking screenshots so the newest CSS is visible.
- Prefer screenshots that show successful working flows instead of empty pages.
- If a screenshot contains a JWT token, use it only for portfolio demonstration and rotate/change the token later if needed.

## Internal-only screenshots

`12_Todo_Table.png` and `13_User_Table.png` are useful while learning EF Core and SQLite, but they should stay out of the public README because they show database internals. In particular, `13_User_Table.png` includes the `PasswordHash` column. A hash is not the raw password, but public portfolio screenshots should avoid exposing authentication data.

## Future screenshots to add

These would make the portfolio even stronger later:

| File | What to capture |
| --- | --- |
| `04-create-todo.png` | Create Todo form while logged in |
| `12-github-pages-live.png` | Deployed Blazor frontend on GitHub Pages |
| `13-render-health.png` | Render backend health endpoint |
| `14-render-swagger.png` | Deployed Swagger page |
