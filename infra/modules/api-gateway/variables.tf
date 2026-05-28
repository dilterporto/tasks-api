variable "name" {
  description = "Name prefix for all resources"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "subnet_ids" {
  description = "Private subnet IDs for the VPC Link"
  type        = list(string)
}

variable "api_gateway_security_group_id" {
  description = "Security group ID attached to the VPC Link"
  type        = string
}

variable "nlb_listener_arn" {
  description = "Internal NLB listener ARN — target of the HTTP_PROXY integration"
  type        = string
}
