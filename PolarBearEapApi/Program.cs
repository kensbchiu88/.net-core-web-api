using PolarBearEapApi.Repository;
using Serilog;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

/*
Log.Logger = new LoggerConfiguration()
    //Serilog�n�g�J���̧C���Ŭ�Information
    .MinimumLevel.Information()
    //Microsoft.AspNetCore�}�Y�����O������warning
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    //�glog��Logs��Ƨ���log.txt�ɮפ��A�åB�H�Ѭ���찵�ɮפ���
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

    builder.Services.AddDbContext<UploadInfoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyDatabaseConnection")));

    builder.Services.AddSingleton<IMesCommand, NoSuchCommand>();
    builder.Services.AddScoped<IMesCommand, AddBomDataCommand>();
    builder.Services.AddScoped<IMesCommand, GetInputDataCommand>();
    builder.Services.AddScoped<IMesCommand, GetSnBySnFixtureCommand>();
    builder.Services.AddScoped<IMesCommand, UnitProcessCheckCommand>();
    builder.Services.AddScoped<IMesCommand, UnitProcessCommitCommand>();
    builder.Services.AddScoped<IMesCommand, UploadInfosCommand>();
    builder.Services.AddScoped<IMesCommandFactory<IMesCommand>, MesCommandFactory<IMesCommand>>();

    builder.Host.UseSerilog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

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


