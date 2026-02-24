# inventory-api

Inventory API is a mini REST API in ASP.NET that allows to manage products, customers and orders.
The project implements business validation such as stock control or order status management and a clear separation between models and HTTP contracts through DTOs.
The main objective is to practice backend design and REST APIs and simulate a real project.

## Technologies

- .NET 9
- ASP.NET Core Minimal API
- Entity Framework Core
- SQLite

## Endpoints
- GET /products

- GET /products/{id}
- POST /products
- PATCH /products/{id}
- PATCH /products/{id}/deactivate
- GET /customers
- GET /customers/{id}
- POST /customers
- PATCH /customers/{id}
- GET /orders
- GET /orders/{id}
- POST /orders
- PATCH /orders/{id}

## Example: Create Order

- Request:
```json
{
  "customerId": 1,
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 5
    }
  ]
}
```

- Response (201 Created):
```json
{
  "id": 5,
  "customer": {
    "id": 1,
    "name": "John Doe"
  },
  "orderDate": "2026-02-20T13:03:23.1274803Z",
  "orderStatus": "Pending",
  "totalAmount": 7.15,
  "orderItems": [
    {
      "id": 7,
      "product": {
        "id": 1,
        "name": "Apples"
      },
      "quantity": 2,
      "unitPrice": 1.2,
      "totalPrice": 2.4
    },
    {
      "id": 8,
      "product": {
        "id": 2,
        "name": "Bread"
      },
      "quantity": 5,
      "unitPrice": 0.95,
      "totalPrice": 4.75
    }
  ]
}
```

## API Documentation

Interactive API documentation is available via Swagger:
http://localhost:xxxx/swagger

## How to run

1. Open bash terminal in **InventoryApi\InventoryApi directory**
2. Restore NuGet packages with `dotnet restore`
3. Run the app with `dotnet run`. The application will start on a local development port  (e.g. http://localhost:5131)
