
---

## **Hacker News API (.NET Core) - README.md**
```markdown
# Hacker News API

This is the back-end API for the Hacker News feed, built using ASP.NET Core. It provides endpoints to fetch the top 200 news stories and caches results for optimal performance.

## Features
- 🖥️ RESTful API using .NET Core
- ⚡ Fetches and caches top 200 stories from Hacker News
- 🔍 Search functionality
- ✅ Swagger API documentation
- 🛠️ Dependency Injection for maintainability
- 🧪 Unit tests with xUnit & Moq

## Tech Stack
- **Framework**: ASP.NET Core 7
- **Language**: C#
- **Caching**: In-Memory Cache
- **Testing**: xUnit & Moq
- **Documentation**: Swagger

## Installation
### Prerequisites
- .NET SDK (7.0)
- Visual Studio / VS Code

### Steps to Run the API
# Clone the repository
git clone https://github.com/saket-kr-jha/Hacker_News_Feed_API.git
cd hacker-news-api

# Restore dependencies
dotnet restore

# Run the API
dotnet run

# Open Swagger UI
http://localhost:5245/swagger/index.html

# Running Unit Tests
dotnet test
