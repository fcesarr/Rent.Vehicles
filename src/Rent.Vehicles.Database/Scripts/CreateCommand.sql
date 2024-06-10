CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Command (
    Id UUID PRIMARY KEY,
    SagaId UUID NOT NULL,
    Type VARCHAR(255) NOT NULL,  -- Adjust this type based on ActionType's underlying type
    Data BYTEA NOT NULL,
    Created TIMESTAMPTZ NOT NULL,
    SerializerType VARCHAR(255) NOT NULL  -- Adjust this type based on SerializerType's underlying type
);