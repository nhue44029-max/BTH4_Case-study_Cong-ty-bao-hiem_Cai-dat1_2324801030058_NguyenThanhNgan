using Microsoft.EntityFrameworkCore;

namespace ProductService.Data;

public sealed class ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : DbContext(options)
{
    public DbSet<ProductServiceRecord> Records => Set<ProductServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductServiceRecord>(entity =>
        {
            entity.ToTable("productservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class ProductServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IProductServiceRepository
{
    Task<IReadOnlyList<ProductServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ProductServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class ProductServiceRepository(ProductServiceDbContext dbContext) : IProductServiceRepository
{
    public async Task<IReadOnlyList<ProductServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<ProductServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(ProductServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class ProductServiceDataExtensions
{
    public static IServiceCollection AddProductServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<ProductServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IProductServiceRepository, ProductServiceRepository>();
        return services;
    }
}
