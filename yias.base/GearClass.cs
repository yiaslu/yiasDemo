using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace yias
{
    public class GearClass
    {
        #region 构造方法
        Dictionary<string, string> strValue = new Dictionary<string, string>();
        public string this[string key]
        {
            get
            {
                if (!strValue.ContainsKey(key))
                {
                    strValue.Add(key, "");
                }
                return strValue[key];
            }
            set
            {
                bool ishave = false;
                foreach (var item in strValue.Keys)
                {
                    if (item == key)
                    {
                        ishave = true;
                        strValue[key] = value;
                        break;
                    }
                }
                if (!ishave)
                {
                    strValue.Add(key, value);
                }
            }
        }
        public GearClass(string json)
        {
            try
            {
                JObject jsonmodel = (JObject)JsonConvert.DeserializeObject(json);
                foreach (var jpropInfo in jsonmodel)
                {
                    strValue.Add(jpropInfo.Key + "", (jpropInfo.Value + "").Replace("[+|+]", "'"));
                }
            }
            catch (Exception)
            {

                throw new Exception("调用程序错误，无法返回请求");
            }
        }
        public GearClass(Dictionary<string, string> dx)
        {
            strValue = dx;
        }
        #endregion


        public void Add(string key, object obj)
        {
            if (obj != null)
                if (obj.GetType().BaseType.Name == "BaseMODEL")
                {
                    strValue.Add(key, JsonConvert.SerializeObject(obj));
                }
                else
                    strValue.Add(key, obj.ToString());
        }
        public GearClass(string TypeName, string Method, string BusinessName)
        {
            this.BusinessName = BusinessName;
            this.TypeName = TypeName;
            this.Method = Method;
            this.Types = "Method";
        }
        public GearClass(string TypeName, string Method, string BusinessName, string ConfigFileName)
        {
            this.BusinessName = BusinessName;
            this.TypeName = TypeName;
            this.Method = Method;
            this.Types = "Method";
        }
        private string BusinessName
        {
            get { return this["BusinessName"]; }
            set { this["BusinessName"] = value; }
        }

        private string TypeName
        {
            get { return this["typeName"]; }
            set { this["typeName"] = value; }
        }

        private string Method
        {
            get { return this["Method"]; }
            set { this["Method"] = value; }
        }
        private string Types
        {
            get { return this["types"]; }
            set { this["types"] = value; }
        }
        public object Send()
        {
            string dllStr = (AppDomain.CurrentDomain.BaseDirectory + "\\" + this["BusinessName"] + ".dll").Replace("\\\\", "\\");
            Assembly assembly = Assembly.LoadFrom(dllStr);
            Type type = assembly.GetType(this["typeName"]);
            object obj = assembly.CreateInstance(type.FullName, true);
            if (this["types"] == "Method")
            {
                MethodInfo mt = obj.GetType().GetMethod(this["Method"]);
                int length = mt.GetParameters().Length;
                ParameterInfo[] paramsInfo = mt.GetParameters();
                object[] objs = new object[length];
                for (int i = 0; i < length; i++)
                {
                    Type tType = paramsInfo[i].ParameterType;
                    //如果它是值类型,或者String   
                    if (tType.Equals(typeof(string)) || (!tType.IsInterface && !tType.IsClass))
                    {
                        //改变参数类型   
                        objs[i] = Convert.ChangeType(this[paramsInfo[i].Name], tType);
                    }
                    else if ((tType.BaseType.Name == "BaseMODEL" || (tType.BaseType.BaseType != null && tType.BaseType.BaseType.Name == "BaseMODEL")) && !string.IsNullOrEmpty(this[paramsInfo[i].Name]))
                    {
                        JObject jsonmodel = (JObject)JsonConvert.DeserializeObject(this[paramsInfo[i].Name]);

                        object info = tType.Assembly.CreateInstance(tType.FullName, true);
                        foreach (PropertyInfo propInfo in tType.GetProperties())
                        {
                            foreach (var jpropInfo in jsonmodel)
                            {
                                if (propInfo.Name == jpropInfo.Key)
                                {
                                    if (!string.IsNullOrEmpty(jpropInfo.Value + ""))
                                    {
                                        //if (propInfo.PropertyType.Name == "List`1")
                                        //{
                                        //    propInfo.SetValue(info, Convert.ChangeType(getListVal(propInfo.PropertyType, jpropInfo.Value + "", modela), propInfo.PropertyType), null);
                                        //}
                                        //else
                                        propInfo.SetValue(info, GetMainValue(propInfo, jpropInfo.Value), null);
                                    }
                                    break;
                                }
                            }
                        }
                        objs[i] = Convert.ChangeType(info, tType);
                    }
                    //else if (tType.Name == "List`1")
                    //{
                    //    objs[i] = Convert.ChangeType(getListVal(tType, this[paramsInfo[i].Name], modela), tType);
                    //}
                }
                try
                {
                    object ob = mt.Invoke(obj, objs);
                    return ob;
                }
                catch (TargetInvocationException exp)
                {
                    return new { type = "error", msg = exp.InnerException.Message };
                }
            }
            return obj;
        }

        private object getListVal(Type tType, string value, Assembly modela)
        {
            object ojblist = Activator.CreateInstance(tType);
            string strItem = tType.FullName.Replace("System.Collections.Generic.List`1[[", "");
            JArray arr = JArray.Parse(value);
            Type modType = modela.GetType((strItem.Substring(0, strItem.IndexOf(','))));
            foreach (JToken item in arr)
            {
                JObject jsonmodel = item.Value<JObject>();

                object itemobj = modType.Assembly.CreateInstance(modType.FullName, true);

                foreach (PropertyInfo propInfo in modType.GetProperties())
                {
                    foreach (var jpropInfo in jsonmodel)
                    {
                        if (propInfo.Name == jpropInfo.Key)
                        {
                            if (!string.IsNullOrEmpty(jpropInfo.Value + ""))
                            {
                                if (propInfo.PropertyType.Name == "List`1")
                                {
                                    propInfo.SetValue(itemobj, Convert.ChangeType(getListVal(propInfo.PropertyType, jpropInfo.Value + "", modela), propInfo.PropertyType), null);
                                }
                                else
                                    propInfo.SetValue(itemobj, GetMainValue(propInfo, jpropInfo.Value), null);
                            }
                            break;
                        }
                    }
                }
                ojblist.GetType().GetMethod("Add").Invoke(ojblist, new Object[] { itemobj });
            }
            return ojblist;
        }

        private object GetMainValue(PropertyInfo proper, object value)
        {
            string strRowType = proper.PropertyType.FullName.ToLower();
            object retValue = null;
            try
            {
                if (strRowType.IndexOf("int") != -1)
                {
                    retValue = int.Parse(value + "");
                }
                else if (strRowType.IndexOf("decimal") != -1)
                {
                    retValue = decimal.Parse(value + "");
                }
                else if (strRowType.IndexOf("bool") != -1)
                {
                    retValue = bool.Parse(value + "");
                }
                else if (strRowType.IndexOf("datetime") != -1)
                {
                    retValue = DateTime.Parse(value + "");
                }
                else
                    retValue = value + "";
            }
            catch
            {
            }
            return retValue;
        }
    }
}
