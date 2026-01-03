using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Address;
using bidify_be.DTOs.Auction;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Category;
using bidify_be.DTOs.Gift;
using bidify_be.DTOs.GiftType;
using bidify_be.DTOs.PackageBid;
using bidify_be.DTOs.Product;
using bidify_be.DTOs.Tags;
using bidify_be.DTOs.TransitionPackageBid;
using bidify_be.DTOs.Users;
using bidify_be.DTOs.Voucher;
using bidify_be.Exceptions;
using bidify_be.Extensions;
using bidify_be.Infrastructure.Context;
using bidify_be.Infrastructure.Mapping;
using bidify_be.Infrastructure.Seed;
using bidify_be.Infrastructure.UnitOfWork;
using bidify_be.Repository.Implementations;
using bidify_be.Repository.Interfaces;
using bidify_be.Services;
using bidify_be.Services.Implementations;
using bidify_be.Services.Interfaces;
using bidify_be.Validators.Address;
using bidify_be.Validators.Auction;
using bidify_be.Validators.Auth;
using bidify_be.Validators.Category;
using bidify_be.Validators.Gift;
using bidify_be.Validators.GiftType;
using bidify_be.Validators.PackageBid;
using bidify_be.Validators.Product;
using bidify_be.Validators.Tags;
using bidify_be.Validators.TransitionPackageBid;
using bidify_be.Validators.Users;
using bidify_be.Validators.Voucher;
using CloudinaryDotNet;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

// Adding Validators
builder.Services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();
builder.Services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

builder.Services.AddScoped<IValidator<AddCategoryRequest>, AddCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryRequest>, UpdateCategoryRequestValidator>();

builder.Services.AddScoped<IValidator<AddPackageBidRequest>, AddPackageBidRequestValidator>();
builder.Services.AddScoped<IValidator<UpdatePackageBidRequest>, UpdatePackageBidRequestValidator>();

builder.Services.AddScoped<IValidator<AddTagRequest>, AddTagRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateTagRequest>, UpdateTagRequestValidator>();

builder.Services.AddScoped<IValidator<AddAddressRequest>, AddAddressRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateAddressRequest>, UpdateAddressRequestValidator>();

builder.Services.AddScoped<IValidator<AddGiftTypeRequest>, AddGiftTypeRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateGiftTypeRequest>, UpdateGiftTypeRequestValidator>();

builder.Services.AddScoped<IValidator<AddGiftRequest>, AddGiftRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateGiftRequest>, UpdateGiftRequestValidator>();

builder.Services.AddScoped<IValidator<AddVoucherRequest>, AddVoucherRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateVoucherRequest>, UpdateVoucherRequestValidator>();

builder.Services.AddScoped<IValidator<AddProductRequest>, AddProductRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();

builder.Services.AddScoped<IValidator<AddAuctionRequest>, AddAuctionRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateAuctionRequest>, UpdateAuctionRequestValidator>();

builder.Services.AddScoped<IValidator<TransitionPackageBidRequest>, TransitionPackageBidRequestValidator>();

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
builder.Services.AddScoped<IVnPayService, VnPayService>();

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

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Adding AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// Adding Jwt from extension method
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureCors();

var app = builder.Build();

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

app.MapControllers();

app.Run();
