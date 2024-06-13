CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Events (
    Id UUID PRIMARY KEY,
    SagaId UUID NOT NULL,
    Name VARCHAR(255) NOT null,
    StatusType VARCHAR(255) NOT null,
    Message VARCHAR(255) NOT null,
    Created TIMESTAMPTZ NOT NULL
);