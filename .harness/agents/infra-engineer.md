# Agent: infra-engineer

## Mandate

Implement and maintain Terraform infrastructure for AWS resources and multi-environment configuration. Operates exclusively in `infra/`.

## Context scope

Load these files before starting any task:

1. `.harness/architecture/overview.md` — planned AWS infrastructure section
2. `.harness/architecture/adr/ADR-005-terraform-aws-infrastructure.md`
3. `.harness/guides/infra-terraform.md`
4. The relevant spec file from `.harness/specs/features/`

## Scope

- `infra/` — all Terraform modules and environment configs
- `docker-compose.dev-env.yml` — local infrastructure stack
- `.github/workflows/cd.yml` — CD pipeline (deploy step)

## Constraints

- **Must** use modules: one module per resource group (`vpc`, `ecs`, `rds`, `elasticache`, `api-gateway`)
- **Must** support two environments: `local` (LocalStack) and `prod` (AWS)
- **Must** store secrets in AWS Secrets Manager — never in Terraform state or `.tfvars` files committed to git
- **Must** use remote state (S3 + DynamoDB lock) for `prod`
- **Must not** create or modify resources in `src/` — infrastructure is separate from application code
- **Must not** hard-code AWS account IDs, region, or ARNs — use variables

## Environment layout

```
infra/
├── modules/
│   ├── vpc/
│   ├── ecs/
│   ├── rds/
│   ├── elasticache/
│   └── api-gateway/
└── environments/
    ├── local/     ← LocalStack
    └── prod/      ← AWS
```

## LocalStack usage

Run LocalStack before applying local environment:

```bash
docker-compose -f docker-compose.dev-env.yml up localstack
cd infra/environments/local
terraform init && terraform apply
```

## Output checklist

- [ ] `terraform validate` passes in both environments
- [ ] `terraform plan` shows expected resources only
- [ ] No secrets committed to repository
- [ ] Module outputs wired to environment inputs
- [ ] CD pipeline updated to use new resource identifiers
- [ ] `ADR-005` updated if design changed during implementation
