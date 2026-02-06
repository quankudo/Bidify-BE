# 🔨 Bidify – Backend Service

Backend service cho hệ thống đấu giá trực tuyến thời gian thực (Realtime Online Auction Platform),  
xây dựng bằng ASP.NET Core Web API, thiết kế theo Clean Architecture, hướng tới production-ready,  
dễ mở rộng và bảo trì.

Project phục vụ mô hình SPA (React) hoặc Mobile App.

---

## Tổng quan hệ thống

Bidify hỗ trợ nhiều vai trò:

- Guest: Xem phiên đấu giá  
- Buyer: Tham gia đấu giá, thanh toán, quản lý ví  
- Seller: Tạo sản phẩm, phiên đấu giá  
- Admin: Quản trị hệ thống, phê duyệt, báo cáo  

### Chức năng cốt lõi

- Đấu giá realtime < 1s (SignalR)  
- Quản lý ví & gói Bids  
- Thanh toán & đơn hàng  
- Khiếu nại & thông báo  
- Referral reward  
- Dashboard Admin  

---

## Kiến trúc tổng thể

```text
React (SPA / Mobile)
       │
       │ REST API / SignalR
       ▼
ASP.NET Core Web API
       │
       │ EF Core / Dapper
       ▼
MySQL Database
```

- REST API cho nghiệp vụ  
- SignalR realtime  
- Hangfire background jobs  
- Stateless JWT API  

---

## Công nghệ sử dụng

### Backend

- ASP.NET Core 8  
- EF Core  
- Identity + JWT  
- SignalR  
- Hangfire  
- AutoMapper  
- FluentValidation  
- MySQL (Pomelo)

### Hạ tầng

- Docker & Docker Compose  
- Cloudinary  
- Mailtrap  
- VNPay  
- GitHub Actions  

---

## Cấu trúc source code

```text
src/
├── Controllers/        # API endpoints – thin controllers
├── Domain/             # Entities, Enums, Constants
├── DTOs/               # Request / Response models
├── Services/           # Business logic
├── Repository/         # Data access layer
├── Infrastructure/     # DbContext, EF Config, Hangfire, Mapping
├── Validators/         # FluentValidation
├── Hubs/               # SignalR hubs
├── Exceptions/         # Global exception handling
├── Helpers/
│   └── Extensions/
└── Migrations/
```

**Nguyên tắc:**

- Clean Architecture  
- Separation of Concerns  
- Controller không chứa business logic  

---

## Authentication & Authorization

- JWT Bearer  
- ASP.NET Identity  
- Role-based:

  - Guest  
  - Buyer  
  - Seller  
  - Admin  

Header:

```text
Authorization: Bearer <access_token>
```

---

## Realtime Auction (SignalR)

Hub:

```text
/hubs/app
```

Flow:

```text
Client join
 → Validate bid
 → Persist
 → Broadcast
```

---

## Background Jobs (Hangfire)

- Auto start/end auction  
- Winner calculation  
- Referral reward  
- Email sending  
- Expired auction scan  

Dashboard:

```text
/hangfire
```

---

## Docker

Run:

```bash
docker-compose up -d
```

Services:

- api  
- mysql  
- hangfire  

---

## Cấu hình môi trường

File:

```text
appsettings.Development.json
```

```json
{
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
  }
}
```

---

## Chạy local

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Swagger:

```text
https://localhost:5001/swagger
```

---

## Validation & Error Handling

- FluentValidation  
- Global exception handler  
- Unified response format  
- Logging rõ ràng  

---

## Audit & Tracking

- Bid history  
- Wallet history  
- Transaction history  

---

## Convention

- Thin Controller  
- Service-driven  
- Repository + UnitOfWork  

Commit:

```text
feat: implement realtime auction bidding
fix: prevent duplicated wallet transaction
```

---

## License

MIT License

---

# ARCHITECTURE

## Mục tiêu

- Scalability  
- Maintainability  
- Separation of Concerns  
- Realtime performance  
- Production readiness  

---

## Clean Architecture Flow

```text
Controller
   ↓
Service
   ↓
Repository
   ↓
Database
```

---

## Domain Layer

- Entities  
- Enums  
- Constants  
- Core business rules  
- Không phụ thuộc framework  

---

## Data Access

- EF Core → CRUD  
- Dapper → complex query  
- UnitOfWork → consistency  

---

## Realtime

- SignalR  
- Redis backplane ready  

Flow:

```text
Validate → Persist → Broadcast
```

---

## Background Processing

- Hangfire  
- Persistent jobs  
- Dashboard  

---

## Security

- JWT stateless  
- Role-based  
- Validate input  
- Không expose entity  

---

## Wallet Consistency

- Log mọi biến động  
- Không update số dư trực tiếp  
- Tránh race condition  

---

## Deployment

- Dockerized  
- Horizontal scaling  
- Shared storage  

---

## Future Improvements

- Redis cache  
- RabbitMQ  
- Microservice Auction  
- Event-driven architecture  
