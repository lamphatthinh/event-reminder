# Event Reminder Rebuild Guide

This guide helps you rebuild the Event Reminder application from an empty solution. The goal is to learn the architecture by implementing the app layer by layer.

## Version Baseline

Use these versions to rebuild the app close to the current repository:

```text
.NET target framework: net8.0
.NET SDK: 8.0.419
SQL Server Docker image: mcr.microsoft.com/mssql/server:2022-latest
RabbitMQ Docker image: rabbitmq:management
```

Create `global.json` in the solution root:

```json
{
  "sdk": {
    "version": "8.0.419",
    "rollForward": "latestFeature"
  }
}
```

Package versions used by this app:

| Package | Version |
| --- | --- |
| `FluentValidation.AspNetCore` | `11.3.0` |
| `FluentValidation.DependencyInjectionExtensions` | `11.9.2` |
| `MailKit` | `4.6.0` |
| `MediatR` | `12.3.0` |
| `MediatR.Contracts` | `2.0.1` |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | `8.0.6` |
| `Microsoft.Data.SqlClient` | `5.2.1` |
| `Microsoft.EntityFrameworkCore` | `8.0.6` |
| `Microsoft.EntityFrameworkCore.SqlServer` | `8.0.6` |
| `Microsoft.EntityFrameworkCore.Tools` | `8.0.6` |
| `Microsoft.Extensions.Hosting` | `8.0.0` |
| `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` | `1.20.1` |
| `Newtonsoft.Json` | `13.0.3` |
| `RabbitMQ.Client` | `6.8.1` |
| `Swashbuckle.AspNetCore` | `6.6.2` |

## 1. Initialize The Solution

Create the solution and projects:

```bash
dotnet new sln -n EventReminder

dotnet new classlib -n EventReminder.Domain
dotnet new classlib -n EventReminder.Application
dotnet new classlib -n EventReminder.Contracts
dotnet new classlib -n EventReminder.Persistence
dotnet new classlib -n EventReminder.Infrastructure
dotnet new classlib -n EventReminder.BackgroundTasks
dotnet new webapi -n EventReminder.Services.Api
dotnet new webapi -n EventReminder.Services.Notifications
```

Add projects to the solution:

```bash
dotnet sln add EventReminder.Domain/EventReminder.Domain.csproj
dotnet sln add EventReminder.Application/EventReminder.Application.csproj
dotnet sln add EventReminder.Contracts/EventReminder.Contracts.csproj
dotnet sln add EventReminder.Persistence/EventReminder.Persistence.csproj
dotnet sln add EventReminder.Infrastructure/EventReminder.Infrastructure.csproj
dotnet sln add EventReminder.BackgroundTasks/EventReminder.BackgroundTasks.csproj
dotnet sln add EventReminder.Services.Api/EventReminder.Services.Api.csproj
dotnet sln add EventReminder.Services.Notifications/EventReminder.Services.Notifications.csproj
```

Add project references:

```bash
dotnet add EventReminder.Application reference EventReminder.Domain
dotnet add EventReminder.Application reference EventReminder.Contracts

dotnet add EventReminder.Persistence reference EventReminder.Application
dotnet add EventReminder.Persistence reference EventReminder.Domain

dotnet add EventReminder.Infrastructure reference EventReminder.Application

dotnet add EventReminder.BackgroundTasks reference EventReminder.Application
dotnet add EventReminder.BackgroundTasks reference EventReminder.Infrastructure
dotnet add EventReminder.BackgroundTasks reference EventReminder.Persistence

dotnet add EventReminder.Services.Api reference EventReminder.Application
dotnet add EventReminder.Services.Api reference EventReminder.Infrastructure
dotnet add EventReminder.Services.Api reference EventReminder.Persistence

dotnet add EventReminder.Services.Notifications reference EventReminder.Application
dotnet add EventReminder.Services.Notifications reference EventReminder.Infrastructure
dotnet add EventReminder.Services.Notifications reference EventReminder.Persistence
dotnet add EventReminder.Services.Notifications reference EventReminder.BackgroundTasks
```

## 2. Build The Domain Layer

