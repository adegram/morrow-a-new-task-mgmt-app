# Morrow

A private, single-user task manager built with ASP.NET Core Razor Pages, PostgreSQL, and a responsive dashboard. In production, tasks are stored in hosted PostgreSQL rather than on the container filesystem.

## Deploy on Vercel

This repository is configured for Vercel container deployments. Vercel detects the root-level `Dockerfile.vercel`, builds the .NET 10 app, and routes HTTP traffic to the port supplied in `PORT`.

1. Push this project to GitHub and import that repository in Vercel. Set the project root directory to the folder containing `Dockerfile.vercel` and `Taskflow.csproj`.
2. In the Vercel project, open **Storage** or the **Marketplace**, add the Neon PostgreSQL integration, and create a database. The integration provides a PostgreSQL connection string to the project.
3. In **Settings → Environment Variables**, set `APP_PASSWORD` to a strong, unique password. The app accepts the database URL as `DATABASE_URL` or `POSTGRES_URL`.
4. Deploy or redeploy the project. Open the deployment URL and sign in with `APP_PASSWORD`.

Vercel does not run `docker-compose.yml`; it deploys the app image from `Dockerfile.vercel`. The normal `Dockerfile` and Compose file are for local Docker use. Vercel container instances are stateless, so the Neon database is the durable source of task data.

## Run locally with Docker

From this directory:

```sh
docker compose up --build -d
```

Open [http://localhost:8080](http://localhost:8080). The local PostgreSQL database persists in the `taskflow-data` Docker volume. The local sign-in password is `local-only-password-change-me`; change it in `docker-compose.yml` before sharing the local setup. Stop the app with `docker compose down`. To delete the local database too, run `docker compose down -v`.

## Features

- Dashboard with open, due today, and completed task summaries
- Today, Upcoming, All tasks, and Completed views
- Create, edit, complete, and delete tasks
- Notes, due dates, categories, and priorities
- Search and sort the current task list
- Password-protected dashboard with secure authentication cookies
- PostgreSQL persistence and responsive desktop/mobile layout

## Configuration

| Variable | Required | Description |
| --- | --- | --- |
| `DATABASE_URL` | Yes | PostgreSQL connection string. URI format (`postgresql://...`) and Npgsql keyword format are supported. |
| `POSTGRES_URL` | Fallback | Alternate environment variable name used by some Vercel integrations. |
| `APP_PASSWORD` | Production | Shared password used to sign in. Required in Production. |
| `PORT` | Vercel | Port Vercel assigns to the container; read automatically by the app. |

## Database note

This version stores tasks in PostgreSQL. It does not automatically import rows from an older SQLite database. If that SQLite file contains tasks you need, export them before switching deployments.
