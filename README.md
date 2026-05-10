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

## Build & Run Scripts

The `BuildAndRun/` folder contains scripts to build and run each service without typing `dotnet` commands manually.

### Windows (`BuildAndRun/Windows/`)

| Script | What it does |
|---|---|
| `build.bat` | Restores and builds the entire solution in Release mode |
| `run-customer-service.bat` | Starts CustomerService on port 5001 |
| `run-gateway.bat` | Starts Gateway on port 5000 |
| `run-frontend.bat` | Starts Frontend on port 5002 |

**Usage:**
1. Complete steps 1 and 2 from [Quick Start](#quick-start) (database + connection string).
2. Double-click (or run from a terminal) `BuildAndRun\Windows\build.bat` to build.
3. Open three terminal windows and run one script in each:
   ```
   BuildAndRun\Windows\run-customer-service.bat
   BuildAndRun\Windows\run-gateway.bat
   BuildAndRun\Windows\run-frontend.bat
   ```

### Linux (`BuildAndRun/Linux/`)

| Script | What it does |
|---|---|
| `build.sh` | Restores and builds the entire solution in Release mode |
| `run-customer-service.sh` | Starts CustomerService on port 5001 |
| `run-gateway.sh` | Starts Gateway on port 5000 |
| `run-frontend.sh` | Starts Frontend on port 5002 |

**Usage:**
1. Complete steps 1 and 2 from [Quick Start](#quick-start) (database + connection string).
2. Make the scripts executable (one-time):
   ```bash
   chmod +x BuildAndRun/Linux/*.sh
   ```
3. Build the solution:
   ```bash
   ./BuildAndRun/Linux/build.sh
   ```
4. Open three terminal windows and run one script in each:
   ```bash
   ./BuildAndRun/Linux/run-customer-service.sh
   ./BuildAndRun/Linux/run-gateway.sh
   ./BuildAndRun/Linux/run-frontend.sh
   ```

---

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

## Swagger UI

| Service         | URL                           | Auth required |
|-----------------|-------------------------------|---------------|
| Gateway         | http://localhost:5000/swagger | No — use it to obtain a token, then authorize proxied routes |
| CustomerService | http://localhost:5001/swagger | No — internal port, JWT is not enforced here |

**Using the Gateway Swagger:**
1. Call `POST /api/auth/login` with your credentials to receive a token.
2. Click **Authorize** (top-right lock icon), paste the token, and confirm.
3. All subsequent requests in that session include the `Authorization: Bearer` header.

> **Note:** `http://localhost:5001/swagger` gives unauthenticated access to the internal service. In production this port must not be publicly reachable.

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


## Claude Prompts
### Prompt 1:

You are a software architect. I need you to design a clean and maintainable microservice backend and frontend using ASP.NET and MVC. 
Please do consider adding gatekeeper api with JWT authentication between the frontend and the backend AS the microservice architecture requires.
Also we want the app to be using MySql DB first model with Entiry Framework (the latest version). Also we need some mechanisms to make our app safe such as
Input validation, Rate limiting, HTTPS Only, CORS policy, Safe error handling, Secure password hashing. The app is for testing purposes only and plase do generate a
comprehensive README.md so we can run it locally. Since is for testing do not overdo it we need it as simple as possible but having the core principles presented.
THe app should be made using .NET Core 8 preferably and be able to be run on windows or linux.

Business Scenario:
Northwind Traders is a fictional company that sells food products to customers 
worldwide. The business needs a simple internal tool that allows staff to look up 
customers and review their order history.
Your task is to build a back-end service that exposes this data in a clean, well-structured 
way.

What the Application Should Do:
1. Customer Overview
Staff should be able to retrieve a list of customers; with each customer's name and the 
number of orders they have placed. It should be possible to search or filter by customer 
name.
2. Customer Detail
When a specific customer is selected, the service should return the customer's details 
along with a summary of their order history. For each order, the following should be 
available:
• The total value of the order
• The number of products included in the order

3. Structure of Your Solution
We leave the architecture entirely to you. Here are a few things to keep in mind:
• If you do build a front-end, it should communicate with the back end exclusively over 
HTTP — it should not have direct access to the database layer.
• You may structure the solution with as many projects or layers as you feel is 
appropriate.

4. Testing
Ideally, include at least one automated test. There is no requirement for full coverage —
we are simply interested in how you think about testability and what you consider worth 
verifying.

At last please do try to build it and fix the build errors, also make an appropriate gitignore in the root directory

### Prompt 2:

Please do add, logging in file for every day, in meaning every ned day new file .log should be available, also please do add swagger for the backend api.

### Prompt 3:
In the root of the current project i added a Folder BuildAndRun, please do add a scripts for Linux and Windows for easy build and run according to folders inside (Linux, Windows). Please 1 script for build for windows, 3 scripts for run for windows, 1 script for build for linux and 3 for run. Also add the instructions for the scripts in the readme.md before the manual build and run.
