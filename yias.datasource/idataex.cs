using System;
using System.Collections.Generic;
using System.Text;

namespace yias.datasource
{
    public interface idataex
    {
        void Open();
        void Close();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
