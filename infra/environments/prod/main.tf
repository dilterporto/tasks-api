terraform {
  required_version = ">= 1.6"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

module "vpc" {
  source = "../../modules/vpc"

  name           = var.name
  environment    = "prod"
  cidr_block     = var.vpc_cidr
  container_port = var.container_port
}

module "rds" {
  source = "../../modules/rds"

  name              = var.name
  environment       = "prod"
  subnet_ids        = module.vpc.private_subnet_ids
  security_group_id = module.vpc.rds_security_group_id
  instance_class    = var.db_instance_class
  db_name           = var.db_name
  db_username       = var.db_username
}

module "ecs" {
  source = "../../modules/ecs"

  name                  = var.name
  environment           = "prod"
  aws_region            = var.aws_region
  vpc_id                = module.vpc.vpc_id
  subnet_ids            = module.vpc.private_subnet_ids
  alb_subnet_ids        = module.vpc.public_subnet_ids
  alb_security_group_id = module.vpc.alb_security_group_id
  security_group_id     = module.vpc.ecs_security_group_id
  container_image       = var.container_image
  migrations_image      = var.migrations_image
  container_port        = var.container_port
  task_cpu              = var.task_cpu
  task_memory           = var.task_memory
  db_secret_arn         = module.rds.secret_arn
  redis_endpoint        = var.redis_endpoint
}
