using System;
using System.Collections.Generic;
using System.Text;

namespace yias
{
    public interface idataex
    {
        void Open();
        void Close();
        void BeginTransaction();
        void Commit();
        void Rollback();
        bool Add(objmodel obj);
        bool Updata(objmodel obj);
        bool Delete(objmodel obj);
        List<objmodel> GetObjmodels(DbhelpNeed need, objmodel obj);
    }
}
