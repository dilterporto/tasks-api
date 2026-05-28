terraform {
  backend "s3" {
    bucket         = "tasks-api-terraform-state"
    key            = "prod/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "tasks-api-terraform-locks"
    encrypt        = true
  }
}
