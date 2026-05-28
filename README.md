# Tasks API

Task management API built with .NET 8, implementing CQRS, Event Sourcing, and Domain-Driven Design.

## Architecture

Clean Architecture with DDD, separating concerns across four layers:

```
┌──────────────────────────────────────────────┐
│         Presentation (Tasks.Api)             │
│         REST API via FastEndpoints           │
├──────────────────────────────────────────────┤
│        Application (Tasks.Application)       │
│        Commands, Queries, Handlers           │
├──────────────────────────────────────────────┤
│          Domain (Tasks.Domain)               │
│       Aggregates, Events, Repositories       │
├──────────────────────────────────────────────┤
│       Infrastructure (Tasks.Persistence)     │
│    Event Store, Projections, Repositories    │
└──────────────────────────────────────────────┘
```

### Architectural Patterns

- **CQRS** — Commands (write) and Queries (read) handled independently via MediatR
- **Event Sourcing** — State changes persisted as domain events; aggregates reconstructed from event history
- **Read Model** — Dedicated projection table (`tasks`) updated by event committers, optimized for queries
- **Result Pattern** — `CSharpFunctionalExtensions.Result<T>` for explicit error handling
- **Pipeline Behaviors** — Cross-cutting concerns (logging, unit of work, cache invalidation) as MediatR decorators
- **Cache-Aside** — Redis caches upcoming tasks; invalidated after any mutating command

## Project Structure

```
src/
├── Tasks.Api/                  # FastEndpoints, validation, request/response mapping
├── Tasks.Application/          # Use cases as Commands and Queries
│   ├── UseCases/               # CreateTask, ChangeTask, DeleteTask, GetTaskById, GetUpcomingTasks
│   ├── Behaviors/              # UnitOfWork, Logging, CacheValidation pipeline behaviors
│   └── Contracts/              # Response DTOs (TaskResponse, UpcomingTasksResponse)
├── Tasks.Domain/               # TaskAggregate, domain events, ITaskRepository
├── Tasks.Persistence/          # EF Core DbContexts, repositories, event committers, projections
├── Tasks.Abstractions/         # Shared interfaces: ICacheManager, IEventCommitter, AggregateRoot
└── Tasks.DependencyInjection/  # Centralized DI configuration

tests/
└── Tasks.Tests/                # Unit tests for domain and application layers
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/tasks` | Create a task |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `PUT` | `/api/tasks/{id}` | Update task subject, description, or user |
| `DELETE` | `/api/tasks/{id}` | Delete a task |
| `GET` | `/api/tasks/upcoming` | Get upcoming tasks grouped by date (cached) |

### Create Task — `POST /api/tasks`

```json
{
  "userId": "guid",
  "subject": "string",
  "description": "string (max 500 chars)",
  "dueAt": "datetime (must be in the future)"
}
```

### Update Task — `PUT /api/tasks/{id}`

```json
{
  "userId": "guid",
  "subject": "string",
  "description": "string (max 500 chars)"
}
```

## Task Lifecycle

```
Created → Started → Completed
```

Domain events emitted at each transition:

| Event | Trigger |
|-------|---------|
| `TaskCreatedEvent` | Task creation |
| `TaskStartedEvent` | Task started |
| `TaskChangedEvent` | Subject/description/user updated |
| `TaskDeletedEvent` | Task deleted |

## Event Sourcing

Every state change is stored as an event in the `events` table. On reads, the `TaskAggregate` is reconstructed by replaying events in order. The `tasks` projection table is kept in sync by event committers after each write.

```
Command → Aggregate (apply event) → EventsDbContext (event store)
                                  → EventCommitter  → ProjectionsDbContext (read model)
```

## Technology Stack

| Concern | Technology |
|---------|------------|
| Framework | .NET 8, FastEndpoints |
| CQRS | MediatR |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Migrations | Flyway |
| Caching | Redis Stack |
| Logging | Serilog + Seq |
| Object mapping | Mapster |
| Testing | xUnit, Moq, AutoFixture |

