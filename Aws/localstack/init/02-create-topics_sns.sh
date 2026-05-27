#!/bin/bash

echo "Creando topic SNS..."

awslocal sns create-topic --name mi-topic

echo "Topic creado"

# awslocal sns subscribe --region us-east-1 --topic-arn arn:aws:sns:us-east-1:000000000000:mi-topic --protocol http --notification-endpoint http://host.docker.internal:5000/api/aws-sns-receiver
awslocal sns subscribe --topic-arn arn:aws:sns:us-east-1:000000000000:mi-topic --protocol http --notification-endpoint http://localhost:5000/api/aws-sns-receiver