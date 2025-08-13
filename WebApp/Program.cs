using FluentValidation;
using HomeApi.Configuration;
using HomeApi.Contracts.Models.Devices;
using HomeApi.Contracts.Models.Rooms;
using HomeApi.Contracts.Validators;
using HomeApi.Data;
using HomeApi.Data.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace HomeApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HomeApi",
                Version = "v1"
            });
        });

       
        builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
        builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();

        builder.Services.AddScoped<IValidator<AddDeviceRequest>, AddDeviceRequestValidator>();
        builder.Services.AddScoped<IValidator<AddRoomRequest>, AddRoomRequestValidator>();
        builder.Services.AddScoped<IValidator<EditDeviceRequest>, EditDeviceRequestValidator>();

        builder.Services.AddFluentValidationAutoValidation();

        builder.Services.AddAutoMapper((v) => v.AddProfile(new MappingProfile()));

        //builder.Services.Configure<HomeOptions>(builder.Configuration.
        //    AddJsonFile("appsettings.json").
        //    AddJsonFile("appsettings.Development.json").
        //    AddJsonFile("HomeOptions.json").Build());

        string connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<HomeApiContext>(options => options.UseSqlServer(connection), ServiceLifetime.Singleton);

        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();



        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }


        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeApi v1"));

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}