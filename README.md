# E-Commerce Web API

A full-featured RESTful e-commerce API built with ASP.NET Core 8.0, implementing clean architecture principles for scalability and maintainability.

## Project Overview

This project is a comprehensive e-commerce backend solution that provides complete functionality for managing an online store. It implements a modern, scalable architecture with separation of concerns, enabling easy maintenance and future enhancements. The API supports product catalog management, shopping cart operations, order processing, user authentication, and payment integration with Stripe.

**Key Goals:**
- Provide a robust, production-ready e-commerce API
- Implement clean architecture for maintainability
- Support modern authentication and authorization
- Enable efficient caching and data retrieval
- Integrate with payment providers for transaction processing

**Use Cases:**
- Online retail platforms
- Multi-vendor marketplaces
- Food delivery services
- Digital product stores

## Tech Stack

**Backend:**
- .NET 8.0 (C#)
- ASP.NET Core Web API
- Entity Framework Core 9.0.4
- ASP.NET Core Identity

**Database:**
- SQL Server (main store database)
- SQL Server (identity database)
- Redis (caching & basket storage)

**Key Libraries:**
- AutoMapper 14.0.0 (object mapping)
- Stripe.net 50.0.0 (payment processing)
- StackExchange.Redis 2.9.25 (Redis client)
- Swashbuckle/Swagger 6.6.2 (API documentation)
- JWT Bearer Authentication

**Patterns & Architecture:**
- Clean/Onion Architecture
- Repository Pattern
- Unit of Work Pattern
- Specification Pattern
- Dependency Injection
- CQRS-inspired design

## Architecture

The project follows **Clean Architecture** principles with clear separation between layers:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│  (API Controllers, Middleware)          │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Application Layer               │
│  (Services, DTOs, Mappings)             │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Domain Layer                    │
│  (Entities, Contracts, Exceptions)      │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Infrastructure Layer            │
│  (EF Core, Repositories, Data Access)   │
└─────────────────────────────────────────┘
```

**Major Components:**

1. **Core/DomainLayer**: Contains domain entities, repository interfaces, and business exceptions
2. **Core/Service**: Business logic implementation, specifications, AutoMapper profiles
3. **Core/ServiceAbstraction**: Service interfaces for dependency injection
4. **Infrastructure/Persistence**: EF Core contexts, repositories, migrations, data seeding
5. **Infrastructure/Presentation**: API controllers, filters, attributes
6. **Shared**: DTOs, query parameters, error models
7. **E-Commerce.Web**: Entry point, middleware configuration, service registration

**Data Flow:**
```
Client Request → Controller → Service → Repository → Database
                                  ↓
                            Redis Cache (optional)
```

## Features

### Core Features
- ✅ **Product Catalog Management**
  - Full CRUD operations
  - Advanced filtering (by brand, type, search query)
  - Sorting (by name, price - ascending/descending)
  - Pagination with configurable page size
  - Product images support

- ✅ **Shopping Basket**
  - Redis-based cart storage
  - Add/update/remove items
  - Persistent cart across sessions
  - TTL-based expiration (30 days default)

- ✅ **Order Management**
  - Create orders from basket
  - Multiple delivery methods
  - Order history retrieval
  - Order status tracking (pending, payment received, payment failed)

- ✅ **User Authentication & Authorization**
  - JWT-based authentication
  - User registration and login
  - Role-based authorization (Admin, SuperAdmin)
  - User profile management
  - Address management

- ✅ **Payment Integration**
  - Stripe payment intent creation/update
  - Automatic price validation
  - Delivery cost calculation
  - Payment status tracking

- ✅ **Caching System**
  - Redis-based response caching
  - Custom cache attribute for endpoints
  - Configurable cache duration

### Additional Features
- 🔧 Custom exception handling middleware
- 🔧 Model validation with detailed error responses
- 🔧 Swagger UI with JWT authorization support
- 🔧 CORS configuration
- 🔧 Data seeding from JSON files
- 🔧 Specification pattern for complex queries

## Setup & How to Run

### Prerequisites
- .NET 8.0 SDK or later ([Download](https://dotnet.microsoft.com/download))
- SQL Server (LocalDB, Express, or Full)
- Redis Server ([Download](https://redis.io/download) or use Docker)
- Visual Studio 2022 / VS Code / Rider (recommended)
- Stripe Account (for payment features)

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd E-Commerce.Web
   ```

2. **Update Connection Strings**
   
   Edit `E-Commerce.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ECommerce;Trusted_Connection=True;TrustServerCertificate=True",
       "IdentityConnection": "Server=YOUR_SERVER;Database=ECommerce.Identity;Trusted_Connection=True;TrustServerCertificate=True",
       "RedisConnectionString": "localhost:6379"
     }
   }
   ```

3. **Configure Stripe API Key**
   ```json
   {
     "StripeSettings": {
       "SecretKey": "sk_test_YOUR_STRIPE_SECRET_KEY"
     }
   }
   ```

4. **Configure JWT Settings**
   ```json
   {
     "JWT": {
       "Options": {
         "SecretKey": "YOUR_256_BIT_SECRET_KEY_HERE",
         "Issuer": "https://your-domain.com/",
         "Audience": "https://your-domain.com/api"
       }
     }
   }
   ```

5. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

6. **Apply Database Migrations**
   ```bash
   # From E-Commerce.Web directory
   dotnet ef database update --project ../Infrastructure/Persistence --context StoreDbContext
   dotnet ef database update --project ../Infrastructure/Persistence --context StoreIdentityDbContext
   ```

7. **Ensure Redis is Running**
   ```bash
   # If using Docker
   docker run -d -p 6379:6379 redis:latest
   
   # Or start your local Redis service
   redis-server
   ```

8. **Run the Application**
   ```bash
   dotnet run --project E-Commerce.Web
   ```
   
   The API will be available at:
   - HTTPS: `https://localhost:7236`
   - HTTP: `http://localhost:5012`
   - Swagger UI: `https://localhost:7236/swagger`

### Common Commands

```bash
# Build the solution
dotnet build

# Run with hot reload
dotnet watch run --project E-Commerce.Web

# Create a new migration
dotnet ef migrations add MigrationName --project Infrastructure/Persistence --context StoreDbContext

# Remove last migration
dotnet ef migrations remove --project Infrastructure/Persistence --context StoreDbContext

# Update to specific migration
dotnet ef database update MigrationName --project Infrastructure/Persistence --context StoreDbContext
```

### Environment Variables (Alternative Configuration)

You can also use environment variables or user secrets:

```bash
# Using dotnet user-secrets
dotnet user-secrets init --project E-Commerce.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING" --project E-Commerce.Web
dotnet user-secrets set "StripeSettings:SecretKey" "YOUR_STRIPE_KEY" --project E-Commerce.Web
```

## Testing

Currently, the project does not include automated tests. To test the API:

**Manual Testing:**
1. Navigate to Swagger UI at `https://localhost:7236/swagger`
2. Use the "Authorize" button to add JWT token
3. Test endpoints directly from the UI

**Recommended Test Coverage:**
- Unit tests for services and business logic
- Integration tests for repositories
- API tests for controllers
- End-to-end tests for critical workflows

**Future Testing Setup:**
```bash
# Add test projects (suggested)
dotnet new xunit -n E-Commerce.Tests.Unit
dotnet new xunit -n E-Commerce.Tests.Integration
```

## Folder Structure

```
E-Commerce.Web/
├── Core/                                    # Core business logic layer
│   ├── DomainLayer/                        # Domain entities and contracts
│   │   ├── Contracts/                      # Repository interfaces
│   │   ├── Exceptions/                     # Custom domain exceptions
│   │   └── Models/                         # Domain entities
│   │       ├── BasketModule/              # Basket entities
│   │       ├── IdentityModule/            # User/Auth entities
│   │       ├── OrderModule/               # Order entities
│   │       └── ProductModule/             # Product entities
│   ├── Service/                            # Business logic implementation
│   │   ├── MappingProfiles/               # AutoMapper configurations
│   │   └── Specifications/                # Query specifications
│   └── ServiceAbstracion/                  # Service interfaces
├── Infrastructure/                          # Infrastructure layer
│   ├── Persistence/                        # Data access layer
│   │   ├── Data/                          # DbContexts and configurations
│   │   │   ├── Configurations/            # EF Core entity configs
│   │   │   ├── DataSeed/                  # JSON seed data
│   │   │   └── Migrations/                # EF Core migrations
│   │   ├── Identity/                      # Identity DbContext
│   │   │   └── Migrations/                # Identity migrations
│   │   └── Repositories/                  # Repository implementations
│   └── Presentation/                       # API presentation layer
│       ├── Attributes/                    # Custom attributes (Cache, etc.)
│       └── Controllers/                   # API controllers
├── Shared/                                  # Shared DTOs and models
│   ├── DataTransferObjects/               # DTOs for data transfer
│   └── ErrorModels/                       # Error response models
├── E-Commerce.Web/                          # Web API entry point
│   ├── CustomMiddlewares/                 # Custom middleware
│   ├── Extensions/                        # Service registrations
│   ├── Factories/                         # Response factories
│   ├── Properties/                        # Launch settings
│   ├── wwwroot/Images/Products/           # Product images
│   ├── appsettings.json                   # Configuration
│   └── Program.cs                         # Application entry point
└── README.md                                # This file
```

**Key Files:**
- `Program.cs` - Application entry point, service configuration
- `appsettings.json` - Configuration settings
- `CustomExceptionHandlerMiddleware.cs` - Global exception handling
- `StoreDbContext.cs` - Main database context
- `StoreIdentityDbContext.cs` - Identity database context
- `UnitOfWork.cs` - Unit of Work pattern implementation
- `SpecificationEvaluator.cs` - Dynamic query builder

## Configuration & Secrets

### Required Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=ECommerce;Trusted_Connection=True;TrustServerCertificate=True",
    "IdentityConnection": "Server=YOUR_SQL_SERVER;Database=ECommerce.Identity;Trusted_Connection=True;TrustServerCertificate=True",
    "RedisConnectionString": "YOUR_REDIS_HOST:6379"
  },
  "JWT": {
    "Options": {
      "SecretKey": "YOUR_SECURE_256_BIT_SECRET_KEY_MINIMUM_32_CHARACTERS",
      "Issuer": "https://your-issuer.com/",
      "Audience": "https://your-audience.com/api"
    }
  },
  "Urls": {
    "BaseUrl": "https://your-domain.com/"
  },
  "StripeSettings": {
    "SecretKey": "sk_test_YOUR_STRIPE_SECRET_KEY"
  }
}
```

### Environment-Specific Settings

**Development:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Production (use environment variables):**
```bash
export ConnectionStrings__DefaultConnection="YOUR_PROD_CONNECTION"
export JWT__Options__SecretKey="YOUR_PROD_JWT_SECRET"
export StripeSettings__SecretKey="sk_live_YOUR_LIVE_KEY"
```

### Security Notes
⚠️ **NEVER commit secrets to version control**
- Use user secrets for development: `dotnet user-secrets`
- Use Azure Key Vault / AWS Secrets Manager for production
- Rotate JWT secret keys regularly
- Use separate Stripe keys for test/production

## Screenshots / Demo

### Suggested Screenshots
1. **Swagger API Documentation** (`swagger-ui-screenshot.png`)
   - Caption: "Complete API documentation with JWT authentication support"

2. **Product Catalog with Pagination** (`products-list.png`)
   - Caption: "Paginated product listing with filtering and sorting"

3. **Order Management** (`order-details.png`)
   - Caption: "Detailed order view with items and delivery information"

4. **Authentication Flow** (`jwt-auth-flow.png`)
   - Caption: "JWT token-based authentication process"

5. **Payment Integration** (`stripe-payment.png`)
   - Caption: "Stripe payment intent creation and processing"

### Demo Endpoints
```bash
# Register a new user
POST /api/Authentication/register

