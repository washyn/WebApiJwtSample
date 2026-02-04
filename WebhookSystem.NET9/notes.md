# TODO:
- Run with sqlite database
- Test with  http files

# NOTES:
- This is web hook sender service.



abp way

```csharp
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;

namespace AbpDemo
{
    public class MyService : ITransientDependency
    {
        private readonly ILocalEventBus _localEventBus;
        public MyService(ILocalEventBus localEventBus)
        {
            _localEventBus = localEventBus;
        }
        public virtual async Task ChangeStockCountAsync(Guid productId, int newCount)
        {
            //TODO: IMPLEMENT YOUR LOGIC...
            //PUBLISH THE EVENT
            await _localEventBus.PublishAsync(
                new StockCountChangedEvent
                {
                    ProductId = productId,
                    NewCount = newCount
                }
            );
        }
    }
    public class StockCountChangedEvent
    {
        public Guid ProductId { get; set; }
        
        public int NewCount { get; set; }
    }
}

```



Responses

```http

HTTP/1.1 201 Created
Connection: close
Content-Type: application/json; charset=utf-8
Date: Wed, 04 Feb 2026 14:40:12 GMT
Server: Kestrel
Location: /api/webhooks/subscriptions/35091743-b93e-4fa6-a7fc-f2d8b36dc2a8
Transfer-Encoding: chunked

{
  "id": "35091743-b93e-4fa6-a7fc-f2d8b36dc2a8",
  "url": "http://localhost:5262/webhooks/receive/generic",
  "secret": "zQscyXRKdKooPSL60EsIdutB4tPNfl4OdeD0QwP34Wk=",
  "events": [
    "order.created",
    "order.updated"
  ],
  "isActive": true,
  "createdAt": "2026-02-04T14:40:12.7365925Z",
  "updatedAt": null,
  "description": "Order processing webhook",
  "maxRetries": 5,
  "retryDelay": "00:01:00",
  "headers": {},
  "deliveries": []
}
```



HTTP/1.1 201 Created
Connection: close
Content-Type: application/json; charset=utf-8
Date: Wed, 04 Feb 2026 21:07:21 GMT
Server: Kestrel
Location: /api/webhooks/subscriptions/980d0966-1b75-4bc1-a126-6592bbf7e4ad
Transfer-Encoding: chunked

{
  "id": "980d0966-1b75-4bc1-a126-6592bbf7e4ad",
  "url": "http://localhost:5262/webhooks/orders",
  "secret": "dD8WvlgmLvV3Y8cdFYEY9Azu/OMw01JwEY/duLDoCfw=",
  "events": [
    "order.created",
    "order.updated"
  ],
  "isActive": true,
  "createdAt": "2026-02-04T21:07:22.582256Z",
  "updatedAt": null,
  "description": "Order processing webhook",
  "maxRetries": 5,
  "retryDelay": "00:02:00",
  "headers": {},
  "deliveries": []
}
