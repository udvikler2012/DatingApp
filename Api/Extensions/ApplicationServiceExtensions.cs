using System;
using Api.Data;
using Api.Interfaces;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
        services.AddControllers();
services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
});
services.AddCors();
services.AddScoped<ITokenService, TokenService>();

return services;
    }

}
