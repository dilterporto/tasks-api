resource "random_password" "db" {
  length  = 32
  special = false
}

resource "aws_secretsmanager_secret" "db_credentials" {
  name                    = "tasks-api/${var.environment}/db-credentials"
  recovery_window_in_days = var.recovery_window_in_days

  tags = {
    Environment = var.environment
  }
}

resource "aws_db_subnet_group" "this" {
  name       = "${var.name}-db-subnet-group"
  subnet_ids = var.subnet_ids

  tags = {
    Name        = "${var.name}-db-subnet-group"
    Environment = var.environment
  }
}

resource "aws_db_parameter_group" "this" {
  name_prefix = "${var.name}-pg15-"
  family      = "postgres15"

  tags = {
    Name        = "${var.name}-pg15"
    Environment = var.environment
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_db_instance" "this" {
  identifier        = "${var.name}-postgres"
  engine            = "postgres"
  engine_version    = "15"
  instance_class    = var.instance_class
  allocated_storage = var.allocated_storage
  storage_type      = "gp3"

  db_name  = var.db_name
  username = var.db_username
  password = random_password.db.result

  db_subnet_group_name   = aws_db_subnet_group.this.name
  parameter_group_name   = aws_db_parameter_group.this.name
  vpc_security_group_ids = [var.security_group_id]

  skip_final_snapshot = true
  publicly_accessible = false

  tags = {
    Name        = "${var.name}-postgres"
    Environment = var.environment
  }
}

resource "aws_secretsmanager_secret_version" "db_credentials" {
  secret_id = aws_secretsmanager_secret.db_credentials.id
  secret_string = jsonencode({
    username         = var.db_username
    password         = random_password.db.result
    host             = aws_db_instance.this.address
    port             = aws_db_instance.this.port
    dbname           = var.db_name
    connectionString = "Host=${aws_db_instance.this.address};Port=${aws_db_instance.this.port};Database=${var.db_name};Username=${var.db_username};Password=${random_password.db.result}"
  })

  depends_on = [aws_db_instance.this]
}
