using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Borda.UnitOfWork
{
    public static class Extensions
    {
        /// <summary>
        /// Register DotnetCore.Cap before UnitOfWork
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TContext">Type of DbContext</typeparam>
        public static void AddUnitOfWork<TContext>(this IServiceCollection services) 
            where TContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        }
    }
}