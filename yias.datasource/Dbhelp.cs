using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using yias.datasource;

namespace yias
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBtype
    {
        Oracle,
        SQL,
        MySql,
        File,
        MonoDB
    }
    /// <summary>
    /// 条件符号
    /// </summary>
    public enum NeedTp
    {
        /// <summary>
        /// =
        /// </summary>
        EQ,
        /// <summary>
        /// >
        /// </summary>
        GT,
        /// <summary>
        /// <
        /// </summary>
        LT,
        /// <summary>
        /// >=
        /// </summary>
        GTEQ,
        /// <summary>
        /// <=
        /// </summary>
        LTEQ,
        /// <summary>
        /// <>/!=
        /// </summary>
        NEQ,
        /// <summary>
        /// in
        /// </summary>
        IN,
        /// <summary>
        /// like
        /// </summary>
        LIKE,
        /// <summary>
        /// 自定义条件
        /// </summary>
        NONE
    }
    public class Dbhelp : idataex
    {
        public static Dbhelp Init(string sqlcon, DBtype dp = DBtype.Oracle)
        {
            return new Dbhelp(sqlcon, dp);
        }
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
            dataex = new data_file(this.dbconnstr);
        }
        private void SetMonoDB()
        {

        }
        #endregion
        #region 模型存储
        /// <summary>
        /// 开启连接
        /// </summary>
        public void Open()
        {
            dataex.Open();
        }
        /// <summary>
        /// 关闭事务
        /// </summary>
        public void Close()
        {
            dataex.Close();
        }
        /// <summary>
        /// 开启事务
        /// </summary>
        public void BeginTransaction()
        {
            dataex.BeginTransaction();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            dataex.Commit();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            dataex.Rollback();
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="obj"></param>
        public bool Add(objmodel obj)
        {
            return dataex.Add(obj);
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="obj"></param>
        public bool Updata(objmodel obj)
        {
            return dataex.Updata(obj);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="obj"></param>
        public bool Delete(objmodel obj)
        {
            return dataex.Delete(obj);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="need"></param>
        /// <returns></returns>
        public List<objmodel> GetObjmodels(DbhelpNeed need, objmodel obj)
        {
            return dataex.GetObjmodels(need, obj);
        }
        #endregion
    }


    /// <summary>
    /// 需要条件
    /// </summary>
    public class DbhelpNeed : objmodel
    {
        private List<wheremodel> _wheres
        {
            get { return (List<wheremodel>)this["DbhelpNeed_wheres"]; }
            set { this["DbhelpNeed_wheres"] = value; }
        }
        public int pagesize
        {
            get { return (int)this["DbhelpNeed_pagesize"]; }
            set { this["DbhelpNeed_pagesize"] = value; }
        }
        public int pageindex
        {
            get { return (int)this["DbhelpNeed_pagesize"]; }
            set { this["DbhelpNeed_pagesize"] = value; }
        }
        public int pagetotal
        {
            get { return (int)this["DbhelpNeed_pagetotal"]; }
            set { this["DbhelpNeed_pagetotal"] = value; }
        }
        public int rowtotal
        {
            get { return (int)this["DbhelpNeed_rowtotal"]; }
            set { this["DbhelpNeed_rowtotal"] = value; }
        }

        private void Add(string key, NeedTp need, object value, string tp)
        {
            _wheres.Add(new wheremodel() { key = key, condition = tp, need = need, values = value, wtype = "W" });
        }

        /// <summary>
        /// 获取条件集合
        /// </summary>
        /// <returns></returns>
        public List<wheremodel> GetWheres()
        {
            return _wheres;
        }

        public DbhelpNeed(int pagesize = 0, int pageindex = 1)
        {
            this.pagesize = pagesize;
            this.pageindex = pageindex;
        }

        /// <summary>
        ///  并且条件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="need"></param>
        /// <param name="value"></param>
        public void And(string key, NeedTp need, object value)
        {
            Add(key, need, value, "AND");
        }

        /// <summary>
        /// 或者条件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="need"></param>
        /// <param name="value"></param>
        public void OR(string key, NeedTp need, object value)
        {
            Add(key, need, value, "OR");
        }

        /// <summary>
        /// 左括号
        /// </summary>
        public void LeftBra(string tp = "and")
        {
            _wheres.Add(new wheremodel() { condition = tp, wtype = "K", key = "(" });
        }

        /// <summary>
        /// 右括号
        /// </summary>
        public void RightBra()
        {
            _wheres.Add(new wheremodel() { wtype = "K", key = ")" });
        }
    }
}
