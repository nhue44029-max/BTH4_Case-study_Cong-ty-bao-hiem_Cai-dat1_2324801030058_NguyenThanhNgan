using Microsoft.EntityFrameworkCore;

namespace PricingService.Data;

public sealed class PricingServiceDbContext(DbContextOptions<PricingServiceDbContext> options) : DbContext(options)
{
    public DbSet<PricingServiceRecord> Records => Set<PricingServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PricingServiceRecord>(entity =>
        {
            entity.ToTable("pricingservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class PricingServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IPricingServiceRepository
{
    Task<IReadOnlyList<PricingServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PricingServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PricingServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class PricingServiceRepository(PricingServiceDbContext dbContext) : IPricingServiceRepository
{
    public async Task<IReadOnlyList<PricingServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<PricingServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(PricingServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class PricingServiceDataExtensions
{
    public static IServiceCollection AddPricingServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<PricingServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPricingServiceRepository, PricingServiceRepository>();
        return services;
    }
}
