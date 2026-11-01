using Microsoft.EntityFrameworkCore;

namespace DocumentService.Data;

public sealed class DocumentServiceDbContext(DbContextOptions<DocumentServiceDbContext> options) : DbContext(options)
{
    public DbSet<DocumentServiceRecord> Records => Set<DocumentServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentServiceRecord>(entity =>
        {
            entity.ToTable("documentservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class DocumentServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IDocumentServiceRepository
{
    Task<IReadOnlyList<DocumentServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DocumentServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DocumentServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class DocumentServiceRepository(DocumentServiceDbContext dbContext) : IDocumentServiceRepository
{
    public async Task<IReadOnlyList<DocumentServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<DocumentServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(DocumentServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class DocumentServiceDataExtensions
{
    public static IServiceCollection AddDocumentServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<DocumentServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IDocumentServiceRepository, DocumentServiceRepository>();
        return services;
    }
}
