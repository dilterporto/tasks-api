# Spec: Terraform infrastructure for ECS Fargate + API Gateway + RDS PostgreSQL with multi-environment support

**Issue:** #10  
**Author:** @dilterporto  
**Status:** Ready  
**Agent:** infra-engineer  

---

## Context

The Tasks API has a functional CD pipeline that builds and pushes a Docker image to ECR but cannot deploy — the ECS cluster, service, RDS instance, and API Gateway do not exist yet. Without this infrastructure:

- Production deployments require manual AWS console work (not reproducible, not auditable)
- Local development cannot mirror the prod topology
- The CD pipeline is scaffolded but effectively a no-op after the ECR push step

This spec provisions the full cloud infrastructure using Terraform following ADR-005.

---

## Specification

### What will be built

A modular Terraform configuration under `infra/` that provisions all AWS resources required to run the Tasks API in production, and emulates the same topology locally via LocalStack — using identical Terraform modules with environment-specific variable overrides.

### Inputs and outputs

```
Input:  tfvars file (local.tfvars | prod.tfvars) + AWS credentials (prod only)
Output: Running Tasks API reachable via API Gateway HTTPS endpoint
        ECS service with Tasks.Api container
        RDS PostgreSQL in private subnet
        ECR repository for container images
        CloudWatch log group
        IAM roles (task execution + task role)
        Secrets Manager secret (DB credentials)
```

### Behavior

- **Must** provision a dedicated VPC with public and private subnets across 2 AZs
- **Must** expose the API exclusively through API Gateway (HTTP API) — no direct ECS public IP
- **Must** route API Gateway → ECS via VPC Link (private integration)
- **Must** store the DB connection string in AWS Secrets Manager and inject it into the ECS task definition as an environment variable — never hardcoded in task definition JSON
- **Must** store Terraform remote state in S3 + DynamoDB lock table for `prod`
- **Must** support a `local` environment via LocalStack with no real AWS credentials required
- **Must** update `docker-compose.dev-env.yml` to include a LocalStack service
- **Must** update the CD workflow (`.github/workflows/cd.yml`) to run `terraform apply` for `prod` on merge to `main`
- **Must** update `CLAUDE.md` with infra commands (`terraform init`, `plan`, `apply`)
- **Must** update `README.md` with the prod architecture diagram and deployment instructions
- **Must not** hardcode environment-specific values (region, instance class, image tag) in `.tf` files — all must be variables
- **Must not** commit `prod.tfvars` — only `prod.tfvars.example` is committed
- **Should** use `db.t3.micro` as the default RDS instance class (overridable via variable)
- **Should** use `256 CPU / 512 MB` as the default ECS task size (overridable via variable)
- **Should** apply least-privilege IAM policies — no wildcard `*` actions in task role
- **Should** wire the existing `Redis__Server` env var to ElastiCache (if provisioned); Redis provisioning is out of scope for this issue

---

## Acceptance Criteria

- [ ] AC-1: `terraform plan` exits 0 against LocalStack using `tfvars/local.tfvars`
- [ ] AC-2: `terraform apply` provisions all components in LocalStack with no manual steps
- [ ] AC-3: `terraform plan` exits 0 against real AWS using `tfvars/prod.tfvars`
- [ ] AC-4: API Gateway endpoint can reach the ECS service end-to-end (health check returns 200)
- [ ] AC-5: ECS task reads the DB connection string from Secrets Manager (not from plain env var in task definition)
- [ ] AC-6: `docker-compose.dev-env.yml` includes the LocalStack service and exposes port 4566
- [ ] AC-7: CD workflow runs `terraform apply -var-file=tfvars/prod.tfvars` on merge to `main`, using GitHub secrets for AWS credentials
- [ ] AC-8: `CLAUDE.md` documents `terraform init`, `terraform plan`, and `terraform apply` under an **Infra** commands section
- [ ] AC-9: `README.md` includes the prod architecture diagram and step-by-step deployment instructions

---

## Technical Constraints

- **Terraform version:** `>= 1.6`
- **AWS provider version:** `~> 5.0` — same provider for both `local` and `prod`; LocalStack uses `endpoint_url = http://localhost:4566`
- **Module structure:** one module per resource group (`networking`, `ecs`, `rds`, `api-gateway`, `ecr`) under `infra/modules/`
- **No mixed environments:** module code must not contain `if local then ... else ...` blocks — use variables only
- **State isolation:** `local` uses local state file; `prod` uses S3 backend with DynamoDB locking
- **Secrets:** DB credentials provisioned by Terraform → stored in Secrets Manager → referenced in ECS task definition via `secrets:` block, not `environment:` block
- **No application code changes:** this spec touches only `infra/`, `docker-compose.dev-env.yml`, `.github/workflows/cd.yml`, `CLAUDE.md`, and `README.md`

---

## Out of Scope

- ElastiCache Redis provisioning (wire the env var if Redis already exists, but do not provision it)
- Multi-region deployment
- Blue/green or canary deployment strategies
- WAF, CloudFront, or custom domain (ACM certificate)
- RDS read replicas or Multi-AZ for the initial deployment
- Auto-scaling policies for ECS service

---

## Sensors

Full sensor definitions and runnable commands are in `.harness/sensors/infra-fitness.md`. Checklist below maps each AC to its sensor group.

### Structural Validation (AC-1, AC-2)

```bash
# All modules format-clean
terraform fmt -check -recursive infra/

# All modules valid
for dir in infra/modules/*/; do terraform -chdir="$dir" validate; done
```

Expected: no output from fmt; `Success! The configuration is valid.` per module.

### Security & Credentials (AC-5)

```bash
# No plaintext secrets
grep -rEn '(password|secret_key|access_key|db_pass)\s*=\s*"[^$][^"]*"' infra/ --include="*.tf"

# No wildcard IAM actions
grep -rEn 'actions\s*=\s*\["\*"\]|"Action"\s*:\s*"\*"' infra/ --include="*.tf"

# prod.tfvars not committed
git ls-files infra/tfvars/prod.tfvars

# ECS uses secrets: block (not environment:) for DB credentials
grep -rn 'CONNECTIONSTRINGS\|ConnectionStrings\|DB_PASSWORD\|DB_PASS' infra/ --include="*.tf" | grep -v 'secrets'
```

Expected: all commands return empty output.

### Module Parameterization (Must not hardcode env-specific values)

```bash
# No hardcoded AWS regions inside modules
grep -rEn '"[a-z]+-[a-z]+-[0-9]"' infra/modules/ --include="*.tf"

# No hardcoded RDS instance class inside modules
grep -rEn '"db\.(t[0-9]|m[0-9]|r[0-9])\.' infra/modules/ --include="*.tf"

# No hardcoded ECS CPU/memory inside modules
grep -rEn '^\s*(cpu|memory)\s*=\s*[0-9]+' infra/modules/ecs/ --include="*.tf"
```

Expected: all commands return empty output.

### Pipeline & Documentation (AC-7, AC-8, AC-9)

```bash
# CD workflow applies terraform on main
grep -n "terraform apply" .github/workflows/cd.yml

# CLAUDE.md has infra commands section
grep -n "terraform" CLAUDE.md

# README.md references prod architecture
grep -nE "API Gateway|ECS|Fargate|RDS" README.md
```

Expected: each command returns at least one matching line.

### Architecture isolation

```bash
# No changes to src/ or tests/
git diff --name-only main | grep -E '^(src|tests)/'
```

Expected: empty output.
