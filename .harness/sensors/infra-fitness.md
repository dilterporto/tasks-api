# Sensor: Infrastructure Fitness

These checks verify that the Terraform infrastructure implementation satisfies the invariants defined in the architecture (ADR-005) and spec #10. Run them after any change to files under `infra/`.

---

## 1. Structural Validation

Verify that all Terraform modules are syntactically and semantically valid.

### Formatting

```bash
terraform fmt -check -recursive infra/
```

Expected output: no output (all files already formatted). Any output names files that need `terraform fmt`.

### Module validation

Run inside each module directory:

```bash
for dir in infra/modules/*/; do
  echo "==> $dir"
  terraform -chdir="$dir" validate
done
```

Expected output: `Success! The configuration is valid.` for each module.

_Maps to spec #10: `terraform validate` passes for all modules (AC-1)._

---

## 2. Security & Credentials

Verify that secrets are never hardcoded and IAM follows least-privilege.

### No plaintext secrets in .tf files

```bash
grep -rEn '(password|secret_key|access_key|db_pass)\s*=\s*"[^$][^"]*"' infra/ --include="*.tf"
```

Expected output: empty. Any match is a violation — the value must come from a variable or data source.

### No wildcard IAM actions

```bash
grep -rEn 'actions\s*=\s*\["\*"\]|"Action"\s*:\s*"\*"' infra/ --include="*.tf"
```

Expected output: empty. Task role and execution role must use specific action ARNs.

### prod.tfvars not committed

```bash
git ls-files infra/tfvars/prod.tfvars
```

Expected output: empty. Only `prod.tfvars.example` may be tracked. If this returns a path, remove it from the index.

### ECS uses Secrets Manager for DB credentials (not plain environment)

```bash
# Check that instance_class attribute in resource blocks is not hardcoded
grep -rEn '^\s*instance_class\s*=\s*"db\.(t[0-9]|m[0-9]|r[0-9])' infra/modules/rds/ --include="*.tf"
```

Wait — this check belongs to Module Parameterization. For credentials:

```bash
# DB password must not appear as a plain "value" in environment blocks (only as valueFrom in secrets blocks)
grep -rEn '"value"\s*:\s*".*[Pp]assword|"value"\s*:\s*"Host=' infra/ --include="*.tf"
```

Expected output: empty. Connection string references must appear inside a `secrets` block in the task definition (using `valueFrom`), not in a plain `environment` block with a `value` key.

_Maps to spec #10: DB connection string stored in Secrets Manager (AC-5); no hardcoded credentials (Must not); least-privilege IAM (Should)._

---

## 3. Module Parameterization

Verify that modules contain no environment-specific values — all configuration enters via variables.

### No hardcoded AWS regions inside modules

```bash
grep -rEn '"[a-z]+-[a-z]+-[0-9]"' infra/modules/ --include="*.tf"
```

Expected output: empty. Region must be passed as a variable (e.g., `var.aws_region`), not hardcoded.

### No hardcoded RDS instance classes in resource blocks inside modules

```bash
grep -rEn '^\s*instance_class\s*=\s*"db\.(t[0-9]|m[0-9]|r[0-9])' infra/modules/rds/ --include="*.tf"
```

Expected output: empty. Resource blocks must use `var.db_instance_class`; variable `default` values are allowed.

### No hardcoded ECS CPU/memory inside modules

```bash
grep -rEn '^\s*(cpu|memory)\s*=\s*[0-9]+' infra/modules/ecs/ --include="*.tf"
```

Expected output: empty. CPU and memory must be `var.task_cpu` and `var.task_memory`.

_Maps to spec #10: Must not hardcode environment-specific values in `.tf` files (Must not)._

---

## 4. Pipeline & Documentation

Verify that the CD workflow, CLAUDE.md, and README.md are updated as required by the spec.

### CD workflow runs terraform apply on main

```bash
grep -n "terraform apply" .github/workflows/cd.yml
```

Expected output: at least one line containing `terraform apply`. The step must be scoped to pushes to `main`.

### CLAUDE.md has infra commands section

```bash
grep -n "terraform" CLAUDE.md
```

Expected output: at least one line. An **Infra** section with `terraform init`, `plan`, and `apply` commands must be present.

### README.md references prod architecture

```bash
grep -nE "API Gateway|ECS|Fargate|RDS" README.md
```

Expected output: at least one line. The prod architecture diagram and deployment instructions must be present.

_Maps to spec #10: CD workflow updated (AC-7); CLAUDE.md updated (AC-8); README.md updated (AC-9)._

---

## Automated enforcement

These checks are designed to run as an `infra-sensor` job in `.github/workflows/ci.yml` once the `infra/` directory exists (implemented in issue #10). Until then, run the commands above manually when reviewing an infrastructure PR.

The `infra-sensor` CI job is a required status check for PRs that touch files under `infra/`, `docker-compose.dev-env.yml`, or `.github/workflows/cd.yml`.
