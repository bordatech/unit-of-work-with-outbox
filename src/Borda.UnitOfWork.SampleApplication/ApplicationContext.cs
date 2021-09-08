using Microsoft.EntityFrameworkCore;

namespace Borda.UnitOfWork.SampleApplication
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityAlwaysColumns();
        }

        public DbSet<Person> Persons { get; set; }
    }
}