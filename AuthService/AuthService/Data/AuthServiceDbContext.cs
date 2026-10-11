using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public sealed class AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : DbContext(options)
{
    public DbSet<AuthServiceRecord> Records => Set<AuthServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthServiceRecord>(entity =>
        {
            entity.ToTable("authservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class AuthServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IAuthServiceRepository
{
    Task<IReadOnlyList<AuthServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AuthServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class AuthServiceRepository(AuthServiceDbContext dbContext) : IAuthServiceRepository
{
    public async Task<IReadOnlyList<AuthServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<AuthServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(AuthServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class AuthServiceDataExtensions
{
    public static IServiceCollection AddAuthServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<AuthServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAuthServiceRepository, AuthServiceRepository>();
        return services;
    }
}
