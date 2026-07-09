# Spec: Bootstrap CD pipeline with ECR provisioning and two-phase Terraform deploy

**Issue:** #35  
**Author:** @dilterporto  
**Status:** Ready  
**Agent:** infra-engineer  

---

## Context

The CD pipeline (`cd.yml`) was scaffolded expecting ECR to already exist before the first deploy. On a fresh AWS account there is no ECR repository, so the pipeline fails at the Docker push step before Terraform has provisioned anything. There is also no way to trigger the pipeline manually — `workflow_dispatch` was not configured — forcing a dummy commit to main to retry a failed deploy.

These two gaps block the first-ever deploy of the application to AWS.

---

## Specification

### What will be built

- An `aws_ecr_repository` resource added to the ECS Terraform module so the container registry is part of the managed infrastructure
- An ECR URL output propagated from the ECS module up to the prod environment
- A restructured `cd.yml` that provisions ECR first (via targeted `terraform apply`), then builds and pushes the image, then runs a full `terraform apply` — making the pipeline safe to run on a fresh account
- A `workflow_dispatch` trigger on `cd.yml` so deploys can be triggered manually from the GitHub Actions UI
- Replacement of the ad-hoc Terraform shell install with `hashicorp/setup-terraform@v3`

### Inputs and outputs

```
Input:  GitHub Actions trigger (push to main OR manual workflow_dispatch)
        GitHub Secrets: AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, AWS_REGION
        Git SHA: github.sha (used as Docker image tag)

Output: ECR repository provisioned in AWS
        Docker image pushed to ECR as <sha> and latest
        All AWS infrastructure (VPC, RDS, ECS, API Gateway) provisioned/updated
        ECS task definition updated to the new image SHA
```

### Behavior

- **Must** create `aws_ecr_repository` in the ECS Terraform module before the ECS task definition depends on it
- **Must** expose the ECR repository URL as a Terraform output (`ecr_repository_url`) at module and environment level
- **Must** run `terraform apply -target=module.ecs.aws_ecr_repository.this` before the Docker build step so ECR always exists when pushing
- **Must** derive the ECR URL from Terraform output — not from a hardcoded secret or env var
- **Must** tag the Docker image with `github.sha` and `latest`
- **Must** run a second full `terraform apply` after the image push, passing the SHA-tagged image URI as `TF_VAR_container_image`
- **Must** support `workflow_dispatch` so the pipeline can be triggered manually without a code push
- **Must** use `hashicorp/setup-terraform@v3` instead of a manual shell install
- **Must not** require an `ECR_REPOSITORY` GitHub secret — the repository name comes from Terraform
- **Should** pass `terraform_wrapper: false` to `setup-terraform` so `terraform output` returns raw values

---

## Acceptance Criteria

- [ ] AC-1: A `terraform plan` on a fresh state creates an `aws_ecr_repository` resource named `tasks-api`
- [ ] AC-2: `terraform output ecr_repository_url` returns a non-empty URL after apply
- [ ] AC-3: The CD workflow succeeds end-to-end on a fresh AWS account (no pre-existing ECR)
- [ ] AC-4: The CD workflow can be triggered manually from GitHub Actions → CD → Run workflow
- [ ] AC-5: The ECS task definition in AWS references the image tagged with `github.sha`, not `placeholder:latest`
- [ ] AC-6: The `ECR_REPOSITORY` GitHub secret is not referenced anywhere in the workflow

---

## Technical Constraints

- Layer: `infra/modules/ecs/` (new resource + output), `infra/environments/prod/outputs.tf` (propagate output), `.github/workflows/cd.yml` (workflow restructure)
- Must not modify application source code or CI (`ci.yml`)
- Must not store the ECR URL as a GitHub secret — it must flow from Terraform outputs
- `terraform apply -target` is acceptable here only for the bootstrap step; the second apply must be target-free

---

## Out of Scope

- ElastiCache / Redis provisioning (tracked separately)
- Multi-environment support (staging) — prod only
- Terraform state backend bootstrap (S3 bucket + DynamoDB lock table) — assumed to exist or handled manually
- CD dependency on CI passing (`needs: build-and-test`) — separate improvement

---

## Sensors

- [ ] Terraform: `terraform validate` passes in `infra/environments/prod`
- [ ] Terraform: `terraform plan` output includes `aws_ecr_repository.this` to be created
- [ ] Workflow: `workflow_dispatch` appears in `cd.yml` triggers
- [ ] Workflow: `hashicorp/setup-terraform@v3` is used (no manual apt-get install of terraform)
- [ ] Workflow: no reference to `secrets.ECR_REPOSITORY` in `cd.yml`
- [ ] Workflow: `terraform apply -target=module.ecs.aws_ecr_repository.this` precedes the docker build step
- [ ] Workflow: `TF_VAR_container_image` uses the ECR URL from Terraform output + `github.sha`
