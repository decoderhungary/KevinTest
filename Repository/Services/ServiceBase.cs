using Repository.Core;

namespace Repository.Services;

public class ServiceBase<T> : IServiceBase<T> where T : class
{
    private readonly AppDbContext _context;

    public ServiceBase(AppDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    public ValueTask<T> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }
    
    public IQueryable<T> GetAsQueryable() => _context.Set<T>().AsQueryable();
}