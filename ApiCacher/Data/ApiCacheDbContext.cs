using Microsoft.EntityFrameworkCore;

namespace ApiCacher.Data
{
	public class ApiCacheDbContext : DbContext
	{
		public ApiCacheDbContext(DbContextOptions<ApiCacheDbContext> options)
		: base(options)
		{
		}

		public DbSet<ApiCacheModel> CachedRequests { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ApiCacheModel>()
				.HasIndex(x => x.Url);
		}
	}
}
