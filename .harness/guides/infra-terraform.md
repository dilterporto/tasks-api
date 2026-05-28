# Guide: Terraform Infrastructure

## Overview

Infrastructure is managed with Terraform. Two environments are supported:

| Environment | Provider | Purpose |
|-------------|----------|---------|
| `local` | LocalStack | Local development and CI |
| `prod` | AWS | Production |

## Directory structure

```
infra/
├── modules/
│   ├── vpc/           ← VPC, subnets, security groups
│   ├── ecs/           ← ECS cluster, task definition, service
│   ├── rds/           ← RDS PostgreSQL instance
│   ├── elasticache/   ← Redis cluster
│   └── api-gateway/   ← HTTP API + VPC Link + NLB
└── environments/
    ├── local/
    │   ├── main.tf
    │   ├── variables.tf
    │   └── terraform.tfvars
    └── prod/
        ├── main.tf
        ├── variables.tf
        ├── backend.tf   ← S3 remote state + DynamoDB lock
        └── terraform.tfvars.example
```

## Running locally with LocalStack

Start LocalStack:

```bash
docker-compose -f docker-compose.dev-env.yml up localstack
```

Apply local environment:

```bash
cd infra/environments/local
terraform init
terraform plan
terraform apply
```

LocalStack endpoints are configured via `AWS_ENDPOINT_URL` in the Terraform provider.

## Running against prod

```bash
cd infra/environments/prod
terraform init   # initializes S3 backend
terraform plan
terraform apply
```

Prod requires these environment variables:

```
AWS_ACCESS_KEY_ID
AWS_SECRET_ACCESS_KEY
AWS_REGION
```

## Module contracts

Each module exposes outputs consumed by parent environment configs. Key outputs:

| Module | Key outputs |
|--------|-------------|
| `vpc` | `vpc_id`, `private_subnet_ids`, `public_subnet_ids` |
| `ecs` | `cluster_arn`, `service_name`, `task_definition_arn` |
| `rds` | `endpoint`, `secret_arn` (Secrets Manager) |
| `elasticache` | `endpoint` |
| `api-gateway` | `invoke_url` |

## Secrets

Database credentials are stored in AWS Secrets Manager. The ECS task definition references the secret ARN — credentials are **never** in `.tfvars` or Terraform state as plaintext.

```hcl
resource "aws_secretsmanager_secret" "db_password" {
  name = "tasks-api/${var.environment}/db-password"
}
```

## Architecture (prod)

```
Internet → API Gateway (HTTP API)
         → VPC Link → NLB (internal)
         → ECS Fargate (Tasks.Api container)
         → RDS PostgreSQL (private subnet)
         → ElastiCache Redis (private subnet)
```

The Tasks.Api container image is pushed to ECR by the CD pipeline.

## Adding a new resource

1. Create or update the relevant module in `infra/modules/<name>/`
2. Wire module outputs to both `local` and `prod` environment `main.tf`
3. Document the new resource in `.harness/architecture/overview.md`
4. If a new pattern is introduced, write an ADR

## Constraints

- Variables must have descriptions
- No hard-coded AWS account IDs, ARNs, or region strings
- Remote state required for `prod` (S3 backend + DynamoDB lock table)
- All sensitive variables use `sensitive = true`
