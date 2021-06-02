using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace yias.datasource
{
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
