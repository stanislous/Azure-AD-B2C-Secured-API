namespace IdentityAuthService.Repositories.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : new()
{
    IEnumerable<TEntity> GetAll();
    Task<int> Create(TEntity entity);
    Task<bool> Update(TEntity entity);
    Task<bool> Delete(int id);
}