The domain layer should contain business rules and core entities. It should not depend on ASP.NET Core, EF Core, RabbitMQ, SQL Server, or email libraries.

Start with shared primitives:

- `Entity`
- `AggregateRoot`
- `ValueObject`
- `DomainEvent`
- `Result`
- `Error`
- `Maybe`

Then add domain models:

- `User`
- `PersonalEvent`
- `GroupEvent`
- `Invitation`
- `FriendshipRequest`
- `Friendship`
- `Attendee`
- `Notification`

Example domain questions:

- Can this user create an event?
- Can this invitation be accepted?
- Can this friendship request be approved?
- Should a notification be created?
- Can this group event be updated by this user?

## 3. Build The Application Layer

The application layer contains use cases. Use commands and queries to describe user actions.

Install packages:

```bash
dotnet add EventReminder.Application package FluentValidation.DependencyInjectionExtensions --version 11.9.2
dotnet add EventReminder.Application package MediatR --version 12.3.0
dotnet add EventReminder.Application package Microsoft.Data.SqlClient --version 5.2.1
dotnet add EventReminder.Application package Microsoft.EntityFrameworkCore --version 8.0.6
dotnet add EventReminder.Application package Newtonsoft.Json --version 13.0.3
```

Create messaging abstractions:

- `ICommand`
- `ICommandHandler`
- `IQuery`
- `IQueryHandler`

Create service abstractions:

- `IUserRepository`
- `IPersonalEventRepository`
- `IGroupEventRepository`
- `IInvitationRepository`
- `IFriendshipRequestRepository`
- `INotificationRepository`
- `IUnitOfWork`
- `IPasswordHasher`
- `IPasswordHashChecker`
- `IJwtProvider`
- `IDateTime`
- `IEmailService`
- `IIntegrationEventPublisher`

Recommended first commands:

- `CreateUserCommand`
- `LoginCommand`
- `CreatePersonalEventCommand`
- `CreateGroupEventCommand`
- `SendFriendshipRequestCommand`
- `AcceptFriendshipRequestCommand`
- `SendInvitationCommand`
- `AcceptInvitationCommand`

Recommended pattern:

```text
Request DTO -> Command -> Validator -> Handler -> Domain model -> Repository -> UnitOfWork
```

## 4. Add Contracts

The contracts project contains API request and response DTOs.

Examples:

- `RegisterRequest`
- `LoginRequest`
- `CreatePersonalEventRequest`
- `PersonalEventResponse`
- `CreateGroupEventRequest`
- `GroupEventResponse`
- `InvitationResponse`
- `FriendshipRequestResponse`
- `UserResponse`

Keep contracts separate from domain entities. API models should not leak domain internals.

## 5. Add Persistence

Install EF Core packages:

```bash
dotnet add EventReminder.Persistence package Microsoft.EntityFrameworkCore --version 8.0.6
dotnet add EventReminder.Persistence package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.6
dotnet add EventReminder.Services.Api package Microsoft.EntityFrameworkCore.Tools --version 8.0.6
```

Implement:

- `EventReminderDbContext`
- entity configurations
- repositories
- `UnitOfWork`
- migrations

Start with tables in this order:

1. `Users`
2. `PersonalEvents`
3. `GroupEvents`
4. `FriendshipRequests`
5. `Friendships`
6. `Invitations`
7. `Attendees`
8. `Notifications`

Add migrations:

```bash
dotnet ef migrations add Create_Database \
  --project EventReminder.Persistence \
  --startup-project EventReminder.Services.Api
```

Apply migrations:

```bash
dotnet ef database update \
  --project EventReminder.Persistence \
  --startup-project EventReminder.Services.Api
```

## 6. Add Infrastructure

The infrastructure layer implements external concerns.

Install packages:

```bash
dotnet add EventReminder.Infrastructure package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.6
dotnet add EventReminder.Infrastructure package RabbitMQ.Client --version 6.8.1
dotnet add EventReminder.Infrastructure package MailKit --version 4.6.0
dotnet add EventReminder.Application package Newtonsoft.Json --version 13.0.3
```

Implement:

