# Event Reminder

## Description
Event Reminder is a .NET 8 Web API for organizing personal and group events, tracking attendees, and delivering async notifications. The solution ships with a dedicated notifications service, SQL Server persistence, and RabbitMQ-backed messaging.

## Features
- JWT-based user authentication and registration
- Personal events and group events with invitations
- Friendship requests and friendship management
- Attendee tracking and paginated list endpoints
- Async notifications via RabbitMQ
- Swagger/OpenAPI documentation for the HTTP API

## Tech Stack
- ASP.NET Core 8
- EF Core 8 + SQL Server 2022
- RabbitMQ 3 (AMQP)
- Swagger/OpenAPI
- MailKit (email delivery)
- Docker + Docker Compose

## Architecture / Design Decisions
- Clean Architecture with distinct Domain, Application, Infrastructure, and Persistence layers
- CQRS with MediatR for commands and queries
- FluentValidation for request validation
- Dependency Injection per layer (AddApplication/AddInfrastructure/AddPersistence)
- Scoped EF Core DbContext in the API service with automatic migrations on startup

## Project Structure
- [EventReminder.Domain/EventReminder.Domain.csproj](EventReminder.Domain/EventReminder.Domain.csproj) - Domain entities and core business rules
- [EventReminder.Application/EventReminder.Application.csproj](EventReminder.Application/EventReminder.Application.csproj) - CQRS handlers, validators, behaviors, and abstractions
- [EventReminder.Persistence/EventReminder.Persistence.csproj](EventReminder.Persistence/EventReminder.Persistence.csproj) - EF Core DbContext, configurations, repositories
- [EventReminder.Infrastructure/EventReminder.Infrastructure.csproj](EventReminder.Infrastructure/EventReminder.Infrastructure.csproj) - Auth, crypto, email, messaging implementations
- [EventReminder.BackgroundTasks/EventReminder.BackgroundTasks.csproj](EventReminder.BackgroundTasks/EventReminder.BackgroundTasks.csproj) - Background workers for notifications and integration events
- [EventReminder.Contracts/EventReminder.Contracts.csproj](EventReminder.Contracts/EventReminder.Contracts.csproj) - Shared request/response DTOs
- [EventReminder.Services.Api/EventReminder.Services.Api.csproj](EventReminder.Services.Api/EventReminder.Services.Api.csproj) - Public REST API and middleware
- [EventReminder.Services.Notifications/EventReminder.Services.Notifications.csproj](EventReminder.Services.Notifications/EventReminder.Services.Notifications.csproj) - Notifications service host

## Getting Started

### Prerequisites
- Docker Desktop (Windows) with Linux containers enabled
- .NET 8 SDK (for local development)

### Quick Start (Docker Compose)
1. From the repo root, start all services:

```bash
docker compose up --build
```

2. Wait until the containers are healthy and the logs show the apps are listening.

### Local Development (optional)
1. Update connection strings in [EventReminder.Services.Api/appsettings.Development.json](EventReminder.Services.Api/appsettings.Development.json) and [EventReminder.Services.Notifications/appsettings.Development.json](EventReminder.Services.Notifications/appsettings.Development.json).
2. Start SQL Server and RabbitMQ (Docker Compose is recommended).
3. Run the API:

```bash
dotnet run --project EventReminder.Services.Api/EventReminder.Services.Api.csproj
```

4. Run the notifications service:

```bash
dotnet run --project EventReminder.Services.Notifications/EventReminder.Services.Notifications.csproj
```

### Service URLs
- API service: http://localhost:5000
- Notifications service: http://localhost:6000
- SQL Server: localhost:1433
- RabbitMQ management UI: http://localhost:15672 (user: guest, pass: guest)

## Environment Variables

### Docker Compose
- `ACCEPT_EULA`: "Y"
- `SA_PASSWORD`: "Strong_password_123!"
- `RABBITMQ_DEFAULT_USER`: "guest"
- `RABBITMQ_DEFAULT_PASS`: "guest"

### appsettings.Development.json
- `ConnectionStrings:EventReminderDb`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:SecurityKey`
- `Jwt:TokenExpirationInMinutes`
- `MessageBroker:HostName`
- `MessageBroker:Port`
- `MessageBroker:UserName`
- `MessageBroker:Password`
- `MessageBroker:QueueName`

## API Documentation / Usage
- Swagger UI: http://localhost:5000/swagger/index.html
- All endpoints are rooted at the API base URL and use JSON request/response bodies.
- Authenticated endpoints require `Authorization: Bearer <token>`.

### Example: Login
**Request**

```http
POST /authentication/login
Content-Type: application/json

{
	"email": "user@example.com",
	"password": "P@ssw0rd!"
}
```

**Response**

```json
{
	"token": "<jwt-token>"
}
```

### Common Endpoints
- `POST /authentication/register`
- `GET /personal-events`
- `POST /personal-events`
- `GET /group-events`
- `POST /group-events/{groupEventId:guid}/invite`
- `GET /invitations/pending`
- `POST /friendship-requests/{friendshipRequestId:guid}/accept`

## Running Tests
No automated test projects are currently included. Add tests and run them with:

```bash
dotnet test
```

## Deployment
- Production-style deployment is defined in [docker-compose.yml](docker-compose.yml).
- The API applies EF Core migrations at startup in [EventReminder.Services.Api/Startup.cs](EventReminder.Services.Api/Startup.cs).

### Stop Services
```bash
docker compose down
```

## Contributing
1. Fork the repository and create a feature branch.
2. Follow the existing layering and CQRS patterns.
3. Add or update tests when introducing new behavior.
4. Open a pull request with a clear summary and rationale.

## License
MIT. See [LICENSE](LICENSE).
"# event-reminder" 
