using Microsoft.EntityFrameworkCore;

namespace PaymentService.Data;

public sealed class PaymentServiceDbContext(DbContextOptions<PaymentServiceDbContext> options) : DbContext(options)
{
    public DbSet<PaymentServiceRecord> Records => Set<PaymentServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentServiceRecord>(entity =>
        {
            entity.ToTable("paymentservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class PaymentServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IPaymentServiceRepository
{
    Task<IReadOnlyList<PaymentServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PaymentServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PaymentServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class PaymentServiceRepository(PaymentServiceDbContext dbContext) : IPaymentServiceRepository
{
    public async Task<IReadOnlyList<PaymentServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<PaymentServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(PaymentServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class PaymentServiceDataExtensions
{
    public static IServiceCollection AddPaymentServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<PaymentServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPaymentServiceRepository, PaymentServiceRepository>();
        return services;
    }
}
