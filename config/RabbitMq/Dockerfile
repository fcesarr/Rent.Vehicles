FROM rabbitmq:3.13.3-management-alpine

COPY ./config/RabbitMq/rabbitmq.conf etc/rabbitmq/rabbitmq.conf

COPY ./config/RabbitMq/definitions.json /etc/rabbitmq/definitions.json

RUN rabbitmq-plugins enable rabbitmq_amqp1_0