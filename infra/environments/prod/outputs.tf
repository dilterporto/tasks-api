output "api_url" {
  description = "API Gateway invoke URL"
  value       = module.api_gateway.invoke_url
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
