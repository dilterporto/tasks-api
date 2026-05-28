aws_region        = "us-east-1"
name              = "tasks-api"
container_image   = "tasks-api:latest"
redis_endpoint    = "redis-stack:6379"
db_instance_class = "db.t3.micro"
task_cpu          = 256
task_memory       = 512
