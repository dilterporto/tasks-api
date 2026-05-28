# Plan: #10 — Terraform infrastructure for ECS Fargate + API Gateway + RDS PostgreSQL with multi-environment support

**Spec:** `.harness/specs/features/10-terraform-aws-infrastructure-multi-env.md`  
**Status:** Draft  
**Created:** 2026-05-28

## Tasks

- [ ] **task-1** · agent: `infra-engineer`
  Implement all Terraform modules (`vpc`, `ecs`, `rds`, `api-gateway`) under `infra/modules/` and wire them into both environment roots (`infra/environments/local/` and `infra/environments/prod/`), including IAM roles, Secrets Manager secret, CloudWatch log group, S3 + DynamoDB remote state backend for `prod`, and all variable definitions — ensuring no hardcoded regions, instance classes, or ARNs in module code.
  _depends on: —_
  _covers: AC-3, AC-4, AC-5_

- [ ] **task-2** · agent: `infra-engineer`
  Add LocalStack service to `docker-compose.dev-env.yml` (port 4566) and verify the local environment end-to-end: `terraform init`, `terraform plan`, and `terraform apply` must exit 0 against LocalStack with no manual steps.
  _depends on: task-1_
  _covers: AC-1, AC-2, AC-6_

- [ ] **task-3** · agent: `infra-engineer`
  Update `.github/workflows/cd.yml` with a `terraform apply` step gated to pushes on `main` (using GitHub secrets for AWS credentials), add an **Infra** commands section to `CLAUDE.md`, and update `README.md` with the prod architecture diagram and step-by-step deployment instructions.
  _depends on: task-1_
  _covers: AC-7, AC-8, AC-9_
