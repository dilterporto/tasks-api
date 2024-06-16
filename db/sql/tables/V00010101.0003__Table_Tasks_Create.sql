CREATE TABLE tasks (
    id UUID PRIMARY KEY NOT NULL,
    user_id UUID NOT NULL,
    at TIMESTAMP DEFAULT NOW(),
    started_at TIMESTAMP,
    completed_at TIMESTAMP,
    subject VARCHAR(255) NOT NULL,
    description TEXT,    
    status VARCHAR(50) DEFAULT 'pending'
)