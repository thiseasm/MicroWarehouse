using MicroWarehouse.Extensions;
using MicroWarehouse.Infrastructure.Abstractions.DatabaseSettings;

namespace MicroWarehouse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<WarehouseDatabaseSettings>(
                builder.Configuration.GetSection("WarehouseDatabase"));

            builder.Services.AddRegistrations();
            builder.Services.AddControllers();
            builder.Services.AddFluentValidation();
            builder.Services.AddMessaging();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
        }
    }
}
