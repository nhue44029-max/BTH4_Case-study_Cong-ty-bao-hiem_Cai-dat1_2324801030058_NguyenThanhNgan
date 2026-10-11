using Microsoft.EntityFrameworkCore;

namespace PolicyService.Data;

public sealed class PolicyServiceDbContext(DbContextOptions<PolicyServiceDbContext> options) : DbContext(options)
{
    public DbSet<PolicyServiceRecord> Records => Set<PolicyServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicyServiceRecord>(entity =>
        {
            entity.ToTable("policyservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class PolicyServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IPolicyServiceRepository
{
    Task<IReadOnlyList<PolicyServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PolicyServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PolicyServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class PolicyServiceRepository(PolicyServiceDbContext dbContext) : IPolicyServiceRepository
{
    public async Task<IReadOnlyList<PolicyServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<PolicyServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(PolicyServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class PolicyServiceDataExtensions
{
    public static IServiceCollection AddPolicyServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<PolicyServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPolicyServiceRepository, PolicyServiceRepository>();
        return services;
    }
}
