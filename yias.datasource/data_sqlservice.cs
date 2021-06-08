using System;
using System.Collections.Generic;
using System.Text;

namespace yias.datasource
{
    public class data_sqlservice : idataex
    {
        public void Open()
        {
        }

        public void Rollback()
        {
        }

        public void BeginTransaction()
        {
        }

        public void Close()
        {
        }

        public void Commit()
        {
        }

        public bool Add(objmodel obj)
        {
            return false;
        }

        public bool Delete(objmodel obj)
        {
            return false;
        }

        public List<objmodel> GetObjmodels(DbhelpNeed need, objmodel obj)
        {
            throw new NotImplementedException();
        }

        public bool Updata(objmodel obj)
        {
            return false;
        }
    }
}