# Get all products (paginated)
GET /api/Products?PageNumber=1&PageSize=10

# Add item to basket
POST /api/Baskets

# Create an order
POST /api/Orders

# Create payment intent
POST /api/Payments/{basketId}
```

## Future Improvements

### Short-term Roadmap
- [ ] Implement comprehensive unit and integration tests
- [ ] Add API versioning (v1, v2)
- [ ] Implement rate limiting
- [ ] Add product reviews and ratings
- [ ] Implement wish list functionality
- [ ] Add email notifications (order confirmation, password reset)
- [ ] Implement inventory management
- [ ] Add product variant support (size, color)

### Medium-term Roadmap
- [ ] Implement real-time notifications using SignalR
- [ ] Add GraphQL endpoint as alternative to REST
- [ ] Implement advanced search with Elasticsearch
- [ ] Add multi-language support (i18n)
- [ ] Implement product recommendations engine
- [ ] Add analytics and reporting dashboard
- [ ] Implement webhook handlers for Stripe events
- [ ] Add file upload for product images to cloud storage

### Long-term Roadmap
- [ ] Microservices migration (separate services for products, orders, payments)
- [ ] Event-driven architecture with message queues (RabbitMQ/Azure Service Bus)
- [ ] Implement CQRS with MediatR
- [ ] Add distributed tracing (OpenTelemetry)
- [ ] Kubernetes deployment configuration
- [ ] Multi-tenant support
- [ ] Advanced fraud detection
- [ ] Mobile app integration (BFF pattern)

## Contribution Guidelines

We welcome contributions! Please follow these guidelines:

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes**
   - Follow existing code style and conventions
   - Add XML documentation comments for public APIs
   - Update README if adding new features
4. **Commit your changes**
   ```bash
   git commit -m "Add: brief description of your changes"
   ```
5. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```
6. **Open a Pull Request**
   - Provide a clear description of changes
   - Reference any related issues
   - Ensure all checks pass

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Keep methods focused and small (Single Responsibility)
- Add comments for complex logic
- Use async/await for I/O operations

