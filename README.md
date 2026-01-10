# trackDiesel API

TrackDiesel is an ASP.NET Core Web API for managing cars, parts, and error codes. It includes JWT authentication, PostgreSQL persistence via Entity Framework Core, Swagger documentation, a data seeder for error codes, and containerization support.

Note: the project contains Turkish response messages in controllers (e.g., "Kullanıcı adı bulunamadı"). I kept examples language-agnostic.

## Tech stack
- .NET 8.0 (net8.0)
- ASP.NET Core Web API
- Entity Framework Core (+ Npgsql provider for PostgreSQL)
- JWT Bearer authentication
- Swagger (Swashbuckle)
- Docker (Dockerfile included)
- GitHub Actions for container build & publish

---

## Quick start (development)

Prerequisites:
- .NET 8 SDK installed (https://dotnet.microsoft.com/)
- PostgreSQL accessible (local/docker/managed)
- Optional: Docker 20+ for container runs

1. Clone the repository
   git clone https://github.com/Muratcany001/trackDieselApi.git
   cd trackDieselApi/BarMenu

2. Update configuration (see next section) and run:
   dotnet restore
   dotnet build
   dotnet run

3. When running in Development environment, Swagger UI is available at:
   http://localhost:5000 (or the URL printed by dotnet) — the code configures Swagger UI at the app root when env is Development.

---

## Configuration

Default configuration file: `BarMenu/appsettings.json`.

Important settings:
- Connection string key: `ConnectionStrings:DefaultConnection`
  - Out of the box the repo contains a connection string for a remote Postgres instance. For local development replace with your own:
    "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=trackdiesel;Username=postgres;Password=yourpassword" }

- JWT secret:
  - Current code uses a hard-coded secret string:
    `"your-256-bit-long-secret-key-that-you-need-to-ensure-is-256-bits-long"`
  - For production, do NOT keep the secret in code. Replace by reading from configuration or environment variables. Example environment variable usage:
    - Set environment variable: ASPNETCORE_Environment=Production
    - Set an env var e.g. TRACKDIESEL_JWT_SECRET and modify Program.cs/AuthController to read it:
      var jwtSecret = Environment.GetEnvironmentVariable("TRACKDIESEL_JWT_SECRET");

- Ports / Hosting:
  - The Dockerfile exposes ports 8080 and 8081. When running the container set ASPNETCORE_URLS or map ports explicitly:
    docker run -e ASPNETCORE_URLS="http://+:8080" -p 8080:8080 ...

CORS:
- Program.cs contains an AllowFrontend policy listing several allowed origins used by the original front-end(s). Add/remove origins as needed.

---

## Database / EF Core

Migrations are present in `BarMenu/Migrations`. Typical dev workflow:

1. Ensure `DefaultConnection` in appsettings points to your Postgres.
2. Install the EF CLI (if not already):
   dotnet tool install --global dotnet-ef
3. Apply migrations:
   dotnet ef database update --project BarMenu --startup-project BarMenu
   (Run from solution root or adjust paths accordingly.)

Seeding:
- The app calls `DataSeeder.SeedErrors(context, jsonPath)` at startup which loads `ErrorCodes.json` from the application base directory and inserts non-duplicate error codes into the `Errors` table.
- To seed, simply start the app after your DB is reachable. The seed prints logs if errors or if the JSON file is missing.

---

## Running with Docker

Build:
- From repository root:
  docker build -t trackdieselapi -f BarMenu/Dockerfile .

Run:
- Because the Dockerfile uses .NET images, ensure you pass ASPNETCORE_URLS and map ports:
  docker run -d -e ASPNETCORE_URLS="http://+:8080" -p 8080:8080 --name trackdieselapi trackdieselapi

Important note about the included Dockerfile:
- The provided Dockerfile uses Windows nanoserver images:
  FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809
  These images require a Windows container host and cannot be built on Linux runners by default.
- If you want to build/run on Linux (Docker Desktop in Linux containers or CI on ubuntu runners), change base images to the Linux variants:
  FROM mcr.microsoft.com/dotnet/aspnet:8.0
  FROM mcr.microsoft.com/dotnet/sdk:8.0
  This will allow Linux-based builds and the existing GitHub Actions workflow to succeed on ubuntu runners.

---

## Authentication

- Login endpoint: POST /api/auth/login
  - Request: { "name": "username", "password": "plaintext" }
  - Response: { "token": "<JWT>" }

- Logout endpoint: POST /api/auth/logout
  - Requires Authorization header with Bearer token.
  - The controller keeps an in-memory blacklist for logged-out tokens (note: this blacklist is process-local; for scaled/production systems, use a persistent blacklist or token short TTL and refresh strategy).

JWT details:
- Token is a standard JWT containing claims:
  - ClaimTypes.Name => username
  - ClaimTypes.NameIdentifier => user.Id (string)
  - ClaimTypes.Role => "User"
