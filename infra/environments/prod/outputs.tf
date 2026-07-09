output "api_url" {
  description = "ALB public DNS name — API base URL"
  value       = "http://${module.ecs.alb_dns_name}"
}

output "cluster_name" {
  description = "ECS cluster name"
  value       = module.ecs.cluster_name
}

output "service_name" {
  description = "ECS service name"
  value       = module.ecs.service_name
}

output "db_secret_arn" {
  description = "Secrets Manager ARN for DB credentials"
  value       = module.rds.secret_arn
}

output "ecr_repository_url" {
  description = "ECR repository URL for pushing images"
  value       = module.ecs.ecr_repository_url
}

output "migrations_ecr_repository_url" {
  description = "ECR repository URL for the Flyway migrations image"
  value       = module.ecs.migrations_ecr_repository_url
}

output "migrations_task_definition_family" {
  description = "ECS task definition family for Flyway migrations"
  value       = module.ecs.migrations_task_definition_family
}

output "private_subnet_ids" {
  description = "Private subnet IDs — used by CD to run ECS migration task"
  value       = module.vpc.private_subnet_ids
}

output "ecs_security_group_id" {
  description = "ECS security group ID — used by CD to run ECS migration task"
  value       = module.vpc.ecs_security_group_id
}
