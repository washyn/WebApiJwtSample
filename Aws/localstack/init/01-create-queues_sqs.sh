#!/bin/bash

echo "Creando colas SQS..."

awslocal sqs create-queue --queue-name mi-cola-1
awslocal sqs create-queue --queue-name mi-cola-2

echo "Colas creadas"