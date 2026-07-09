# ADR-005: Terraform for AWS infrastructure

**Status:** Proposed  
**Date:** 2024-06-01

## Context

The Tasks API needs a production-grade hosting environment on AWS. The infrastructure must support local development (without AWS costs), and multiple environments must share the same configuration with environment-specific overrides.

Manual click-ops in the AWS console is not reproducible, auditable, or version-controlled. An Infrastructure-as-Code approach is required.

## Decision

Use Terraform with a module-per-resource-group structure and two environments: `local` (LocalStack) and `prod` (AWS).

Target architecture:

```
Internet → ALB (public subnets, port 80)
         → ECS Fargate (Tasks.Api container, private subnets)
         → RDS PostgreSQL (private subnet)
         → ElastiCache Redis (private subnet)
```

> **Note:** The original design included API Gateway HTTP API + VPC Link + internal NLB. This was replaced by an internet-facing ALB (issue #37) because the AWS account does not support NLB creation. The API Gateway layer may be reintroduced in a future ADR once NLB is enabled.

- Secrets stored in AWS Secrets Manager, injected into the ECS task definition at runtime
- Remote state in S3 + DynamoDB lock for `prod`
- LocalStack for `local` environment (same Terraform modules, different provider endpoint)
- Container images stored in ECR, built and pushed by the CD pipeline

## Consequences

**Easier:**
- Infrastructure is version-controlled alongside application code
- Local environment mirrors prod topology without AWS costs
- New environments can be created by adding a directory under `infra/environments/`

**Harder:**
- LocalStack has partial API coverage — some AWS features may not be testable locally
- Terraform state drift requires discipline (no manual changes in AWS console)
- First implementation requires non-trivial Terraform and AWS knowledge

## Alternatives considered

- **AWS CDK**: More familiar for .NET developers, but adds a synthesized CloudFormation layer that obscures what is actually deployed
- **Docker Compose only**: Sufficient for local development but not suitable for production
- **Pulumi**: Similar to CDK; adds another language runtime dependency
- **Manual AWS setup**: Not reproducible, not version-controlled, not acceptable

## Implementation

See `.harness/guides/infra-terraform.md` and issue #10.

This ADR transitions to `Accepted` when the `infra/` directory is merged to `main`.
