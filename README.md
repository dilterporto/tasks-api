# Tasks API

A modern, scalable task management API built with .NET 8, implementing advanced architectural patterns for robust and maintainable code.

## 🏗️ Architecture Overview

This project follows a **Clean Architecture** approach with **Domain-Driven Design (DDD)** principles, implementing several key architectural patterns:

### Core Architectural Patterns

- **[CQRS (Command Query Responsibility Segregation)](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)** - Separates read and write operations for better performance and scalability
- **[Event Sourcing](https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)** - Maintains a complete audit trail of all changes through domain events
- **[Domain-Driven Design (DDD)](https://martinfowler.com/bliki/DomainDrivenDesign.html)** - Business logic centered around domain models and aggregates
- **Layered Architecture** - Clear separation of concerns across different layers

### Solution Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│                    (REST API - FastEndpoints)              │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│              (Use Cases as Commands and Queries)                │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│              (Aggregates, Entities, Events)                │
├─────────────────────────────────────────────────────────────┤
│                  Infrastructure Layer                       │
│            (Persistence, Caching, Logging)                 │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Features

The API provides comprehensive task management capabilities:

### Core Operations
- ✅ **Create Task** - Add new tasks with subject, description, and due date
- 🔍 **Get Task By ID** - Retrieve specific task details
- ✏️ **Change Task** - Modify existing task properties
- 🗑️ **Delete Task** - Remove tasks from the system
- 📅 **Get Upcoming Tasks** - Retrieve tasks due in the near future

### Task Management Features
- **Task Status Tracking**: Created → Started → Completed
- **Due Date Management**: Set and track task deadlines
- **User Association**: Tasks are linked to specific users
- **Audit Trail**: Complete history of all task changes through event sourcing

## 🛠️ Technology Stack

### Backend Framework
- **.NET 8** - Latest LTS version with modern C# features
- **FastEndpoints** - High-performance, minimal API framework
- **MediatR** - Mediator pattern implementation for CQRS
- **AutoMapper** - Object-to-object mapping

### Database & Persistence
- **PostgreSQL** - Primary database for event store and projections
- **Flyway** - Database migration management
- **Entity Framework Core** - ORM for data access

### Caching & Performance
- **Redis** - In-memory caching for improved performance
- **Cache-Aside Pattern** - Intelligent caching strategy

### Logging & Monitoring
- **Serilog** - Structured logging framework
- **Seq** - Log aggregation and analysis platform

### Containerization
- **Docker** - Containerized deployment
- **Docker Compose** - Multi-service orchestration

## 🏛️ Project Structure

```
src/
├── Tasks.Api/                 # Presentation Layer
│   ├── Apis/Tasks/           # REST API endpoints
│   ├── Mappings/             # DTO mappings
│   └── Validation/           # Request validation
├── Tasks.Application/         # Application Layer
│   ├── UseCases/             # Business use cases
│   ├── Contracts/            # DTOs and contracts
│   └── Behaviors/            # Cross-cutting concerns
├── Tasks.Domain/             # Domain Layer
│   └── Aggregates/Tasks/     # Task domain model
├── Tasks.Persistence/        # Infrastructure Layer
│   ├── Writing/              # Event store and write operations
│   │   └── EventCommitters/  # Domain event persistence handlers
│   └── Reading/              # Read models and projections
└── Tasks.Abstractions/       # Shared interfaces and contracts
```

## 🔄 CQRS Implementation

### Commands (Write Operations)
- `CreateTaskCommand` - Creates new tasks
- `ChangeTaskCommand` - Modifies existing tasks
- `DeleteTaskCommand` - Removes tasks

### Queries (Read Operations)
- `GetTaskByIdQuery` - Retrieves specific task
- `GetUpcomingTasksQuery` - Fetches upcoming tasks

### Event Sourcing
The system maintains a complete audit trail through domain events:
- `TaskCreatedEvent` - When a task is created
- `TaskChangedEvent` - When a task is modified
- `TaskDeletedEvent` - When a task is removed
- `TaskStartedEvent` - When a task status changes

## 🗄️ Database Schema

### Event Store Tables
- **Events** - Stores all domain events for event sourcing
- **Tasks** - Current task state and projections

### Migration Management
- **Flyway** handles database schema evolution
- Versioned SQL scripts ensure consistent deployments

## 📝 Event Committers (Persistence Layer)

The **Event Committers** are responsible for persisting domain events and updating read models in the Persistence layer. This is a crucial component of the Event Sourcing pattern that ensures data consistency between the event store and projections.

### How Event Committers Work

#### Core Components
- **`EventsCommitter`** - Main orchestrator that routes events to appropriate handlers
- **`IEventCommitter<TEvent>`** - Interface for specific event type handlers
- **`IEventsCommiter`** - Interface for committing multiple events

#### Event Committer Pattern
```csharp
// Each domain event has a dedicated committer
TaskCreatedEventCommitter    → Handles TaskCreatedEvent
TaskChangedEventCommitter    → Handles TaskChangedEvent  
TaskDeletedEventCommitter    → Handles TaskDeletedEvent
TaskStartedEventCommitter    → Handles TaskStartedEvent
```

#### Key Responsibilities
1. **Event Persistence** - Store domain events in the event store
2. **Projection Updates** - Update read models and projections
3. **Data Consistency** - Ensure write and read models stay synchronized
4. **Audit Trail** - Maintain complete history of all state changes

#### Workflow
1. Domain events are raised by aggregates
2. `EventsCommitter.CommitAllAsync()` processes all pending events
3. Events are routed to appropriate type-specific committers
4. Each committer persists the event and updates relevant projections
5. Database transactions ensure atomicity of event storage and projection updates

#### Benefits
- **Separation of Concerns** - Each event type has dedicated handling logic
- **Extensibility** - Easy to add new event types and committers
- **Maintainability** - Clear responsibility boundaries
- **Performance** - Efficient event routing and processing

### 🚀 Hangfire Integration for Background Processing

The system can be extended to use **Hangfire** for background job processing of projection updates, providing:

#### Enhanced Architecture with Hangfire
```
┌─────────────────────────────────────────────────────────────┐
│                    Domain Events                            │
├─────────────────────────────────────────────────────────────┤
│                EventsCommitterWithHangfire                 │
│  ┌─────────────────┐  ┌─────────────────────────────────┐ │
│  │ Event Persistence│  │   Background Job Queueing      │ │
│  │ (Synchronous)   │  │        (Hangfire)              │ │
│  └─────────────────┘  └─────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│              Background Job Processing                     │
│  ┌─────────────────┐  ┌─────────────────────────────────┐ │
│  │ ProjectionUpdate│  │   Retry Logic & Monitoring      │ │
│  │   Handlers      │  │        (Hangfire Dashboard)     │ │
│  └─────────────────┘  └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

#### Key Benefits of Hangfire Integration
- **Asynchronous Projection Updates** - Non-blocking event processing
- **Job Queuing** - Events are queued and processed in background
- **Retry Logic** - Automatic retry on failures with configurable policies
- **Monitoring** - Real-time dashboard for job monitoring and debugging
- **Scalability** - Multiple worker processes can handle projection updates
- **Reliability** - Persistent job storage with PostgreSQL backend

#### Implementation Components
- **`IHangfireEventCommitter<TEvent>`** - Interface for Hangfire-based committers
- **`IProjectionUpdateHandler<TEvent>`** - Background job handlers for projections
- **`EventsCommitterWithHangfire`** - Enhanced committer supporting both sync and async
- **Hangfire Dashboard** - Web interface for monitoring background jobs

#### Configuration
- **PostgreSQL Storage** - Jobs stored in database for persistence
- **Multiple Queues** - Priority-based job processing (default, projection-updates, high-priority)
- **Worker Configuration** - Configurable worker count and server naming
- **Dashboard Access** - Available at `/hangfire` endpoint

## 🚀 Getting Started

### Prerequisites
- **Git** - Source control
- **Docker** - Containerization platform
- **Docker Compose** - Multi-container orchestration

### Quick Start
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd backend-challenge
   ```

2. Start the application:
   ```bash
   docker-compose up
   ```

3. Access the services:
   - **API**: http://localhost:5006
   - **Swagger Documentation**: http://localhost:5006/swagger
   - **Seq Logging**: http://localhost:5341
   - **Redis Management**: http://localhost:8001
   - **Hangfire Dashboard**: http://localhost:5006/hangfire

### Environment Configuration
The application uses environment variables for configuration:
- Database connection strings
- Redis server settings
- Seq logging configuration
- API endpoints and ports

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/tasks` | Create a new task |
| `GET` | `/api/tasks/{id}` | Retrieve task by ID |
| `PUT` | `/api/tasks/{id}` | Update existing task |
| `DELETE` | `/api/tasks/{id}` | Delete a task |
| `GET` | `/api/tasks/upcoming` | Get upcoming tasks |

## 🔧 Development

### Building the Solution
```bash
dotnet build Tasks.sln
```

### Running Tests
```bash
dotnet test tests/Tasks.Tests/
```

### Database Migrations
Database migrations are automatically applied via Flyway when using Docker Compose.

## 🏗️ Design Principles

- **Single Responsibility** - Each class has one reason to change
- **Open/Closed** - Open for extension, closed for modification
- **Dependency Inversion** - High-level modules don't depend on low-level modules
- **Event-Driven** - Loose coupling through domain events
- **Immutable State** - State changes only through defined operations

## 📈 Performance Features

- **Redis Caching** - Reduces database load for frequently accessed data
- **Event Sourcing** - Efficient write operations with full audit trail
- **CQRS Separation** - Optimized read and write operations
- **Async Operations** - Non-blocking I/O operations throughout

## 🔒 Security & Validation

- **Request Validation** - Input validation using FluentValidation
- **Anonymous Access** - Currently configured for development (can be enhanced with authentication)
- **Data Sanitization** - Proper input sanitization and validation

## 🧪 Testing

The solution includes comprehensive testing:
- **Unit Tests** - Domain logic and use cases
- **Integration Tests** - API endpoints and persistence
- **Test Coverage** - Core business logic thoroughly tested

## 📚 Additional Resources

- **Swagger Documentation** - Interactive API documentation
- **Seq Logging** - Centralized logging and debugging
- **Redis Management** - Cache monitoring and management

## 🤝 Contributing

This project demonstrates enterprise-level architecture patterns suitable for:
- Learning advanced .NET patterns
- Understanding CQRS and Event Sourcing
- Implementing clean architecture principles
- Building scalable, maintainable APIs

---

*Built with modern .NET 8 and enterprise architectural patterns for robust, scalable task management.*


