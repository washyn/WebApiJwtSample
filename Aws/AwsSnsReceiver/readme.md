```bash

# awslocal sns list-subscriptions

{
    "Subscriptions": [
        {
            "SubscriptionArn": "arn:aws:sns:us-east-1:000000000000:mi-topic:d0c1cb34-d9df-45d1-9c84-b05424427d7a",
            "Owner": "000000000000",
            "Protocol": "http",
            "Endpoint": "http://localhost:5000/api/aws-sns-receiver",
            "TopicArn": "arn:aws:sns:us-east-1:000000000000:mi-topic"
        },
        {
            "SubscriptionArn": "arn:aws:sns:us-east-1:000000000000:mi-topic:70c2ab68-2a0a-4738-a0ec-a04e9871ce57",
            "Owner": "000000000000",
            "Protocol": "http",
            "Endpoint": "http://host.docker.internal:5000/api/aws-sns-receiver",
            "TopicArn": "arn:aws:sns:us-east-1:000000000000:mi-topic"
        }
    ]
}



awslocal sns publish --topic-arn arn:aws:sns:us-east-1:000000000000:mi-topic --message "mensaje de prueba"

awslocal sns publish  --topic-arn arn:aws:sns:us-east-1:000000000000:mi-topic  --message "hola desde sns"

```