- The code currently validates tokens with a symmetric key. Use a secure secret and store it in environment variables or a secrets manager.

---

## API Reference (selected routes with examples)

General notes:
- Many endpoints require Authorization: Bearer <token>.
- Response messages and validation errors are in Turkish in the controller methods.

1) Authentication
- POST /api/auth/login
  curl example:
  curl -X POST http://localhost:5000/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"name":"alice","password":"password"}'

  Successful response:
  { "token":"eyJ..." }

- POST /api/auth/logout
  curl example:
  curl -X POST http://localhost:5000/api/auth/logout \
    -H "Authorization: Bearer <token>"

2) Users
- POST /users/register
  Request body:
  {
    "name": "alice",
    "email": "alice@example.com",
    "password": "plaintextPassword"
  }
  Example:
  curl -X POST http://localhost:5000/users/register \
    -H "Content-Type: application/json" \
    -d '{"name":"alice","email":"a@b.com","password":"pwd"}'

- GET /users/GetUserByName?name=alice
- GET /users/getUser/{id}
- PATCH /users/updateUser/{id}
- DELETE /users/delete/{id}
- GET /users/getUser  (returns list)

3) Cars (require Authorization)
- POST /cars/AddCar
  Sample body (note JSON structure should match Car and Issue models):
  {
    "name":"My Car",
    "plate":"34XYZ34",
    "age":5,
    "lastMaintenanceDate":"2025-01-01T00:00:00",
    "errorHistory":[
      { "Code":"P123", "Description":"Example error", "Date":"2025-01-01T00:00:00" }
    ],
    "carParts":[
      { "PartId": 1, "State":"New" }
    ]
  }
  curl:
  curl -X POST http://localhost:5000/cars/AddCar \
    -H "Authorization: Bearer <token>" \
    -H "Content-Type: application/json" \
    -d '<json body>'

- GET /cars/GetAll
- GET /cars/GetCarByPlate/{plate}
- PATCH /cars/UpdateCar/{plate} (body: list of Issue objects)
- DELETE /cars/DeleteCar/{plate}
- GET /cars/GetCarCount
- GET /cars/getAllIssues
- GET /cars/GetModelsWithBrokenParts
- GET /cars/MostCommonProblems

4) Parts (require Authorization)
- POST /parts/AddParts
  {
    "name":"Oil Filter",
    "description":"Filter for engine",
    "count": 5,
    "state":"New"
  }

- POST /parts/AddBulkParts  (body: array of Part objects)
- GET /parts/GetAllParts
- GET /parts/GetPartByName/{name}
- GET /parts/GetPartById/{id}
- PATCH /parts/UpdateStockAsync/{id}  (update stock/count)
- DELETE /parts/DeletePart/{id}

5) Errors (error code metadata)
- GET /errors/GetErrorByName/{errorName}
- POST /errors/AddError  (adds new error code entry)

---

## Swagger / OpenAPI
- The app registers Swagger when environment is Development.
- When running in Development mode, visit:
  http://localhost:5000/swagger/index.html
- Swagger UI is configured with a Bearer scheme—use the "Authorize" button to paste a token.

---

## GitHub Actions (CI/CD)
- There's a workflow at `.github/workflows/docker-publish.yml`:
  - Triggers: schedule, push to master, PRs to master, and semver tag pushes (v*.*.*).
  - Uses docker/build-push-action and (when not a PR) signs the image with cosign.
  - Pushes images to `ghcr.io/<owner>/<repo>` by default (REGISTRY env var).
- Note: If you keep the current Dockerfile (Windows nanoserver), the ubuntu-latest runner will not successfully build the image. Either:
  - Change the Dockerfile to Linux images (recommended), or
  - Use a Windows runner in the workflow to build Windows containers.

---

## Troubleshooting & notes

- Hard coded secrets:
  - Replace the hard-coded JWT secret in Program.cs and AuthController with a configuration value or environment variable.
- Docker base images:
  - The provided Dockerfile uses Windows nanoserver images and will fail on Linux runners. Replace with Linux base images unless you specifically target Windows containers.
- Token blacklist:
  - The logout token blacklist is in-memory. For horizontally scaled systems use persistent store (Redis, DB) or short-lived tokens + refresh tokens.
- Data seeding:
  - Ensure `ErrorCodes.json` is present in the application base directory when the app runs. The seeder avoids duplicates by Code.

---

## Contributing
- Fork, create a feature branch, add tests, open a pull request.
- Keep secrets out of commits. Use environment variables or GitHub Secrets.

---

## License
- Add a LICENSE file to this repository (e.g., MIT) if you want to make it open source.

---

If you want, I can:
- Produce a short Postman collection or OpenAPI examples for the main endpoints.
- Propose a minimal Dockerfile change to use Linux images.
- Add a sample docker-compose file that spins up Postgres + the API for local dev.
