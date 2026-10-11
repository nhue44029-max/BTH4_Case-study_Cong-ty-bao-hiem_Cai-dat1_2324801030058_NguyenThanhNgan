using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;

public sealed class ChatServiceDbContext(DbContextOptions<ChatServiceDbContext> options) : DbContext(options)
{
    public DbSet<ChatServiceRecord> Records => Set<ChatServiceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatServiceRecord>(entity =>
        {
            entity.ToTable("chatservice_records");
            entity.HasKey(record => record.Id);
            entity.Property(record => record.Name).HasMaxLength(200).IsRequired();
            entity.Property(record => record.CreatedAtUtc).IsRequired();
        });
    }
}

public sealed class ChatServiceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IChatServiceRepository
{
    Task<IReadOnlyList<ChatServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ChatServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ChatServiceRecord record, CancellationToken cancellationToken = default);
}

internal sealed class ChatServiceRepository(ChatServiceDbContext dbContext) : IChatServiceRepository
{
    public async Task<IReadOnlyList<ChatServiceRecord>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Records.AsNoTracking().OrderBy(record => record.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task<ChatServiceRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Records.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

    public async Task AddAsync(ChatServiceRecord record, CancellationToken cancellationToken = default)
    {
        await dbContext.Records.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public static class ChatServiceDataExtensions
{
    public static IServiceCollection AddChatServiceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("Connection string 'PostgreSQL' is not configured.");

        services.AddDbContext<ChatServiceDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IChatServiceRepository, ChatServiceRepository>();
        return services;
    }
}