### Pull Request Process
- PRs require at least one review before merging
- All tests must pass
- Code coverage should not decrease
- Update documentation for public API changes

### Reporting Issues
- Use GitHub Issues to report bugs
- Provide detailed reproduction steps
- Include environment information (.NET version, OS, etc.)

## License

This project is licensed under the **MIT License**.

**Rationale:** The MIT License is permissive, allows commercial use, and encourages open collaboration while providing liability protection for contributors.

```
MIT License

Copyright (c) 2024 [Your Name/Organization]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

[Full MIT License text...]
```

## Notes / Known Issues

- ⚠️ **Stripe Secret Key Placeholder**: The `appsettings.json` contains "PLACEHOLDER" for Stripe key - must be replaced before running
- ⚠️ **Admin Authorization**: The `ProductsController.GetAllProducts` has `[Authorize("Admin")]` which may restrict access - consider if this should be public
- 📝 **Test Coverage**: No automated tests are currently implemented
- 📝 **Migration Consistency**: There appear to be duplicate migration files (20251008 and 20251009 timestamps) - review migration history
- 📝 **Repository Pattern**: The `UnitOfWork.GetRepository` method has a potential bug - it sets `_repositories["typeName"]` instead of `_repositories[typeName]`

## Author & Contact

**Project Maintainer:** [Ahmed Khaled]

[![GitHub](https://img.shields.io/badge/GitHub-Profile-black?logo=github)](https://github.com/ahmed-khalid2004)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-blue?logo=linkedin)](https://www.linkedin.com/in/ahmed-khalid-5b6349259/)
[![Email](https://img.shields.io/badge/Email-Contact-red?logo=gmail)](mailto:engahmedkhalid3s@gmail.com)

---

### Acknowledgments
- Built with ASP.NET Core and Entity Framework Core
- Payment processing by [Stripe](https://stripe.com)
- Caching powered by [Redis](https://redis.io)

### Support
If you find this project helpful, please ⭐ star the repository!

---

**What I Looked At:**
- Analyzed the complete project structure including all `.cs`, `.csproj`, `.json` configuration files, and migrations
- Reviewed the layered architecture (Core/DomainLayer, Service, Infrastructure/Persistence, Presentation)
- Examined key implementations: controllers, services, repositories, middleware, entity configurations, and Stripe payment integration
