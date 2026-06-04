# Bulky - Enterprise E-Commerce System

Bulky is a robust and scalable E-Commerce backend system built with ASP.NET Core. It demonstrates professional architectural patterns, advanced security implementations, and modern search engine integration.

## Key Features

- **Advanced Search:** Integrated with **Elasticsearch** for high-performance product indexing and searching.
- **Security & Identity:** 
  - Role-based and **Dynamic Policy-Based Authorization**.
  - **JWT Authentication** with secure token management.
  - Custom **Identity** management with `ApplicationUser`.
- **System Resiliency:** 
  - **Rate Limiting** implemented to prevent API abuse and brute-force attacks.
  - **EF Core Retry Strategy** for stable database connectivity in distributed environments.
  - Global Exception Handling Middleware for unified error responses.
- **Background Processing:** 
  - **Hosted Services** for automated token cleanup and cross-service data synchronization.
- **Payment Integration:** Modular payment gateway design (Mock implementation for testing).
- **Performance:** High-speed data retrieval using **MemoryCache** and optimized **AutoMapper** profiles.
- **Logging:** Structured industry-standard logging using **Serilog**.

## Tech Stack

- **Backend:** .NET 8 / ASP.NET Core MVC & Web API
- **Database:** SQL Server (MSSQL)
- **ORM:** Entity Framework Core
- **Search Engine:** Elasticsearch & Kibana
- **Patterns:** Repository Pattern, Unit of Work (UOW), Service Layer Architecture.
- **DevOps:** Docker, Docker Compose for full-stack containerization.

## Development & Testing Note (Disclaimer)

Please note that some modules (such as certain Authorization policies and specific Middlewares) are **temporarily disabled** in this version. This is intentional to:
1.  **Accelerate development** of other core features.
2.  **Facilitate easier local testing** without requiring full authentication handshakes at every step.
3.  **Simulate specific scenarios:** Some "Delays" or "Sleeps" might be present in the code to test **Race Conditions** and **Concurrency** handling.

## Docker Setup

The entire stack is containerized. To run the project:

1. Clone the repository.
2. Ensure you have a `.env` file in the root directory.
3. Run:
```bash
   docker-compose up --build
   
