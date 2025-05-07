<h1 align="center">k-tea</h1>

<br/>

## Technologies and frameworks used:

- ASP.NET Core
- Entity Framework Core
- ASP.NET Identity Core
- Angular 18.2.0
- NG-ZORRO (Angular UI component library)
- Ngx-echarts
- Bootstrap
- Cloudinary for upload musics and pictures
- Database Connection via Docker
- Redis
- Store Produce
- Unit Test

<br/>

## Main features:

- Manage item, category, discount, role, user,...
- Integrated Google’s Gemini AI API to provide intelligent menu recommendations.
- Payment with VNPay
- Dynamic Role-Based
- Excel template export/import functionality for item management (add and update many item using Excel).
- Token-based Authentication (Access & Refresh Tokens)

<br/>

## Project Preview

> Database Screenshot Preview



> Client Screenshot Preview


<br/>

> Admin Screenshot Preview



<br/>

## Visual Studio 2022 and SQL Server

#### Prerequisites

- SQL Server
- Visual Studio 2022 and .NET 8

#### Steps to run

- Update the connection string in appsettings.json in API
- Build the whole solution.
- Open the Package Manager Console Window and make sure that Infrastructure\KM.Infrastructure is selected as the Default project. Then type "Update-Database" then press "Enter". This action will create the database schema.
- In Visual Studio, press "Control + F5".
- Can access admin page with account: username: admin, password: Admin_123



## License

Copyright (c) 2025 imkhoanguyen 
