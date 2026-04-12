using Azure.Identity;
using CloudBackend.Data;
using CloudBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    //  var keyVaultName = builder.Configuration["KeyVaultName"];
    var keyVaultName = builder.Configuration["cloud-task-manager-fs"];
   

    if (!string.IsNullOrWhiteSpace(keyVaultName))
    {
        var keyVaultEndpoint = new Uri($"https://{keyVaultName}.vault.azure.net/");
        builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
    }
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["DbConnectionString"]
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Brak konfiguracji połączenia z bazą danych. Ustaw DbConnectionString w Azure Key Vault lub ConnectionStrings:DefaultConnection w środowisku lokalnym.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim())
    .ToArray() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            return;
        }

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cloud API V1");
        c.RoutePrefix = string.Empty;
    });
}


if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        if (!await context.Tasks.AnyAsync())
        {
            context.Tasks.AddRange(
                new CloudTask { Name = "Przejrzeć konfigurację Azure", IsCompleted = false },
                new CloudTask { Name = "Potwierdzić odczyt sekretu z Key Vault", IsCompleted = false });

            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Błąd podczas inicjalizacji bazy danych.");
    }
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.MapControllers();



app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapGet("/", () => "Backend działa");


app.Run();
