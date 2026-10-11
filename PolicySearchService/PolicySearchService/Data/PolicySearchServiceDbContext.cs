using Microsoft.EntityFrameworkCore;

namespace PolicySearchService.Data;

public sealed class PolicySearchServiceDbContext(DbContextOptions<PolicySearchServiceDbContext> options) : DbContext(options)
{
    public DbSet<PolicySearchServiceRecord> Records => Set<PolicySearchServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicySearchServiceRecord>(entity =>
        {
            entity.ToTable("policysearchservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class PolicySearchServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IPolicySearchServiceRepository
{
    Task<IReadOnlyList<PolicySearchServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PolicySearchServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PolicySearchServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class PolicySearchServiceRepository(PolicySearchServiceDbContext dbContext) : IPolicySearchServiceRepository
{
    public async Task<IReadOnlyList<PolicySearchServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<PolicySearchServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(PolicySearchServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class PolicySearchServiceDataExtensions
{
    public static IServiceCollection AddPolicySearchServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<PolicySearchServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPolicySearchServiceRepository, PolicySearchServiceRepository>();
        return services;
    }
}
