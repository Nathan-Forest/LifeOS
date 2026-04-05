# 🌿 LifeOS API

> A personal productivity backend powering the LifeOS suite — habits, goals, projects, tasks, and study tracking, all in one JWT-authenticated REST API.

![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=flat&logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=flat&logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub_Actions-2088FF?style=flat&logo=githubactions&logoColor=white)

## Overview

LifeOS API is a production-deployed RESTful backend built with C# and .NET 8. It provides full CRUD operations across five life management domains, protected by JWT authentication issued by SecureAuth-Lite.

The API is consumed exclusively through CloudControl's Next.js frontend, with all requests proxied through Next.js API routes to keep the backend off the public internet.

## Features

- **Habits** — Full CRUD with soft delete, frequency, target count, colour coding, and active toggle
- **Goals** — Track long-term objectives with progress
- **Projects** — Manage personal and portfolio projects
- **Tasks** — To-do and task management linked to projects
- **Study** — Study session tracking and notes
- **JWT Authentication** — All endpoints protected, tokens issued by SecureAuth-Lite
- **Soft Deletes** — Records marked inactive (`IsActive = false`) rather than permanently removed
- **EF Core Migrations** — Database schema managed with Entity Framework Core

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | .NET 8 / ASP.NET Core |
| ORM | Entity Framework Core |
| Database | SQLite |
| Auth | JWT Bearer tokens |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions (self-hosted runner) |

## API Endpoints
```
GET    /api/habits          — List all active habits
POST   /api/habits          — Create a habit
PUT    /api/habits/{id}     — Update a habit
DELETE /api/habits/{id}     — Soft delete a habit

# Goals, Projects, Tasks, Study follow the same pattern
```

## Running Locally
```bash
git clone https://github.com/Nathan-Forest/LifeOS.git
cd LifeOS
dotnet restore
dotnet run
```

> **Note:** Requires a `.env` file with `JWT_SECRET` matching your SecureAuth-Lite instance. The `.env` file is not committed — see `.env.example` for required variables.

## Deployment

Deployed on a self-hosted Ubuntu Server via Docker Compose with a persistent SQLite volume mount (`./data:/app/data`). Auto-deployed via GitHub Actions self-hosted runner on push to main.

## Part of The Forest Den

LifeOS API is the backend for the LifeOS suite, consumed by [CloudControl](https://github.com/Nathan-Forest/cloudcontrol) and authenticated via SecureAuth-Lite.