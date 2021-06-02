using System;
using System.Collections.Generic;
using System.Text;

namespace yias.service
{
    /// <summary>
    /// 自定义模型
    /// </summary>
    public class objmodel
    {
        public objmodel() { }
        Dictionary<string, object> strValue = new Dictionary<string, object>();
        public object this[string key]
        {
            get
            {
                if (strValue.ContainsKey(key))
                    return strValue[key];
                return null;
            }
            set
            {
                if (!strValue.ContainsKey(key))
                {
                    strValue.Add(key, value);
                }
                else
                {
                    strValue[key] = value;
                }
            }
        }
        public modelseting modelset
        {
            get { return (modelseting)this["objmodel_modelseting"]; }
            set { this["objmodel_modelseting"] = value; }
        }
    }

    /// <summary>
    /// 模型属性
    /// </summary>
    public class modelseting : objmodel
    {
        public string objname
        {
            get { return this["modelseting_objname"].ToString(); }
            set { this["modelseting_objname"] = value; }
        }
        public List<modelattr> modelattrs
        {
            get { return (List<modelattr>)this["modelseting_modelattrs"]; }
            set { this["modelseting_modelattrs"] = value; }
        }
    }

    /// <summary>
    /// 模型属性 属性
    /// </summary>
    public class modelattr : objmodel
    {
        public modelattr()
        {
            this.isprimary = false;
            this.isidentity = false;
        }
        public bool? isprimary
        {
            get { return (bool?)this["modelattr_isprimary"]; }
            set { this["modelattr_isprimary"] = value; }
        }
        public bool? isvirtual
        {
            get { return (bool?)this["modelattr_isvirtual"]; }
            set { this["modelattr_isvirtual"] = value; }
        }
        public bool? isidentity
        {
            get { return (bool?)this["modelattr_isidentity"]; }
            set { this["modelattr_isidentity"] = value; }
        }
        public string dbtype
        {
            get { return this["modelattr_dbtype"].ToString(); }
            set { this["modelattr_dbtype"] = value; }
        }
    }

    /// <summary>
    /// 条件模型
    /// </summary>
    public class wheremodel : objmodel
    {
        /// <summary>
        /// 类型 K括号  W条件
        /// </summary>
        public string wtype
        {
            get { return this["wheremodel_wtype"] + ""; }
            set { this["wheremodel_wtype"] = value; }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public string key
        {
            get { return this["wheremodel_key"] + ""; }
            set { this["wheremodel_key"] = value; }
        }
        /// <summary>
        /// 条件  and 并且 or 或者 
        /// </summary>
        public string condition
        {
            get { return this["wheremodel_condition"] + ""; }
            set { this["wheremodel_condition"] = value; }
        }
        /// <summary>
        /// 符号 >,<,=,<>,!=,>=,<=,like,in 
        /// </summary>
        public string symbol
        {
            get { return this["wheremodel_symbol"] + ""; }
            set { this["wheremodel_symbol"] = value; }
        }
        /// <summary>
        /// 符合值
        /// </summary>
        public string values
        {
            get { return this["wheremodel_values"] + ""; }
            set { this["wheremodel_values"] = value; }
        }
    }
}
