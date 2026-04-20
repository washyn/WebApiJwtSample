#!/bin/bash

echo "Creando topic SNS..."

awslocal sns create-topic --name mi-topic

echo "Topic creado"