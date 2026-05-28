output "api_url" {
  description = "LocalStack API Gateway invoke URL"
  value       = module.api_gateway.invoke_url
}

output "cluster_name" {
  description = "ECS cluster name"
  value       = module.ecs.cluster_name
}

output "db_secret_arn" {
  description = "Secrets Manager ARN for DB credentials"
  value       = module.rds.secret_arn
}
