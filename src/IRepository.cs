using System.Collections.Generic;

namespace hwwebapi.Core {

    public interface IRepository<TId, TEntity> {

        IEnumerable<TEntity> GetAll();
        bool TryGet(TId id, out TEntity value);
        TId Create(TEntity value);
        bool TryUpdate(TId id, TEntity value);
        bool Delete(TId id);

    }

}