- `JwtProvider`
- `PasswordHasher`
- `EmailService`
- `IntegrationEventPublisher`
- `MachineDateTime`

Register these services in `DependencyInjection.cs`.

## 7. Build The API

Install packages:

```bash
dotnet add EventReminder.Services.Api package FluentValidation.AspNetCore --version 11.3.0
dotnet add EventReminder.Services.Api package Microsoft.EntityFrameworkCore.Tools --version 8.0.6
dotnet add EventReminder.Services.Api package Swashbuckle.AspNetCore --version 6.6.2
```

Start with these controllers:

- `AuthenticationController`
- `UsersController`
- `PersonalEventsController`
- `GroupEventsController`
- `FriendshipRequestsController`
- `InvitationsController`
- `FriendshipsController`

Recommended endpoint order:

1. `POST /authentication/register`
2. `POST /authentication/login`
3. `GET /users/{id}`
4. `POST /personal-events`
5. `GET /personal-events`
6. `POST /group-events`
7. `GET /group-events`
8. `POST /friendship-requests`
9. `POST /friendship-requests/{id}/accept`
10. `POST /group-events/{id}/invite`
11. `POST /invitations/{id}/accept`

Add API concerns:

- Swagger
- JWT authentication
- authorization
- global exception middleware
- validation pipeline behavior
- automatic migration on startup

## 8. Add Notifications Service

Build this after the API works.

Install packages:

```bash
dotnet add EventReminder.BackgroundTasks package Microsoft.Extensions.Hosting --version 8.0.0
```

Add background services:

- `PersonalEventNotificationsProducerBackgroundService`
- `GroupEventNotificationsProducerBackgroundService`
- `EmailNotificationConsumerBackgroundService`
- `IntegrationEventConsumerBackgroundService`

RabbitMQ queues should be durable:

```csharp
channel.QueueDeclare(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false);
```

Persistent messages:

```csharp
IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true;

channel.BasicPublish(
    exchange: string.Empty,
    routingKey: queueName,
    basicProperties: properties,
    body: body);
```

## 9. Add Docker Compose

Start with infrastructure:

- SQL Server
- RabbitMQ

Then add:

- API service
- Notifications service

Use Docker service names inside containers:

```text
SQL Server host: event-reminder-db
RabbitMQ host: event-reminder-mq
```

Expose useful ports:

```text
API: http://localhost:5000
Notifications: http://localhost:6000
RabbitMQ UI: http://localhost:15672
SQL Server: localhost:1433
```

Add health checks so the API and notifications service wait for SQL Server and RabbitMQ.

## 10. Suggested Feature Order

Build the app in this order:

1. Register user
2. Login and JWT
3. Get user profile
4. Create personal event
5. List personal events
6. Create group event
7. List group events
8. Send friendship request
9. Accept friendship request
10. Send group event invitation
11. Accept or reject invitation
12. Add attendee tracking
13. Create notification records
14. Publish integration events
15. Consume integration events
16. Send email notifications
17. Run everything with Docker Compose

## 11. Recommended Workflow Per Feature

For each feature, implement from the outside in:

```text
Contract
Command or Query
Validator
Handler
Domain behavior
Repository method
Controller endpoint
Manual API test
```

Example for registration:

```text
RegisterRequest
CreateUserCommand
CreateUserCommandValidator
CreateUserCommandHandler
User.Create(...)
IUserRepository.Insert(...)
AuthenticationController.Register(...)
```

## 12. Manual Test Checklist

After each feature:

- Does the project build?
- Does the endpoint appear in Swagger?
- Does validation reject bad input?
- Does the database row get created or updated correctly?
- Does the handler return useful errors?
- Does authentication work for protected endpoints?
- Does Docker Compose still start cleanly?

## 13. Final Milestone

When the rebuild is complete, you should be able to run:

```bash
docker compose up --build
```

Then open:

```text
http://localhost:5000/swagger/index.html
http://localhost:15672
```

At that point, you will have rebuilt the application through domain modeling, CQRS use cases, EF Core persistence, JWT authentication, RabbitMQ messaging, background workers, and Docker Compose.
