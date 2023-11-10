using Serilog;
using Microsoft.EntityFrameworkCore;
using FIT.MES.Service;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Middlewares;
using PolarBearEapApi.Infra;
using PolarBearEapApi.Infra.Services;
using Microsoft.AspNetCore.Diagnostics;
using PolarBearEapApi.PublicApi.Filters;

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

    //add context 
    builder.Services.AddDbContext<UploadInfoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));
    builder.Services.AddDbContext<EapTokenDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection"))) ;
    builder.Services.AddDbContext<LearnFileAlterWarningDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));
    builder.Services.AddDbContext<StoreProcedureDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MesDatabaseConnection")));

    //add adaptor
    builder.Services.AddScoped<ITokenRepository, DbTokenRepository>();
    builder.Services.AddScoped<IUploadInfoRepository, DbUploadInfoRepository>();
    builder.Services.AddScoped<IMesService, FitMesService>();
    builder.Services.AddScoped<EquipmentService>();
    builder.Services.AddScoped<ILearnFileAlterWarningRepository, DbLearnFileAlterWarningRepository>();
    builder.Services.AddScoped<IStoredProcedureResultRepository, SqlServerStoredProcedureResultRepository>();


    //add application service
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
    builder.Services.AddScoped<IMesCommand, GetSnBySmtSnCommand>();
    builder.Services.AddScoped<ILearnFileAlterWarningService, LearnFileAlterWarningService>();
    builder.Services.AddScoped<IMesCommand, BindSnFixtureSnCommand>();
    builder.Services.AddScoped<IMesCommand, GetSnlistByFixturesnCommand>();
    builder.Services.AddScoped<IMesCommand, SnLinkWoCommand>();
    builder.Services.AddScoped<IMesCommand, GenerateSnByWoCommand>();
    builder.Services.AddScoped<IMesCommand, GetInvalidtimeBySnCommand>();
    builder.Services.AddScoped<IMesCommand, UnbindSnFixturesnCommand>();
    builder.Services.AddScoped<IMesCommand, HoldSnlistCommitCommand>();
    builder.Services.AddScoped<IMesCommand, GetSnByRawsnCommand>();

    builder.Services.AddScoped<IMesCommand, WeightCheckCommitCommand>();
    builder.Services.AddScoped<IMesCommand, GetCountryandqtyByWo>();
    builder.Services.AddScoped<IMesCommand, GetControlTimeStartCommand>();
    builder.Services.AddScoped<IMesCommand, GetCountryPnDataCommand>();
    builder.Services.AddScoped<IMesCommand, TrackingChangeSectionCommand>();
    builder.Services.AddScoped<IMesCommand, GetFgLabeCommandl>();

    builder.Services.AddSingleton<IConfigCacheService, ConfigCacheService>();
    builder.Services.AddSingleton<IEmailService, EmailService>();

    //Filter
    builder.Services.AddScoped<SimpleResponseRewriteActionFilter>(); 

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

public partial class Program { }


