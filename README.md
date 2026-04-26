# MinBudget - Full Stack Budget Management App

## 📊 Project Overview

MinBudget is a modern full-stack budget management application built with:
- **Frontend:** Blazor WebAssembly (WASM)
- **Backend:** ASP.NET Core Web API with Clean Architecture
- **Database:** SQL Server (LocalDB or Azure SQL)
- **Auth:** JWT Tokens + ASP.NET Core Identity

## 📁 Project Structure

```
MinBudget/
├── Frontend/                       ← Blazor WASM Frontend
│   └── MinBudget/
│       ├── Pages/                  (Razor pages)
│       ├── Components/             (Razor components)
│       ├── Services/               (API client, etc.)
│       ├── Domain/                 (Local domain logic)
│       ├── Layout/                 (UI layouts)
│       ├── wwwroot/                (Static assets)
│       ├── Program.cs              (Frontend setup)
│       └── MinBudget.csproj
│
├── Backend/                        ← Clean Architecture Backend
│   ├── MinBudget.Domain/           (Core entities & interfaces)
│   │   ├── Entities/
│   │   │   ├── Expense.cs
│   │   │   ├── Income.cs
│   │   │   └── ApplicationUser.cs (stub)
│   │   └── Interfaces/
│   │       ├── IExpenseRepository.cs
│   │       └── IIncomeRepository.cs
│   │
│   ├── MinBudget.Application/      (Business logic & DTOs)
│   │   ├── Services/
│   │   │   ├── ExpenseService.cs
│   │   │   ├── IncomeService.cs
│   │   │   └── AuthService.cs
│   │   └── DTOs/
│   │       ├── ExpenseDto.cs
│   │       ├── IncomeDto.cs
│   │       └── AuthDto.cs
│   │
│   ├── MinBudget.Infrastructure/   (Database & repositories)
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── ApplicationUser.cs
│   │   │   └── Migrations/
│   │   │       └── InitialCreate.cs
│   │   └── Repositories/
│   │       ├── ExpenseRepository.cs
│   │       └── IncomeRepository.cs
│   │
│   ├── MinBudget.Api/              (REST API)
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── ExpensesController.cs
│   │   │   └── IncomesController.cs
│   │   ├── Program.cs              (Backend setup + DI)
│   │   ├── appsettings.json        (Config)
│   │   └── MinBudget.Api.csproj
│   │
│   └── [.csproj files for projects above]
│
├── MinBudget.sln                   (Solution file)
├── BACKEND.md                      (Backend docs)
└── README.md                       (This file)
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 / Rider / VS Code
- SQL Server LocalDB or cloud SQL

### Running the API

```bash
cd Backend/MinBudget.Api
dotnet run
```

API available at:
- **Base URL:** https://localhost:7001
- **Swagger UI:** https://localhost:7001/swagger

### Running the Frontend

```bash
cd Frontend/MinBudget
dotnet run
```

Frontend available at: https://localhost:7000

### Both Together

```bash
# Terminal 1 - Backend
cd Backend/MinBudget.Api && dotnet run

# Terminal 2 - Frontend  
cd Frontend/MinBudget && dotnet run
```

## 🔐 API Endpoints

### Authentication
```
POST   /api/auth/register          Register new user
POST   /api/auth/login             Login (returns JWT token)
```

### Expenses
```
GET    /api/expenses/month/{year}/{month}   Get month expenses
GET    /api/expenses/{id}                    Get single expense
POST   /api/expenses                         Create expense
PUT    /api/expenses/{id}                    Update expense
DELETE /api/expenses/{id}                    Delete expense
```

### Incomes
```
GET    /api/incomes/month/{year}/{month}    Get month incomes
GET    /api/incomes/{id}                     Get single income
POST   /api/incomes                          Create income
PUT    /api/incomes/{id}                     Update income
DELETE /api/incomes/{id}                     Delete income
```

## 🔧 Configuration

Edit `Backend/MinBudget.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MinBudgetDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "MinBudget",
    "Audience": "MinBudgetClient"
  }
}
```

## 📚 Authentication Flow

1. **Register/Login:** User sends credentials to `/api/auth/register` or `/api/auth/login`
2. **JWT Token:** API returns JWT token valid for 7 days
3. **Store Token:** Frontend stores token in browser localStorage
4. **Use Token:** Include in header: `Authorization: Bearer {token}`
5. **Protected Endpoints:** API validates token and extracts userId

Example header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## 🧪 Testing with Swagger

1. Start API: `cd Backend/MinBudget.Api && dotnet run`
2. Open: https://localhost:7001/swagger
3. **Register** a user via `/auth/register`
4. **Login** via `/auth/login` to get token
5. Click **Authorize** button, paste token: `Bearer {your-token}`
6. Test protected endpoints

## 📦 Tech Stack

### Frontend
- Blazor WebAssembly 8.0
- Bootstrap CSS
- HttpClient for API calls

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- Microsoft.AspNetCore.Identity
- JWT Bearer Authentication
- Swagger/OpenAPI

### Database
- SQL Server (LocalDB development, Azure SQL production)

## 🏗️ Clean Architecture Benefits

- **Separation of Concerns:** Each layer has specific responsibility
- **Testability:** Services can be unit tested independently
- **Maintainability:** Easy to locate and modify features
- **Scalability:** Adding new features doesn't impact existing code
- **Reusability:** Services/DTOs shared across controllers

## 🐛 Troubleshooting

### Build Fails
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Database Issues (Linux/macOS)
LocalDB is Windows-only. Use SQL Server Docker or Azure SQL:

```bash
# Docker SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Update `appsettings.json`:
```json
"DefaultConnection": "Server=localhost,1433;Database=MinBudgetDb;User Id=sa;Password=YourPassword123!;"
```

### Port Already in Use
```bash
# Change port in launchSettings.json or:
dotnet run --urls "https://localhost:8001"
```

## 📖 Documentation

- **Backend Details:** See `BACKEND.md`
- **API Spec:** Available at `/swagger` when API runs
- **Code Comments:** Inline comments for complex logic

## 🎯 Next Steps

- [ ] Implement frontend login component
- [ ] Add expense/income management UI
- [ ] Implement dashboard with charts
- [ ] Add budget categories
- [ ] Deploy to Azure
- [ ] Add push notifications
- [ ] Mobile app (MAUI)

## 📝 License

Private project. All rights reserved.

## 👤 Author

MinBudget Team

