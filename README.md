# 🔨 Bidify – Backend Service

Backend service cho hệ thống đấu giá trực tuyến thời gian thực (Realtime Online Auction Platform),
xây dựng bằng ASP.NET Core Web API, thiết kế theo Clean Architecture, hướng tới production-ready,
dễ mở rộng và bảo trì.

Project phục vụ mô hình SPA (React) hoặc Mobile App.

---

## Tổng quan hệ thống

Bidify là nền tảng đấu giá trực tuyến hỗ trợ nhiều vai trò người dùng:

- Guest: Xem phiên đấu giá
- Buyer: Tham gia đấu giá, thanh toán, quản lý ví
- Seller: Tạo sản phẩm, phiên đấu giá
- Admin: Quản trị hệ thống, phê duyệt, báo cáo

### Chức năng cốt lõi
- Đấu giá realtime với độ trễ < 1s (SignalR)
- Quản lý ví và gói Bids
- Thanh toán và xử lý đơn hàng
- Khiếu nại và thông báo hệ thống
- Cơ chế giới thiệu – nhận thưởng
- Dashboard và báo cáo cho Admin

---

## Kiến trúc tổng thể

React (SPA)
  |
  | REST API / SignalR
  v
ASP.NET Core Web API
  |
  | EF Core / Dapper
  v
MySQL Database

- REST API cho nghiệp vụ chính
- SignalR cho đấu giá realtime và notification
- Hangfire xử lý background jobs
- Stateless API (JWT)

---

## Công nghệ sử dụng

Backend:
- ASP.NET Core 8
- Entity Framework Core
- ASP.NET Identity + JWT
- SignalR
- Hangfire
- AutoMapper
- FluentValidation
- MySQL (Pomelo)

Hạ tầng & tích hợp:
- Docker và Docker Compose
- Cloudinary (lưu trữ hình ảnh)
- Mailtrap (email testing)
- VNPay (payment gateway)
- GitHub Actions (CI – optional)

---

## Cấu trúc source code

├── Controllers        (API endpoints – thin controllers)
├── Domain             (Entities, Enums, Constants)
├── DTOs               (Request / Response models)
├── Services           (Business logic)
├── Repository         (Data access layer)
├── Infrastructure     (DbContext, EF Config, Hangfire, Mapping)
├── Validators         (FluentValidation)
├── Hubs               (SignalR hubs)
├── Exceptions         (Global exception handling)
├── Helpers / Extensions
└── Migrations

Áp dụng Clean Architecture và Separation of Concerns.
Controller không chứa business logic.

---

## Authentication & Authorization

- Xác thực bằng JWT Bearer Token
- Tích hợp ASP.NET Identity
- Role-based authorization:
  - Guest
  - Buyer
  - Seller
  - Admin

Header:
Authorization: Bearer <access_token>

---

## Realtime Auction (SignalR)

Hub: /hubs/app

Broadcast:
- Giá đấu mới
- Kết thúc phiên đấu giá
- Thông báo người dùng

Luồng xử lý:
1. Client join auction room
2. Server validate bid
3. Persist bid history
4. Broadcast realtime

---

## Background Jobs (Hangfire)

Sử dụng Hangfire cho các tác vụ nền:
- Tự động bắt đầu / kết thúc phiên đấu giá
- Xác định người thắng
- Phát thưởng referral
- Gửi email thông báo
- Scan các phiên đấu giá quá hạn

Dashboard: /hangfire

---

## Docker & Docker Compose

Chạy nhanh bằng Docker:
docker-compose up -d

Services:
- api: ASP.NET Core Web API
- mysql: MySQL database
- hangfire: background jobs (shared database)

---

## Cấu hình môi trường

File: appsettings.Development.json

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "ConnectionStrings": {
    "mySqlConnection": "",
    "HangfireConnection": ""
  },

  "JwtSettings": {
    "validIssuer": "BidifyAPI",
    "validAudience": "",
    "expires": 360,
    "key": ""
  },

  "MailSettings": {
    "Host": "",
    "Port": 587,
    "UserName": "",
    "Password": "",
    "From": ""
  },

  "Cloudinary": {
    "CloudName": "",
    "ApiKey": "",
    "ApiSecret": ""
  },

  "Vnpay": {
    "TmnCode": "",
    "HashSecret": "",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "Command": "pay",
    "CurrCode": "VND",
    "Version": "2.1.0",
    "Locale": "vn",
    "PaymentBackReturnUrl": ""
  },

  "TimeZoneId": "SE Asia Standard Time",
  "AllowedHosts": "*"
}

---

## Chạy local (không dùng Docker)

dotnet restore
dotnet ef database update
dotnet run

Swagger:
https://localhost:5001/swagger

---

## Validation & Error Handling

- Validate input bằng FluentValidation
- Global exception handler
- Unified API response format
- Log và trace lỗi rõ ràng

---

## Audit & Transaction Tracking

- Lưu lịch sử đấu giá
- Lưu lịch sử ví
- Lưu lịch sử giao dịch
- Dễ truy vết và đối soát

---

## Convention & Code Style

- Thin Controller
- Service-driven business logic
- Repository + UnitOfWork

Commit message:
feat: implement realtime auction bidding
fix: prevent duplicated wallet transaction

---

## License

MIT License

---

==============================
ARCHITECTURE.md
==============================

Mục tiêu kiến trúc:
- Scalability
- Maintainability
- Separation of Concerns
- Realtime performance
- Production readiness

Kiến trúc Clean Architecture:

Controller
  ↓
Service (Business Logic)
  ↓
Repository
  ↓
Database

Domain Layer:
- Entities
- Enums
- Constants
- Business rules cốt lõi
- Không phụ thuộc EF, SignalR hay ASP.NET Core

Data Access:
- EF Core cho CRUD và transaction
- Dapper cho query phức tạp / dashboard
- UnitOfWork đảm bảo nhất quán dữ liệu

Realtime (SignalR):
- Native .NET
- Scale tốt với Redis backplane
- Flow: Validate → Persist → Broadcast

Background Processing:
- Hangfire
- Persist job vào DB
- Dashboard trực quan

Security:
- JWT stateless
- Role-based authorization
- Validate mọi input
- Không expose entity trực tiếp

Wallet consistency:
- Mọi biến động ví đều ghi log
- Không update trực tiếp số dư
- Tránh race condition khi đấu giá

Deployment:
- Docker hóa toàn bộ stack
- Scale API theo chiều ngang
- DB và Hangfire dùng chung storage

Future improvements:
- Redis cache
- RabbitMQ
- Microservice Auction
- Event-driven architecture
