namespace DragonNutrex.App.Interfaces;

public interface IRepository<T>
{
    void Create(T entity);
    void Update(T entity);
    void Delete(Guid id);
    List<T> GetAll();
    Task<List<T>> GetAllAsync(); 
    T? GetById(Guid id);
}