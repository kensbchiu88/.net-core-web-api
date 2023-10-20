using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<UploadInfoDbContext>));

                if(dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                
                services.AddDbContext<UploadInfoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryEmployeeTest");
                });
               
            });

            builder.UseEnvironment("Development");
        }
    }
}
