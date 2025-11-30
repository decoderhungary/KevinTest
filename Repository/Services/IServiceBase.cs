namespace Repository.Services;

public interface IServiceBase<T>: IDisposable where T : class
{
    ValueTask<T> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    IQueryable<T> GetAsQueryable();
}