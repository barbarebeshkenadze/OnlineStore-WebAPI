# OnlineStore — ASP.NET Core Web API

OnlineStore is a backend REST API for managing products, categories, customers, and orders. It was built as a C#/.NET final project. The project does **not** include a frontend; endpoints can be tested with Swagger UI.

## Technologies

- C# and ASP.NET Core Web API (.NET 8)
- SQL Server and Entity Framework Core
- Repository and Service layers, interfaces, dependency injection
- Async/await, DTOs, LINQ, HttpClient
- Swagger/OpenAPI, centralized exception middleware

## Solution structure

| Project | Responsibility |
| --- | --- |
| `OnlineStore.API` | HTTP controllers, request/response DTOs, middleware, startup configuration |
| `OnlineStore.Core` | Domain entities and repository/service interfaces |
| `OnlineStore.Infrastructure` | EF Core DbContext, migrations, repository and service implementations |
| `OnlineStoreFinal` | Additional project present in the solution; not required for the main Web API workflow |

## API endpoints

| Resource | Methods / routes |
| --- | --- |
| Products | `GET /api/Products`, `GET /api/Products/{id}`, `POST /api/Products`, `PUT /api/Products/{id}`, `DELETE /api/Products/{id}` |
| Categories | `GET /api/Categories`, `GET /api/Categories/{id}`, `POST /api/Categories`, `PUT /api/Categories/{id}`, `DELETE /api/Categories/{id}` |
| Customers | `GET /api/Customers`, `GET /api/Customers/{id}`, `POST /api/Customers`, `PUT /api/Customers/{id}`, `DELETE /api/Customers/{id}` |
| Orders | `GET /api/Orders`, `GET /api/Orders/{id}`, `POST /api/Orders`, `PUT /api/Orders/{id}/status`, `DELETE /api/Orders/{id}` |
| External products | `GET /api/ExternalProducts` (fetches products from DummyJSON) |

## Getting started

1. Install the .NET 8 SDK, SQL Server, and Visual Studio with ASP.NET and web development workload.
2. Open `OnlineStoreFinal.slnx` in Visual Studio.
3. Configure the `DefaultConnection` connection string in `OnlineStore.API/appsettings.json` for **your own** SQL Server instance. Do not commit database passwords.
4. Set `OnlineStore.API` as the startup project.
5. Apply EF Core migrations in Visual Studio Package Manager Console (default project: `OnlineStore.Infrastructure`):

   ```powershell
   Update-Database -StartupProject OnlineStore.API -Project OnlineStore.Infrastructure
   ```

   Alternatively, from the solution root with the `dotnet-ef` tool installed:

   ```bash
   dotnet ef database update --project OnlineStore.Infrastructure --startup-project OnlineStore.API
   ```

6. Run the API from Visual Studio (F5) and open `/swagger` while running in the Development environment.
7. Optionally execute `SQL/DatabaseObjects.sql` in SQL Server Management Studio to create the SQL view and stored procedure.

## Database

EF Core migrations define `Products`, `Categories`, `Customers`, `Orders`, and `OrderItems`, including their relationships. `SQL/DatabaseObjects.sql` contains a view (`vw_ProductDetails`) and a stored procedure (`sp_GetProductsByMinPrice`) for SQL demonstration.

## Order workflow

An order is created for a customer with one or more products. The service validates the request, calculates item prices and total, and manages product stock. Order status updates and deletion are handled through dedicated endpoints.

## Error handling

Controllers return HTTP responses such as 200, 201, 204, 400, 404, and 409 where appropriate. `ExceptionMiddleware` handles unexpected server exceptions and returns a JSON error response.

## External API

`GET /api/ExternalProducts` uses `HttpClient` to retrieve product data from `https://dummyjson.com/products?limit=10`.

## Notes

- This is a **backend-only** educational project.
- Swagger UI is available in Development mode.
- Never commit passwords, tokens, or production connection strings.
