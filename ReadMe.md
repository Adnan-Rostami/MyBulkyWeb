Bulky – Scalable E-Commerce Backend (.NET 8)

Bulky is a scalable E-Commerce backend built with **ASP.NET Core .NET 8** using **Layered Architecture** and common enterprise backend patterns.

## Key Features
- JWT Authentication
- Role-based and Policy-based Authorization
- Elasticsearch integration for product search
- Rate Limiting
- Background Services
- Global Exception Handling
- Serilog Logging
- Docker support
- Payment gateway structure
- MemoryCache support

## Architecture & Patterns
- Layered Architecture
- Service Layer
- Repository Pattern
- Unit of Work

## Tech Stack
- .NET 8
- ASP.NET Core Web API
- C#
- SQL Server
- Entity Framework Core
- Elasticsearch
- Docker
- Serilog

## Getting Started
1. Clone the repository
2. Configure required settings
3. Apply database migrations
4. Run the application

## Docker Setup
1. Make sure required environment variables or `.env` file exist
2. Run:
```bash
docker-compose up --build
=======
Highlights
Designed RESTful APIs with JWT Authentication and dynamic permission-based authorization.
Integrated Elasticsearch for high-performance product search and indexing.
Implemented rate limiting to prevent brute-force and abuse attacks.
Built background services for automated token cleanup and system synchronization.
Applied structured logging with Serilog.
Containerized full stack using Docker & Docker Compose.
Tech Stack
.NET 8 / ASP.NET Core Web API
SQL Server + EF Core
Elasticsearch
Docker
>>>>>>> f0ba4a0b42c846eb021f9065124e0c22d8f6f9c4
