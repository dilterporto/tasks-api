output "endpoint" {
  description = "RDS instance endpoint"
  value       = aws_db_instance.this.address
}

output "port" {
  description = "RDS instance port"
  value       = aws_db_instance.this.port
}

output "secret_arn" {
  description = "Secrets Manager ARN containing DB credentials and connection string"
  value       = aws_secretsmanager_secret.db_credentials.arn
}

output "db_name" {
  description = "PostgreSQL database name"
  value       = aws_db_instance.this.db_name
}
