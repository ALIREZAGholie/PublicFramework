using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Webgostar.Framework.Base.BaseModels;
using Webgostar.Framework.Infrastructure.MediatR;

namespace Webgostar.Framework.Infrastructure.Contexts;

public abstract class EfBaseContext : DbContext
{
    private readonly ICustomPublisher _publisher;
    private readonly Assembly _assembly;

    /// <summary>
    /// Base Db Context
    /// </summary>
    /// <param name="options"></param>
    /// <param name="publisher"></param>
    /// <param name="domainAssembly">Assembly IEntityTypeConfiguration</param>
    protected EfBaseContext(DbContextOptions options, ICustomPublisher publisher, Assembly domainAssembly) : base(options)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _assembly = domainAssembly ?? throw new ArgumentNullException(nameof(domainAssembly));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            List<AggregateRoot> modifiedEntities = GetModifiedEntities();

            await PublishEvents(modifiedEntities, cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public List<AggregateRoot> GetModifiedEntities()
    {
        try
        {
            return ChangeTracker.Entries<AggregateRoot>()
                .Where(x => x.State != EntityState.Detached)
                .Select(c => c.Entity)
                .Where(c => c.DomainEvents.Any())
                .ToList();
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task PublishEvents(List<AggregateRoot> modifiedEntities, CancellationToken cancellationToken)
    {
        try
        {
            if (modifiedEntities?.Any() != true) return;

            foreach (AggregateRoot entity in modifiedEntities)
            {
                List<BaseDomainEvent> events = [.. entity.DomainEvents];

                foreach (BaseDomainEvent domainEvent in events)
                {
                    entity.DomainEvents.Remove(domainEvent);
                    await _publisher.Publish(domainEvent, PublishStrategy.Async, cancellationToken);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            base.OnConfiguring(optionsBuilder);
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        try
        {
            builder.ApplyConfigurationsFromAssembly(_assembly);

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public DbSet<SystemError> SystemError { get; set; }
}