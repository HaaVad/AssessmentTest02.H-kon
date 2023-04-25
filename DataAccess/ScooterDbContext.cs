using AssessmentTest02.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace AssessmentTest02.DataAccess
{
    public class ScooterDbContext : DbContext
    {
        public DbSet<Scooter> Scooter => Set<Scooter>();
        public DbSet<Brand> Brand => Set<Brand>();
        public DbSet<Ride> Ride => Set<Ride>();
        public DbSet<Customer> Customer => Set<Customer>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
	        optionsBuilder.UseSqlServer(@"Server = (localdb)\MSSQLLocalDB; " +
	                                    "Database = ScooterDB; " +
	                                    "Trusted_Connection = True;");
        }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes()) {
				MethodInfo? method = entityType.ClrType.GetMethod("OnModelCreating", BindingFlags.Static | BindingFlags.NonPublic);
				method?.Invoke(null, new object[] { modelBuilder });
			}
		}


		public void ResetIdentityStartingValue(string tableName, int startingValue = 1)
		{
			Database.ExecuteSqlRaw("IF EXISTS(SELECT * FROM sys.identity_columns " +
				$"WHERE OBJECT_NAME(OBJECT_ID) = '{tableName}' AND last_value IS NOT NULL) " +
				$"DBCC CHECKIDENT({tableName}, RESEED, {startingValue - 1});");
		}
	}
}
