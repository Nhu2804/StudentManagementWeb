using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.Security;
using StudentManagementWeb.Core;
using StudentManagementWeb.Infrastructure;
using StudentManagementWeb.Core.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("Thiếu Jwt:SigningKey trong cấu hình.");

builder.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = jwtKey)
    .AddAuthorization(options =>
    {
        // Quyền import điểm: hiện chỉ Admin; sau này thêm AppRoles.Teacher tại đây, một nơi duy nhất.
        options.AddPolicy(AppPolicies.ScoreImport, p => p.RequireRole(AppRoles.Admin));
    });

builder.Services.AddFastEndpoints(options =>
{
    options.Assemblies = [typeof(CoreAssemblyMarker).Assembly];
});

builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "Student Management API";
        s.Version = "v1";
    };
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(options => options.Endpoints.RoutePrefix = "api");

if (app.Environment.IsDevelopment())
    app.UseSwaggerGen();

app.Run();