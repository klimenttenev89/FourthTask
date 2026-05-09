# Northwind Traders — Staff Portal

A microservice-based internal tool for looking up customers and their order history.

## Architecture

```
┌──────────────┐    HTTP     ┌─────────────┐    HTTP (internal)    ┌──────────────────┐
│   Frontend   │ ──────────► │   Gateway   │ ────────────────────► │ CustomerService  │
│  :5002 (MVC) │             │  :5000 (YARP│                       │  :5001 (Web API) │
│              │             │  + JWT Auth)│                       │                  │
└──────────────┘             └─────────────┘                       └──────────────────┘
                                                                            │
                                                                    ┌───────▼────────┐
                                                                    │   MySQL DB     │
                                                                    │ NorthwindTraders│
                                                                    └────────────────┘
```

| Service          | Port  | Role                                          |
|------------------|-------|-----------------------------------------------|
| CustomerService  | 5001  | Internal Web API — EF Core + MySQL            |
| Gateway          | 5000  | Reverse proxy — JWT auth, rate limiting, CORS |
| Frontend         | 5002  | ASP.NET Core MVC — session-based UI           |

## Security Features

| Feature                   | Where                                     |
|---------------------------|-------------------------------------------|
| JWT authentication        | Gateway issues & validates tokens         |
| Rate limiting             | Gateway: 100 req/min per IP               |
| CORS policy               | Gateway: Frontend origin only             |
| Secure password hashing   | Gateway: BCrypt (work factor 11)          |
| Input validation          | DataAnnotations + `[ApiController]`       |
| Safe error handling       | Global exception handlers in all services |
| HTTPS redirect            | All three services                        |
| HttpOnly session cookie   | Frontend session                          |
| Anti-forgery token        | Frontend login form                       |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- MySQL 8.x (or compatible — MariaDB 10.6+)

## Quick Start

### 1. Create the Database

Connect to your MySQL server and run:

```bash
mysql -u root -p < sql/northwind_seed.sql
```

### 2. Configure the Connection String

Edit `src/CustomerService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=NorthwindTraders;User=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Services

Open **three terminal windows** and run one command in each:

**Terminal 1 — CustomerService**
```bash
dotnet run --project src/CustomerService
```

**Terminal 2 — Gateway**
```bash
dotnet run --project src/Gateway
```

**Terminal 3 — Frontend**
```bash
dotnet run --project src/Frontend
```

### 5. Open the App

Browse to: **http://localhost:5002**

**Test credentials:**

| Username | Password   | Role  |
|----------|------------|-------|
| admin    | Admin@123  | Admin |
| staff    | Staff@123  | Staff |

## Running the Tests

```bash
dotnet test tests/CustomerService.Tests
```

The tests use EF Core InMemory — no database required.

## EF Core DB First — Re-scaffolding

The models in `src/CustomerService/Models/` were created to match the schema in `sql/northwind_seed.sql`.
If you modify the schema, you can re-scaffold the models with:

```bash
dotnet tool install --global dotnet-ef
dotnet ef dbcontext scaffold \
  "Server=localhost;Port=3306;Database=NorthwindTraders;User=root;Password=password;" \
  Pomelo.EntityFrameworkCore.MySql \
  --output-dir Models \
  --context NorthwindContext \
  --context-dir Data \
  --project src/CustomerService \
  --namespace NorthwindTraders.CustomerService.Models \
  --force
```

## API Reference

All requests to CustomerService go through the Gateway at `http://localhost:5000`.  
Include the JWT token in the `Authorization: Bearer <token>` header.

### Authentication

```
POST /api/auth/login
Content-Type: application/json

{ "username": "admin", "password": "Admin@123" }
```

**Response:** `{ "token": "eyJ...", "expiresAt": "2024-..." }`

### Customers

```
GET /api/customers             — list all customers (optional ?search=term)
GET /api/customers/{id}        — customer detail with order history
```

**Customer list item:**
```json
{
  "customerId": "ALFKI",
  "companyName": "Alfreds Futterkiste",
  "contactName": "Maria Anders",
  "country": "Germany",
  "orderCount": 2
}
```

**Customer detail:**
```json
{
  "customerId": "ALFKI",
  "companyName": "Alfreds Futterkiste",
  "contactName": "Maria Anders",
  "city": "Berlin",
  "country": "Germany",
  "orders": [
    { "orderId": 1, "orderDate": "2024-01-15", "totalValue": 275.00, "productCount": 2 }
  ]
}
```

## Project Structure

```
NorthwindTraders/
├── src/
│   ├── CustomerService/          # Internal microservice
│   │   ├── Controllers/
│   │   ├── Data/                 # EF Core DbContext
│   │   ├── DTOs/
│   │   ├── Models/               # DB-first EF models
│   │   └── Repositories/
│   ├── Gateway/                  # YARP proxy + JWT
│   │   ├── Controllers/          # Auth endpoint
│   │   ├── Models/
│   │   └── Services/
│   └── Frontend/                 # ASP.NET Core MVC
│       ├── Controllers/
│       ├── Models/
│       ├── Services/             # ApiService (calls Gateway)
│       └── Views/
├── tests/
│   └── CustomerService.Tests/    # xUnit — repository layer
├── sql/
│   └── northwind_seed.sql        # Schema + seed data
└── README.md
```

## Notes

- CustomerService is **not exposed** to the internet — it has no auth because it trusts the Gateway.
- The Gateway validates the JWT on every `/api/customers` request before forwarding to CustomerService.
- The Frontend stores the JWT in the server-side session (not in the browser) — the token never reaches JavaScript.
- For production: replace the hard-coded test users with a real user table and BCrypt-hashed passwords stored in the database; enforce HTTPS; use a secrets manager for the JWT key.
