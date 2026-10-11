using Microsoft.EntityFrameworkCore;

namespace DashboardService.Data;

public sealed class DashboardServiceDbContext(DbContextOptions<DashboardServiceDbContext> options) : DbContext(options)
{
    public DbSet<DashboardServiceRecord> Records => Set<DashboardServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DashboardServiceRecord>(entity =>
        {
            entity.ToTable("dashboardservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class DashboardServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IDashboardServiceRepository
{
    Task<IReadOnlyList<DashboardServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DashboardServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DashboardServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class DashboardServiceRepository(DashboardServiceDbContext dbContext) : IDashboardServiceRepository
{
    public async Task<IReadOnlyList<DashboardServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<DashboardServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(DashboardServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class DashboardServiceDataExtensions
{
    public static IServiceCollection AddDashboardServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<DashboardServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IDashboardServiceRepository, DashboardServiceRepository>();
        return services;
    }
}