## Running Locally

**Requirements:** Git and Docker.

```bash
git clone <repository-url>
cd backend-challenge
docker-compose up
```

### Services

| Service | URL |
|---------|-----|
| API + Swagger | http://localhost:5006/swagger |
| Seq (logs) | http://localhost:5341 |
| RedisInsight | http://localhost:8001 |

### Development environment (infrastructure only)

```bash
docker-compose -f docker-compose.dev-env.yml up
dotnet run --project src/Tasks.Api
```

## Running Tests

```bash
dotnet test tests/Tasks.Tests/
```

## Production Infrastructure

The production environment runs on AWS, provisioned via Terraform:

```
Internet → API Gateway (HTTP API)
         → VPC Link → NLB (internal)
         → ECS Fargate (Tasks.Api container)
         → RDS PostgreSQL (private subnet)
```

### AWS Resources

| Resource | Description |
|----------|-------------|
| API Gateway (HTTP API) | Public entrypoint — routes all traffic via VPC Link |
| VPC Link + NLB | Private integration between API Gateway and ECS |
| ECS Fargate | Serverless container runtime for `Tasks.Api` |
| RDS PostgreSQL | Managed database in private subnet |
| Secrets Manager | Stores DB credentials — injected into ECS at runtime |
| CloudWatch Logs | Container log aggregation (`/ecs/tasks-api`) |

### Deploying to Production

Deployments happen automatically on merge to `main` via `.github/workflows/cd.yml`:

1. Builds and pushes the Docker image to ECR
2. Runs `terraform apply` from `infra/environments/prod/`
3. ECS pulls the new image and performs a rolling update

To deploy manually:

```bash
# Set required environment variables
export AWS_ACCESS_KEY_ID=<key>
export AWS_SECRET_ACCESS_KEY=<secret>
export AWS_REGION=us-east-1

cd infra/environments/prod
cp terraform.tfvars.example terraform.tfvars  # fill in values
terraform init
terraform apply
```

### One-time state backend bootstrap (prod only)

Before the first `terraform init` in `infra/environments/prod/`, the S3 bucket and DynamoDB table referenced in `backend.tf` must exist:

```bash
# Create S3 bucket for Terraform state
aws s3api create-bucket \
  --bucket tasks-api-terraform-state \
  --region us-east-1

aws s3api put-bucket-versioning \
  --bucket tasks-api-terraform-state \
  --versioning-configuration Status=Enabled

aws s3api put-bucket-encryption \
  --bucket tasks-api-terraform-state \
  --server-side-encryption-configuration \
    '{"Rules":[{"ApplyServerSideEncryptionByDefault":{"SSEAlgorithm":"AES256"}}]}'

# Create DynamoDB table for state locking
aws dynamodb create-table \
  --table-name tasks-api-terraform-locks \
  --attribute-definitions AttributeName=LockID,AttributeType=S \
  --key-schema AttributeName=LockID,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --region us-east-1
```

These resources are intentionally outside Terraform management to avoid chicken-and-egg bootstrapping.

### Terraform validation (no cloud required)

The Terraform configuration can be validated offline (no LocalStack or AWS needed):

```bash
cd infra/environments/local
terraform init
terraform validate   # validates all four modules
```

### Terraform plan against LocalStack

LocalStack 3.8 (Community) is used for local development. Start it first:

```bash
docker-compose -f docker-compose.dev-env.yml up localstack
```

Then run the plan:

```bash
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

cd infra/environments/local
terraform init
terraform plan     # exits 0, shows 39 resources
```

> **LocalStack Pro note:** `terraform apply` requires LocalStack Pro for ECS, RDS, Network Load Balancer, and API Gateway v2. Community edition supports only VPC, IAM, Secrets Manager, and CloudWatch Logs. For full stack local testing, set `LOCALSTACK_AUTH_TOKEN` and use LocalStack Pro. For day-to-day development, the `docker-compose.dev-env.yml` app services (Postgres, Redis, Seq) are sufficient without Terraform.
