# Morrow

A public task manager built with ASP.NET Core Razor Pages, PostgreSQL, and a responsive dashboard. No sign-in is required. All visitors share the same task list, so tasks added, edited, completed, or deleted by one visitor are visible to everyone.

## Deploy on Vercel

This repository is configured f or Vercel container deployments. Vercel builds the .NET 10 app with the root-level `Dockerfile.vercel` and routes HTTP traffic to the port supplied in `PORT`.

1. Push this project to GitHub and import that repository in Vercel. Set **Root Directory** to the folder containing `Dockerfile.vercel` and `Taskflow.csproj` (use `.` if they are at the repository root).
2. Set **Application Preset** to **Container**. Leave the build and output settings at their defaults; the Dockerfile defines the build and start commands.
3. In the Vercel project, open **Storage** or the **Marketplace**, add the Neon PostgreSQL integration, and create a database. The integration provides a PostgreSQL connection string to the project.
4. Confirm the Neon integration supplies `DATABASE_URL` or `POSTGRES_URL` to the app.
5. Deploy or redeploy the project and open the public deployment URL.

Vercel does not run `docker-compose.yml`; it deploys the app image from `Dockerfile.vercel`. The normal `Dockerfile` and Compose file are for local Docker use. Vercel container instances are stateless, so Neon is the durable store for both task data and ASP.NET Core Data Protection keys. The app creates the `tasks` and `data_protection_keys` tables automatically at startup; no manual SQL setup or extra environment variable is needed.

If you deployed an earlier version, push the updated source to the connected GitHub branch and let Vercel create a new deployment. The shared Data Protection key store is required so anti-forgery tokens from a page request can be validated by any serverless instance handling the form submission.

## Run locally with Docker

From this directory:

```sh
docker compose up --build -d
```

Open [http://localhost:8080](http://localhost:8080). The local PostgreSQL database persists in the `taskflow-data` Docker volume. Stop the app with `docker compose down`. To delete the local database too, run `docker compose down -v`.

## Features

- Dashboard with open, due today, and completed task summaries
- Today, Upcoming, All tasks, and Completed views
- Create, edit, complete, and delete tasks
- Notes, due dates, categories, and priorities
- Search and sort the current task list
- Public dashboard with no sign-in flow
- PostgreSQL persistence and responsive desktop/mobile layout

## Public access and shared data

There is no sign-in or per-user isolation. Every visitor can view, create, edit, complete, and delete tasks in the same shared workspace. Treat the deployment as a public demo; do not store private or sensitive information in it.

## Configuration

| Variable | Required | Description |
| --- | --- | --- |
| `DATABASE_URL` | Yes | PostgreSQL connection string. URI format (`postgresql://...`) and Npgsql keyword format are supported. |
| `POSTGRES_URL` | Fallback | Alternate environment variable name used by some Vercel integrations. |
| `PORT` | Vercel | Port Vercel assigns to the container; read automatically by the app. |

## Database note

This version stores tasks in PostgreSQL. It does not automatically import rows from an older SQLite database. If that SQLite file contains tasks you need, export them before switching deployments.
