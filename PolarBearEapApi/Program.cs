using PolarBearEapApi.Repository;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Services;
using PolarBearEapApi.Commons.Middlewares;
using Microsoft.Extensions.Caching.Memory;
using PolarBearEapApi.Commons;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

/*
Log.Logger = new LoggerConfiguration()
    //Serilog要寫入的最低等級為Information
    .MinimumLevel.Information()
    //Microsoft.AspNetCore開頭的類別等極為warning
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //寫log到Logs資料夾的log.txt檔案中，並且以天為單位做檔案分割
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
*/

try
{
    Log.Information("----Starting Web Host----");
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();

    builder.Services.AddDbContext<UploadInfoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));
    builder.Services.AddDbContext<EapTokenDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));

    builder.Services.AddScoped<ITokenService, DbTokenService>();
    builder.Services.AddScoped<IMesCommandFactory<IMesCommand>, MesCommandFactory<IMesCommand>>();
    builder.Services.AddSingleton<IMesCommand, NoSuchCommand>();
    builder.Services.AddScoped<IMesCommand, AddBomDataCommand>();
    builder.Services.AddScoped<IMesCommand, GetInputDataCommand>();
    builder.Services.AddScoped<IMesCommand, GetSnBySnFixtureCommand>();
    builder.Services.AddScoped<IMesCommand, UnitProcessCheckCommand>();
    builder.Services.AddScoped<IMesCommand, UnitProcessCommitCommand>();
    builder.Services.AddScoped<IMesCommand, UploadInfosCommand>();
    builder.Services.AddScoped<IMesCommand, LoginCommand>();
    builder.Services.AddScoped<IMesCommand, BindCommand>();

    builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<LoggerMiddleware>();
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseMiddleware<TokenMiddleware>();
    

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally { Log.CloseAndFlush(); }


