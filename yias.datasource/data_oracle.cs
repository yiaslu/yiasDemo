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
        private object ExecuteScalar(string sqlstr, params OracleParameter[] paramsList)
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
        private int ExecuteNonQuery(string sqlstr, params OracleParameter[] paramsList)
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
        private DataTable FillTable(string sqlstr, params OracleParameter[] paramsList)
        {
            OracleDataAdapter m_da = new OracleDataAdapter(sqlstr, m_conn);
            m_da.SelectCommand.Transaction = m_trans;
            if (paramsList != null && paramsList.Length > 0)
                m_da.SelectCommand.Parameters.AddRange(paramsList);
            var m_table = new DataTable();
            m_da.Fill(m_table);
            return m_table;
        }

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

        public bool Add(objmodel obj)
        {
            if (!string.IsNullOrEmpty(obj.modelset.dbname))
            {
                var inertstr = "INSERT INTO {tabname} ({keys}) VALUES({values})";
                var tableName = obj.modelset.dbname;
                var keyList = new List<string>();
                var valList = new List<string>();
                List<OracleParameter> plist = new List<OracleParameter>();
                foreach (var item in obj.modelset.modelattrs)
                {
                    if (obj[item.keyname] != null)
                    {
                        keyList.Add(item.keyname);
                        if (!item.isprimary.Value && !item.isidentity.Value)
                        {
                            valList.Add(":" + item.keyname);
                            plist.Add(new OracleParameter() { ParameterName = ":" + item.keyname, DbType = DbType.String, Value = obj[item.keyname] });
                        }
                        else
                        {
                            valList.Add("seq_" + tableName + ".nextval");
                        }
                    }
                }
                if (keyList.Count != 0)
                {
                    var insert = inertstr.Replace("{tabname}", tableName).Replace("{keys}", string.Join(",", keyList)).Replace("{values}", string.Join(",", valList));
                    var ret = ExecuteNonQuery(insert, plist.ToArray());
                    return ret > 0;
                }
                else { throw new Exception("未获取到需要添加的数据！ "); }
            }
            return false;
        }

        public bool Updata(objmodel obj)
        {
            if (!string.IsNullOrEmpty(obj.modelset.dbname))
            {
                var updatestr = "UPDATE {tabname} SET {setval} WHERE {keys}";
                var tableName = obj.modelset.dbname;
                var keysList = new List<string>();
                var setvalList = new List<string>();
                List<OracleParameter> plist = new List<OracleParameter>();
                foreach (var item in obj.modelset.modelattrs)
                {
                    if (obj[item.keyname] != null)
                    {
                        if (!item.isprimary.Value)
                        {
                            setvalList.Add(item.keyname + "=:" + item.keyname);
                        }
                        else
                        {
                            keysList.Add(item.keyname + "=:" + item.keyname);
                        }
                        plist.Add(new OracleParameter() { ParameterName = ":" + item.keyname, DbType = DbType.String, Value = obj[item.keyname] });
                    }
                }
                if (keysList.Count != 0)
                {
                    var updat = updatestr.Replace("{tabname}", tableName).Replace("{setval}", string.Join(",", setvalList)).Replace("{keys}", string.Join(" and ", keysList));
                    var ret = ExecuteNonQuery(updat, plist.ToArray());
                    return ret > 0;
                }
                else { throw new Exception("未获取到需要添加的数据！ "); }
            }
            return false;
        }

        public bool Delete(objmodel obj)
        {
            if (!string.IsNullOrEmpty(obj.modelset.dbname))
            {
                var deletestr = "DELETE {tabname} WHERE {keys}";
                var tableName = obj.modelset.dbname;
                var keysList = new List<string>();
                List<OracleParameter> plist = new List<OracleParameter>();
                foreach (var item in obj.modelset.modelattrs)
                {
                    if (obj[item.keyname] != null)
                    {
                        if (item.isprimary.Value)
                        {
                            keysList.Add(item.keyname + "=:" + item.keyname);
                        }
                        plist.Add(new OracleParameter() { ParameterName = ":" + item.keyname, DbType = DbType.String, Value = obj[item.keyname] });
                    }
                }
                if (keysList.Count != 0)
                {
                    var del = deletestr.Replace("{tabname}", tableName).Replace("{keys}", string.Join(" and ", keysList));
                    var ret = ExecuteNonQuery(del, plist.ToArray());
                    return ret > 0;
                }
                else { throw new Exception("未获取到需要添加的数据！ "); }
            }
            return false;
        }

        public List<objmodel> GetObjmodels(DbhelpNeed need, objmodel obj)
        {
            if (!string.IsNullOrEmpty(obj.modelset.dbname))
            {
                var selectstr = "SELECT * FROM {tabname} WHERE 1=1 {where}";
                var tableName = obj.modelset.dbname;
                var wheres = "";
                List<OracleParameter> plist = new List<OracleParameter>();
                foreach (var item in need.GetWheres())
                {
                    wheres += " " + item.condition + " ";
                    if (item.wtype == "K")
                    {
                        wheres += item.key;
                    }
                    else
                    {
                        bool isAddVal = true;
                        switch (item.need)
                        {
                            case NeedTp.EQ:
                                wheres += item.key + "=" + ":" + item.key;
                                break;
                            case NeedTp.GT:
                                wheres += item.key + ">" + ":" + item.key;
                                break;
                            case NeedTp.LT:
                                wheres += item.key + "<" + ":" + item.key;
                                break;
                            case NeedTp.GTEQ:
                                wheres += item.key + ">=" + ":" + item.key;
                                break;
                            case NeedTp.LTEQ:
                                wheres += item.key + "<=" + ":" + item.key;
                                break;
                            case NeedTp.NEQ:
                                wheres += item.key + "<>" + ":" + item.key;
                                break;
                            case NeedTp.IN:
                                isAddVal = false;
                                wheres += item.key + " in " + item.values;
                                break;
                            case NeedTp.LIKE:
                                isAddVal = false;
                                wheres += item.key + " like " + ":" + item.key;
                                plist.Add(new OracleParameter() { ParameterName = ":" + item.key, DbType = DbType.String, Value = "%" + item.values + "%" });
                                break;
                            case NeedTp.NONE:
                                isAddVal = false;
                                wheres += item.key + item.values;
                                break;
                            default:
                                break;
                        }
                        if (isAddVal)
                            plist.Add(new OracleParameter() { ParameterName = ":" + item.key, DbType = DbType.String, Value = item.values });
                    }
                }
                var select = selectstr.Replace("{tabname}", tableName).Replace("{where}", wheres);

                if (need.pagesize != 0)
                {
                    var rowcount = 0;
                    select = GetPageSql(select, need.pagesize, need.pageindex, plist.ToArray(), out rowcount);
                    need.rowtotal = rowcount;
                    need.pagetotal = need.rowtotal / need.pagesize + 1;
                }

                var seldt = FillTable(select, plist.ToArray());
                List<objmodel> retList = new List<objmodel>();
                if (seldt != null && seldt.Rows.Count != 0)
                {
                    foreach (DataRow row in seldt.Rows)
                    {
                        var retobj = new objmodel();
                        foreach (var item in obj.modelset.modelattrs)
                        {
                            if (row[item.keyname] != null)
                                retobj[item.keyname] = row[item.keyname];
                        }
                        retList.Add(retobj);
                    }
                }
                return retList;

            }
            return null;
        }
        
        /// <summary>
        /// 获取分页sql
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="pagesize"></param>
        /// <param name="pageindex"></param>
        /// <param name="paramsList"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private string GetPageSql(string strSql, int pagesize, int pageindex, OracleParameter[] paramsList, out int count)
        {
            count = (int)ExecuteScalar("SELECT COUNT(*) FROM (" + strSql + ")SELECTTABLES", paramsList);

            string retSql = "SELECT * FROM (SELECT ROWNUM ROWINDEX,SELECTTABLES.* FROM (" + strSql + ")SELECTTABLES)WHERE ROWINDEX BETWEEN " + ((pageindex - 1) * pagesize) + " AND " + (pageindex * pagesize);

            return retSql;
        }
    }
}
