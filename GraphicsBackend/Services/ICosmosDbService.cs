using GraphicsBackend.Models;

namespace GraphicsBackend.Services
{
    public interface ICosmosDbService<T> where T : class
    {        
        Task<T> GetAsync(string id,string container);
        Task<IEnumerable<T>> GetAllAsync(string querystring,string container);
        Task AddAsync(T item,string id,string container);
        Task UpdateAsync(string id,T item, string container);
        Task DeleteAsync(string id, string container);
    }
}
