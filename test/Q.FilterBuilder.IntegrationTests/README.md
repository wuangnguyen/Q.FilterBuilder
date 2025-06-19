# Q.FilterBuilder.IntegrationTests

Comprehensive integration test project for Q.FilterBuilder that validates the complete workflow from JSON input to actual database query execution using **Testcontainers** and **ASP.NET Core Test Server**.

## Overview

This test project validates the **complete real-world workflow** via web API endpoints:

1. **JSON Input** → **QueryBuilderConverter** → **FilterGroup**
2. **FilterGroup** → **IFilterBuilder** → **SQL Query + Parameters**
3. **Query Execution** → **Multiple ORMs** → **Database Results**
4. **Result Validation** → **Assert Expected Outcomes**

## Testcontainers Integration

The integration tests use **Testcontainers for .NET** to automatically manage database containers:

### Supported Databases
- **SQL Server** - `mcr.microsoft.com/mssql/server:2022-latest`
- **MySQL** - `mysql:8.0`
- **PostgreSQL** - `postgres:15`

### Automatic Container Management
- Containers are **automatically started** before each test class
- Containers are **automatically cleaned up** after each test class
- Each test class gets **fresh database instances**
- No manual Docker setup required

### Test Data
Each container is automatically populated with:
- **Users table** - 4 test users with various attributes
- **Categories table** - 3 product categories  
- **Products table** - 4 test products linked to categories

### Prerequisites
- Docker Desktop must be running
- .NET 8.0 SDK
- No additional setup required - Testcontainers handles everything

### Configuration Architecture

The test infrastructure uses an extremely simplified configuration approach:

1. **Provider Selection**: The `DatabaseProvider` setting determines which provider to use
2. **Direct Provider Sections**: Each provider has its own flat configuration section (e.g., `SqlServer`, `MySql`, `PostgreSql`)
3. **Flat Configuration Structure**: All settings are at the top level of each provider section
4. **Automatic Connection Strings**: Connection strings are generated automatically by Testcontainers
5. **Simple Configuration Binding**: Uses `Configuration.GetSection(provider.ToString()).Bind(databaseConfig)`
6. **No Nested Objects**: Eliminated unnecessary nesting for maximum simplicity

## Running Tests

### Run All Tests

```bash
# Run all integration tests
dotnet test test/Q.FilterBuilder.IntegrationTests/
```

### Run Specific Provider Tests

Tests automatically run against all available providers. To test a specific provider:

```bash
# SQL Server only
$env:DatabaseProvider="SqlServer"
dotnet test test/Q.FilterBuilder.IntegrationTests/

# MySQL only
$env:DatabaseProvider="MySql"
dotnet test test/Q.FilterBuilder.IntegrationTests/

# PostgreSQL only
$env:DatabaseProvider="PostgreSql"
dotnet test test/Q.FilterBuilder.IntegrationTests/
```

### Run Specific Test Categories

```bash
# Dapper integration tests only
dotnet test --filter "DapperIntegrationTests"

# Entity Framework tests only
dotnet test --filter "EfIntegrationTests"

# ADO.NET tests only
dotnet test --filter "AdoNetIntegrationTests"

# Specific test method
dotnet test --filter "MixedOperators_ShouldReturnCorrectResults"
```

### Using Visual Studio
1. Ensure Docker Desktop is running
2. Open Test Explorer
3. Run tests individually or by category

## Configuration

### Environment Variables
- `DatabaseProvider` - Database provider to use (SqlServer, MySql, PostgreSql)

### appsettings.test.json
```json
{
  "DatabaseProvider": "SqlServer", // update this to test different providers
  "SqlServer": {
    "ImageName": "mcr.microsoft.com/mssql/server:2022-latest",
    "Database": "FilterBuilderTest",
    "Username": "sa",
    "Password": "YourStrong@Passw0rd123",
    "Environment": {
      "ACCEPT_EULA": "Y",
      "MSSQL_PID": "Express"
    }
  },
  "MySql": {
    "ImageName": "mysql:8.0",
    "Database": "testdb",
    "Username": "testuser",
    "Password": "testpass",
  },
  "PostgreSql": {
    "ImageName": "postgres:15",
    "Database": "testdb",
    "Username": "testuser",
    "Password": "testpass"
  }
}
```

## Test Data Schema

### Users Table
- **Id** (int, PK)
- **Name** (string, required)
- **Email** (string, required)
- **Age** (int)
- **Salary** (decimal)
- **IsActive** (bool)
- **CreatedDate** (datetime)
- **LastLoginDate** (datetime, nullable)
- **Department** (string, nullable)
- **Role** (string, nullable)
- **CategoryId** (int, FK, nullable)

### Categories Table
- **Id** (int, PK)
- **Name** (string, required)
- **Description** (string, nullable)
- **IsActive** (bool)
- **CreatedDate** (datetime)

### Products Table
- **Id** (int, PK)
- **Name** (string, required)
- **Description** (string, nullable)
- **Price** (decimal)
- **Stock** (int)
- **IsAvailable** (bool)
- **CreatedDate** (datetime)
- **UpdatedDate** (datetime, nullable)
- **Status** (string, nullable)
- **Rating** (double, nullable)
- **CategoryId** (int, FK)
- **CreatedByUserId** (int, FK, nullable)
- **Tags** (string, JSON array)
- **Metadata** (string, JSON object)

## ORM Testing

Each test validates query execution across multiple ORMs:

### Entity Framework Core
- Uses `FromSqlRaw` for parameterized queries
- Validates EF Core compatibility

### Dapper
- Direct SQL execution with dynamic parameters
- Validates micro-ORM compatibility

### ADO.NET
- Raw database connection and command execution
- Validates low-level database compatibility

### Docker Issues
```bash
# Check Docker status
docker info

# Pull required images manually
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker pull mysql:8.0
docker pull postgres:15
```

### Debug Mode
Enable detailed logging:
```bash
$env:LOGGING__LOGLEVEL__DEFAULT="Debug"
$env:LOGGING__LOGLEVEL__MICROSOFT="Information"
dotnet test test/Q.FilterBuilder.IntegrationTests/ --verbosity detailed
```

### Performance Considerations
- Container startup adds ~10-15 seconds per provider
- Tests run sequentially to avoid resource conflicts
- Database seeding occurs once per container lifecycle
- Connection pooling disabled for clean test isolation

## Contributing

When adding new tests:
1. Follow the existing naming conventions
2. Include comprehensive assertions
3. Test across all supported providers
4. Update JSON samples as needed
5. Document expected behavior clearly
