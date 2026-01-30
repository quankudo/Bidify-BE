using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.Exceptions;
using bidify_be.Extensions;
using bidify_be.Hubs;
using bidify_be.Infrastructure.Context;
using bidify_be.Infrastructure.Hangfire.Jobs;
using bidify_be.Infrastructure.Mapping;
using bidify_be.Infrastructure.Seed;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Repository.Implementations;
using bidify_be.Repository.Interfaces;
using bidify_be.Services;
using bidify_be.Services.Implementations;
using bidify_be.Services.Interfaces;
using CloudinaryDotNet;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Transactions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mySqlConnection");

    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Hangfire
builder.Services.AddHangfire(config =>
{
    config.UseStorage(
        new MySqlStorage(
            builder.Configuration.GetConnectionString("HangfireConnection"),
            new MySqlStorageOptions
            {
                TablesPrefix = "Hangfire_", // tránh trùng table
                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(15),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true // tự tạo table
            }
        )
    );
});

builder.Services.AddHangfireServer();
//-------------------------------------------

builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailServiceImpl>();
builder.Services.AddSingleton<RazorTemplateService>();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    var account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );

    return new Cloudinary(account);
});

builder.Services.AddSignalR();

// Adding Validators
builder.Services.AddValidators();

// Adding Services  
builder.Services.AddScoped<IUserServices, UserServiceImpl>();
builder.Services.AddScoped<ITokenService, TokenServiceImpl>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserServiceImpl>();
builder.Services.AddScoped<ICategoryServices, CategoryServiceImpl>();
builder.Services.AddScoped<IPackageBidService, PackageBidServiceImpl>();
builder.Services.AddScoped<ITagService, TagServiceImpl>();
builder.Services.AddScoped<IAddressService, AddressServiceImpl>();
builder.Services.AddScoped<IGiftTypeService, GiftTypeServiceImpl>();
builder.Services.AddScoped<IGiftService, GiftServiceImpl>();
builder.Services.AddScoped<IVoucherService, VoucherServiceImpl>();
builder.Services.AddScoped<IProductService, ProductServiceImpl>();
builder.Services.AddScoped<IFileStorageService, FileStorageServiceImpl>();
builder.Services.AddScoped<ICloudStorageService, CloudStorageServiceImpl>();
builder.Services.AddScoped<ITopupService, TopupService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransitionPackageBidService, TransitionPackageBidServiceImpl>();
builder.Services.AddScoped<IAuctionService, AuctionServiceImpl>();
builder.Services.AddScoped<INotificationService, NotificationServiceImpl>();
builder.Services.AddScoped<IDashboardService, DashboardServiceImpl>();
builder.Services.AddScoped<IBidsHistoryService, BidsHistoryServiceImpl>();
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<EndAuctionJob>();
builder.Services.AddScoped<ScanEndedAuctionsJob>();

// Adding Repositories and UnitOfWork
builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryImpl>();
builder.Services.AddScoped<IPackageBidRepository, PackageBidRepositoryImpl>();
builder.Services.AddScoped<ITagRepository, TagRepositoryImpl>();
builder.Services.AddScoped<IAddressRepository, AddressRepositoryImpl>();
builder.Services.AddScoped<IGiftTypeRepository, GiftTypeRepositoryImpl>();
builder.Services.AddScoped<IGiftRepository, GiftRepositoryImpl>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepositoryImpl>();
builder.Services.AddScoped<IProductRepository, ProductRepositoryImpl>();
builder.Services.AddScoped<IFileStorageRepository, FileStorageRepository>();
builder.Services.AddScoped<ITopupTransactionRepository, TopupTransactionRepositoryImpl>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepositoryImpl>();
builder.Services.AddScoped<ITransitionPackageBidRepository, TransitionPackageBidRepositoryImpl>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepositoryImpl>();
builder.Services.AddScoped<INotificationRepository, NotificationRepositoryImpl>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepositoryImpl>();
builder.Services.AddScoped<IBidsHistoryRepository, BidsHistoryRepositoryImpl>();
builder.Services.AddScoped<IOrderRepository, OrderRepositoryImpl>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Adding Jwt from extension method
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureCors();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<ScanEndedAuctionsJob>(
        "scan-ended-auctions",
        job => job.ExecuteAsync(),
        Cron.Minutely
    );
}
//Khởi tạo tài khoản admin nếu chưa có
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DatabaseSeeder.InitializeAsync(services);
}

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.MapHub<AppHub>("/hubs/app");

app.Run();
