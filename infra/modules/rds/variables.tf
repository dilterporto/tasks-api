variable "name" {
  description = "Name prefix for all resources"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "subnet_ids" {
  description = "Private subnet IDs for the DB subnet group"
  type        = list(string)
}

variable "security_group_id" {
  description = "Security group ID that controls inbound access to RDS"
  type        = string
}

variable "instance_class" {
  description = "RDS instance class"
  type        = string
  default     = "db.t3.micro"
}

variable "allocated_storage" {
  description = "Allocated storage in GB"
  type        = number
  default     = 20
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

variable "recovery_window_in_days" {
  description = "Number of days Secrets Manager waits before deleting the secret (0 = immediate, prod should use 7)"
  type        = number
  default     = 0
}
