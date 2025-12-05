using bidify_be.Domain.Contracts;
using bidify_be.Domain.Entities;
using bidify_be.DTOs.Auth;
using bidify_be.DTOs.Users;
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
using bidify_be.Validators.Auth;
using bidify_be.Validators.Users;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

// Adding Validators
builder.Services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();
builder.Services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

// Adding Services  
builder.Services.AddScoped<IUserServices, UserServiceImpl>();
builder.Services.AddScoped<ITokenService, TokenServiceImpl>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserServiceImpl>();

// Adding Repositories and UnitOfWork
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryImpl>();
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
