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
  # LocalStack ignores real credentials; set AWS_ACCESS_KEY_ID=test AWS_SECRET_ACCESS_KEY=test in your shell
  region                      = var.aws_region
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    ec2                  = "http://localhost:4566"
    ecs                  = "http://localhost:4566"
    rds                  = "http://localhost:4566"
    apigateway           = "http://localhost:4566"
    apigatewayv2         = "http://localhost:4566"
    secretsmanager       = "http://localhost:4566"
    iam                  = "http://localhost:4566"
    logs                 = "http://localhost:4566"
    elbv2                = "http://localhost:4566"
    elasticloadbalancing = "http://localhost:4566"
  }
}

module "vpc" {
  source = "../../modules/vpc"

  name           = var.name
  environment    = "local"
  cidr_block     = var.vpc_cidr
  container_port = var.container_port
}

module "rds" {
  source = "../../modules/rds"

  name              = var.name
  environment       = "local"
  subnet_ids        = module.vpc.private_subnet_ids
  security_group_id = module.vpc.rds_security_group_id
  instance_class    = var.db_instance_class
  db_name           = var.db_name
  db_username       = var.db_username
}

module "ecs" {
  source = "../../modules/ecs"

  name              = var.name
  environment       = "local"
  aws_region        = var.aws_region
  vpc_id            = module.vpc.vpc_id
  subnet_ids        = module.vpc.private_subnet_ids
  security_group_id = module.vpc.ecs_security_group_id
  container_image   = var.container_image
  container_port    = var.container_port
  task_cpu          = var.task_cpu
  task_memory       = var.task_memory
  db_secret_arn     = module.rds.secret_arn
  redis_endpoint    = var.redis_endpoint
}

module "api_gateway" {
  source = "../../modules/api-gateway"

  name                          = var.name
  environment                   = "local"
  subnet_ids                    = module.vpc.private_subnet_ids
  api_gateway_security_group_id = module.vpc.api_gateway_security_group_id
  nlb_listener_arn              = module.ecs.nlb_listener_arn
}
