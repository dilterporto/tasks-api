variable "aws_region" {
  description = "AWS region used for LocalStack"
  type        = string
  default     = "us-east-1"
}

variable "name" {
  description = "Name prefix for all resources"
  type        = string
  default     = "tasks-api"
}

variable "vpc_cidr" {
  description = "VPC CIDR block"
  type        = string
  default     = "10.0.0.0/16"
}

variable "container_image" {
  description = "Docker image for the ECS task"
  type        = string
  default     = "tasks-api:latest"
}

variable "container_port" {
  description = "Port the container listens on"
  type        = number
  default     = 8080
}

variable "task_cpu" {
  description = "ECS task CPU units"
  type        = number
  default     = 256
}

variable "task_memory" {
  description = "ECS task memory in MiB"
  type        = number
  default     = 512
}

variable "db_instance_class" {
  description = "RDS instance class"
  type        = string
  default     = "db.t3.micro"
}

variable "db_name" {
  description = "PostgreSQL database name"
  type        = string
  default     = "tasksdb"
}

variable "db_username" {
  description = "PostgreSQL master username"
  type        = string
  default     = "tasksadmin"
}

variable "redis_endpoint" {
  description = "Redis endpoint (host:port) — points to docker-compose redis-stack in local env"
  type        = string
  default     = "redis-stack:6379"
}
