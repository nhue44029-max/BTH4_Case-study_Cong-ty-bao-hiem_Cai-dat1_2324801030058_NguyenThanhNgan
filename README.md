# Bài thực hành 5 - PricingService và Eureka

Bài này phát triển tiếp từ BTH4, vì vậy tên solution vẫn là `BTH4_2324801030058_ThanhNgan.sln`.

## Task 1: Bổ sung PricingService

Mã nguồn và kiểm thử nằm trong `PricingService/PricingService.Api`, `PricingService/PricingService`, `PricingService/PricingService.Test` và `PricingService/PricingService.IntegrationTest`. Các project đã được thêm vào solution.

Kiểm tra từ thư mục gốc repository:

```powershell
dotnet build .\BTH4_2324801030058_ThanhNgan.sln
dotnet test .\BTH4_2324801030058_ThanhNgan.sln --no-build
```

## Task 2: Eureka

- Cấu hình máy chủ: `eureka-server/src/main/resources/application.yml`.
- Cấu hình Eureka client: `PricingService/PricingService/appsettings.json`.
- Đăng ký client: `PricingService/PricingService/Startup.cs` với `AddDiscoveryClient(Configuration)`.

Project dùng .NET 8 và Steeltoe 3.4. `Program.cs` gọi `Startup.ConfigureServices` và `Startup.Configure`. Steeltoe 3.4 tự khởi động discovery client đã đăng ký; `UseDiscoveryClient()` trong tài liệu cũ không còn cần thiết.

Chạy trên laptop có Docker Desktop và .NET 8:

```powershell
docker compose -f .\scripts\infra.yml up -d --build eureka
dotnet run --project .\PricingService\PricingService\PricingService.csproj --launch-profile http
```

Mở `http://localhost:8761` để xem `PRICINGSERVICE` ở trạng thái `UP`. Kiểm tra API tại `http://localhost:5040/health`.
