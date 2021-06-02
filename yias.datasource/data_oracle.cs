using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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


        public DbCommand m_cmd;
        public DbConnection m_conn;
        public DbTransaction m_trans;
        public string dbconnstr;
        private idataex dataex;
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

    public interface idataex
    {
        void Open();
        void Close();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }

    public class data_oracle : idataex
    {
        private OracleCommand m_cmd;
        private OracleConnection m_conn;
        private OracleTransaction m_trans;
        private string dbconnstr;
        public data_oracle(string dbconnstr)
        {
            this.dbconnstr = dbconnstr;
            this.m_conn = new OracleConnection(dbconnstr);
        }
        public void Open()
        {
            this.m_conn.Open();
        }
        public void Close()
        {
            this.m_conn.Close();
        }
        public void BeginTransaction()
        {
            if (m_trans == null)
            {
                m_trans = m_conn.BeginTransaction();
                m_cmd.Transaction = m_trans;
            }
        }
        public void Commit()
        {
            if (m_trans != null)
            {
                m_trans.Commit();
                m_trans = null;
            }
        }
        public void Rollback()
        {
            if (m_trans != null)
            {
                m_trans.Rollback();
                m_trans = null;
            }
        }
        public object ExecuteScalar(string sqlstr, params DbParameter[] paramsList)
        {

            object returnObj = null;
            m_cmd.Parameters.Clear();
            m_cmd.CommandText = sqlstr;
            m_cmd.CommandType = CommandType.Text;
            if (paramsList != null)
            {
                m_cmd.BindByName = true;
                foreach (OracleParameter param in paramsList)
                {
                    m_cmd.Parameters.Add(param);
                }
            }
            ConnectionState connState = m_conn.State;
            if (connState == ConnectionState.Closed)
                m_conn.Open();
            try
            {
                returnObj = m_cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connState == ConnectionState.Closed)
                    m_conn.Close();
            }
            return returnObj;
        }
        public int ExecuteNonQuery(string sqlstr, params DbParameter[] paramsList)
        {
            int returnInt = 0;
            m_cmd.Parameters.Clear();
            m_cmd.CommandText = sqlstr;
            if (paramsList != null)
            {
                m_cmd.BindByName = true;
                foreach (OracleParameter param in paramsList)
                {
                    m_cmd.Parameters.Add(param);
                }
            }
            ConnectionState connState = m_conn.State;
            if (connState == ConnectionState.Closed)
                m_conn.Open();
            try
            {
                returnInt = m_cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connState == ConnectionState.Closed)
                    m_conn.Close();
            }
            return returnInt;
        }
        public DataTable FillTable(string sqlstr)
        {
            OracleDataAdapter m_da = new OracleDataAdapter(sqlstr, m_conn);
            m_da.SelectCommand.Transaction = m_trans;
            var m_table = new DataTable();
            m_da.Fill(m_table);
            return m_table;
        }
    }
}
