using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Inventory_Product.Application.Abstraction.Persistence;
using Inventory_Product.Application.Products.Queries.GetProducts;
using Inventory_Product.Repository.Repositories;
using Inventory_Product.Repository.Persistence;
using Inventory_Product.Application.Products.Commands.CreateProduct;
using Inventory_Product.Application.Products.Commands.UpdateProduct;
using Inventory_Product.Application.Products.Commands.DeactivateProduct;
using Inventory_Product.Repository.Connections;
using Inventory_Product.Application.Categories.Commands.CreateCategory;
using Inventory_Product.Application.Categories.Commands.DeactivateCategory;
using Inventory_Product.Application.Categories.Commands.UpdateCategory;
using Inventory_Product.Application.Categories.Queries.GetCategories;
using Inventory_Product.Application.Categories.Queries.GetCategoryById;
using Inventory_Product.Application.InventoryMovements.Commands.RegisterInventoryMovement;
using Inventory_Product.Application.InventoryMovements.Queries.GetProductMovements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;
using Inventory_Product.Api.Exceptions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("http://localhost:8080/realms/InventoryRealm/protocol/openid-connect/auth"),
                TokenUrl = new Uri("http://localhost:8080/realms/InventoryRealm/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    ["openid"] = "OpenID Connect authentication",
                    ["profile"] = "Access user profile"
                }
            }
        }
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("oauth2", document)] = ["openid", "profile"]
    });
});
var keycloakAuthority = "http://localhost:8080/realms/InventoryRealm";
var keycloakMetadataAddress = builder.Configuration["Keycloak:MetadataAddress"] ?? $"{keycloakAuthority}/.well-known/openid-configuration";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.MetadataAddress = keycloakMetadataAddress;
        options.Audience = "inventory-product-api";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = keycloakAuthority,
            ValidateAudience = true,
            ValidAudience = "inventory-product-api",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var realmAccess = context.Principal?.FindFirst("realm_access")?.Value;
                var identity = context.Principal?.Identity as ClaimsIdentity;
                if (!string.IsNullOrWhiteSpace(realmAccess) && identity is not null)
                {
                    using var document = JsonDocument.Parse(realmAccess);
                    if (document.RootElement.TryGetProperty("roles", out var roles))
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            if (role.ValueKind == JsonValueKind.String)
                            {
                                identity.AddClaim(
                                    new Claim(
                                        ClaimTypes.Role,
                                        role.GetString()!));
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("inventory-admin"));
    options.AddPolicy("InventoryAccess", policy => policy.RequireRole("inventory-admin", "inventory-user"));
});
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("The database connection string is not configured.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddScoped<GetProductsHandler>();
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<UpdateProductHandler>();
builder.Services.AddScoped<DeactivateProductHandler>();
builder.Services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
builder.Services.AddScoped<ICategoryCommandRepository, CategoryCommandRepository>();
builder.Services.AddScoped<GetCategoriesHandler>();
builder.Services.AddScoped<GetCategoryByIdHandler>();
builder.Services.AddScoped<CreateCategoryHandler>();
builder.Services.AddScoped<UpdateCategoryHandler>();
builder.Services.AddScoped<DeactivateCategoryHandler>();
builder.Services.AddScoped<IInventoryMovementCommandRepository, InventoryMovementCommandRepository>();
builder.Services.AddScoped<IInventoryMovementReadRepository, InventoryMovementReadRepository>();
builder.Services.AddScoped<RegisterInventoryMovementHandler>();
builder.Services.AddScoped<GetProductMovementsHandler>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Product API");
        options.OAuthClientId("inventory-product-client");
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
    });
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();