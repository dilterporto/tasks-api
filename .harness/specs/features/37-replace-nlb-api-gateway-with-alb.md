# Spec: Replace NLB + API Gateway with internet-facing ALB

**Issue:** #37  
**Author:** @dilterporto  
**Status:** Ready  
**Agent:** infra-engineer  

---

## Context

The AWS account used for deployment does not support creating Network Load Balancers (ELBv2 NLB restriction on new accounts). The original architecture routed traffic through API Gateway HTTP API → VPC Link → internal NLB → ECS Fargate. This topology is blocked until AWS Support enables NLB on the account.

To unblock production deploys, the ingress layer is simplified: an internet-facing Application Load Balancer replaces the NLB + API Gateway combination. ECS tasks remain in private subnets; only the ALB is public-facing.

---

## Specification

### What will be built

An internet-facing ALB placed in public subnets that forwards HTTP traffic on port 80 to ECS Fargate tasks in private subnets. The API Gateway HTTP API and VPC Link are removed. The `api-gateway` Terraform module is deleted.

New architecture:
```
Internet → ALB (public subnets, port 80) → ECS Fargate (private subnets) → RDS PostgreSQL
```

### Inputs and outputs

```
Input:  HTTP request on port 80 to ALB DNS name
Output: Response from ECS Fargate container

Terraform output: api_url = "http://<alb-dns-name>"
```

### Behavior

- **Must** create an internet-facing ALB in public subnets
- **Must** place ECS tasks in private subnets (no change from before)
- **Must** use a dedicated ALB security group that allows inbound TCP 80 from 0.0.0.0/0
- **Must** update the ECS security group to allow inbound from the ALB security group (not API Gateway SG)
- **Must** configure the ALB target group with HTTP protocol and health check on `/health/live` returning 200
- **Must** expose `alb_dns_name` as a Terraform output from the ECS module
- **Must** expose `api_url` as `http://<alb_dns_name>` from the prod environment
- **Must not** create an NLB or API Gateway VPC Link
- **Must not** reference `api_gateway_security_group_id` anywhere
- **Should** remove the `api-gateway` Terraform module entirely to avoid confusion

---

## Acceptance Criteria

- [ ] AC-1: `terraform plan` shows `aws_lb.this` of type `application` and `internal = false`
- [ ] AC-2: `terraform plan` shows no `aws_apigatewayv2_*` or `aws_lb` of type `network` resources
- [ ] AC-3: ALB health check targets `/health/live` with HTTP protocol
- [ ] AC-4: `terraform output api_url` returns an `http://` URL after apply
- [ ] AC-5: `GET http://<alb_dns_name>/health/live` returns 200 after ECS task starts
- [ ] AC-6: No reference to `secrets.ECR_REPOSITORY` or `nlb_listener_arn` remains in any Terraform file

---

## Technical Constraints

- Layer: `infra/modules/vpc/`, `infra/modules/ecs/`, `infra/environments/prod/`
- ALB must be in public subnets; ECS tasks must remain in private subnets
- Security group chain: Internet → ALB SG (port 80) → ECS SG (container port) → RDS SG (5432)
- Must not add HTTPS/TLS — out of scope for this issue
- Must not modify application source code or CI workflow

---

## Out of Scope

- HTTPS / TLS termination on the ALB
- WAF or DDoS protection
- Custom domain / Route 53
- Restoring API Gateway once NLB is enabled by AWS Support
- ElastiCache / Redis provisioning

---

## Sensors

- [ ] Terraform: `terraform validate` passes in `infra/environments/prod`
- [ ] Terraform: no `aws_apigatewayv2_*` resources in any module
- [ ] Terraform: no `aws_lb` resource with `load_balancer_type = "network"`
- [ ] Terraform: `alb_dns_name` output exists in ECS module
- [ ] Workflow: `api-gateway` module directory does not exist
