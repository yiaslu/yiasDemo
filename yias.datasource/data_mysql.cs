using System;
using System.Collections.Generic;
using System.Text;

namespace yias.datasource
{
    public class data_mysql : idataex
    {

        public void BeginTransaction()
        {
        }

        public void Close()
        {
        }

        public void Commit()
        {
        }
        public void Open()
        {
        }

        public void Rollback()
        {
        }

        public bool Add(objmodel obj)
        {
            return false;
        }
        public bool Updata(objmodel obj)
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


    }
}
