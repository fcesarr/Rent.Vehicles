CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE Commands (
    Id UUID PRIMARY KEY,
    SagaId UUID NOT NULL,
    ActionType VARCHAR(255) NOT NULL,  -- Adjust this type based on ActionType's underlying type
    SerializerType VARCHAR(255) NOT NULL,  -- Adjust this type based on SerializerType's underlying type
    EntityType VARCHAR(255) NOT NULL,  -- Adjust this type based on EntityType's underlying type
    Data BYTEA NOT NULL,
    Created TIMESTAMPTZ NOT NULL
);