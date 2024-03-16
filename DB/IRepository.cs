using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DB.Repository;

public interface IRepository<T>
{
    Task<T?> GetById(int id);
    Task<IEnumerable<T?>> GetAll(int skip, int take);
    Task Save();
    Task<T?> DeleteById(int id);
    Task<T?> Add(T entity);
}