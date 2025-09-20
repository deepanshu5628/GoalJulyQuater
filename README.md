# GoalJulyQuater

# GoalQuater1

A sample ASP.NET Core Web API project for managing customers, orders, and order items, featuring data seeding, validation, and CSV export functionality. The project is built with .NET 8 and demonstrates modern API development practices, including test automation.

## Features

- **Customer, Order, and OrderItem Management**: CRUD endpoints for customers, orders, and order items.
- **Data Seeding**: Automatically seeds the database with realistic sample data using [Bogus](https://github.com/bchavez/Bogus) on startup.
- **Validation**: Uses [FluentValidation](https://fluentvalidation.net/) for robust input validation.
- **CSV Export**: Exports order data as CSV via a secure API endpoint.
- **Swagger/OpenAPI**: Interactive API documentation enabled by [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).
- **Unit Testing**: Comprehensive test coverage using xUnit, Moq, FluentAssertions, and EF Core InMemory.

## Technologies Used

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (SQL Server & InMemory)
- FluentValidation
- Bogus (for data generation)
- CsvHelper (for CSV export)
- xUnit, Moq, FluentAssertions (for testing)
- Swashbuckle (Swagger UI)

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or change the connection string for your environment)
- Visual Studio 2022 or later (recommended)

### Setup

1. **Clone the repository**  

2. **Configure the Database Connection**  
   Update the `FabricConnectionString` in `appsettings.json` to point to your SQL Server instance.

3. **Restore Dependencies**  

4. **Run Database Migrations (if needed)**  
   If you want to apply migrations manually:

5. **Run the Application**  
The API will be available at `https://localhost:<port>/swagger` for interactive documentation.

6. **Seed Data**  
   On first run, the database will be seeded automatically with sample customers, orders, and order items.

### Running Tests

You do **not** need to run the web application to execute tests.  
To run all tests:

Or use the Test Explorer in Visual Studio (`Test > Run All Tests`).

## API Highlights

- **Swagger UI**: Browse and test all endpoints at `/swagger`.
- **CSV Export**:  
  - Endpoint: `GET /orders`
  - Requires header: `X-Api-Key` (set in configuration)
  - Returns: CSV file of all orders and their items

## Project Structure

- `GoalQuater1/` - Main Web API project
- `GoalQuater1.Tests/` - Test project (unit/integration tests)
- `Services/DataSeeder.cs` - Seeds the database with sample data
- `Controllers/` - API controllers for Customers, Orders, OrderItems, and Export

## Customization

- **Validation Rules**: See `Services/CustomValidator.cs` for how to adjust validation.
- **Data Generation**: Modify `DataSeeder.cs` to change the amount or type of sample data.
- **API Key**: Set your API key for export in your configuration (e.g., `appsettings.json`).

## License

This project is for educational/demo purposes.

---

*Generated with .NET 8, ASP.NET Core, and best practices for modern API development.*
