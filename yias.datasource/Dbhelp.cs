using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace yias.datasource
{
    public enum DBtype
    {
        Oracle,
        SQL,
        MySql,
        File,
        MonoDB
    }
    public class Dbhelp : idataex
    {
        public static Dbhelp Init(string sqlcon, DBtype dp = DBtype.Oracle)
        {
            return new Dbhelp(sqlcon, dp);
        }
        DbCommand m_cmd;
        DbConnection m_conn;
        DbTransaction m_trans;
        string dbconnstr;
        idataex dataex;
        #region 工厂
        Dbhelp(string sqlcon, DBtype dp)
        {
            this.dbconnstr = sqlcon;
            switch (dp)
            {
                case DBtype.Oracle:
                    SetOracle();
                    break;
                case DBtype.SQL:
                    SetSqlService();
                    break;
                case DBtype.MySql:
                    SetMySql();
                    break;
                case DBtype.File:
                    SetFile();
                    break;
                case DBtype.MonoDB:
                    SetMonoDB();
                    break;
                default:
                    throw new Exception("数据库类型不支持！");
            }
        }
        private void SetOracle()
        {
            dataex = new data_oracle(this.dbconnstr);
        }
        private void SetSqlService()
        {

        }
        private void SetMySql()
        {

        }
        private void SetFile()
        {

        }
        private void SetMonoDB()
        {

        }
        #endregion
        #region 模型存储

        public void Open()
        {
            dataex.Open();
        }

        public void Close()
        {
            dataex.Close();
        }

        public void BeginTransaction()
        {
            dataex.BeginTransaction();
        }

        public void Commit()
        {
            dataex.Commit();
        }

        public void Rollback()
        {
            dataex.Rollback();
        }
        #endregion
    }
}
