using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace yias
{
    public class GearClassObj
    {
        #region 构造方法
        Dictionary<string, object> strValue = new Dictionary<string, object>();
        public GearClassObj(string json)
        {
            try
            {
                strValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            }
            catch (Exception)
            {

                throw new Exception("调用程序错误，无法解析请求");
            }
        }
        public object this[string key]
        {
            get
            {
                try
                {
                    if (strValue[key] == null)
                    {

                    }
                }
                catch (Exception)
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

        /// <summary>
        ///类库名称
        /// </summary>
        private string BusinessName
        {
            get { return this["BusinessName"].ToString(); }
            set { this["BusinessName"] = value; }
        }

        /// <summary>
        /// 类库
        /// </summary>
        private string TypeName
        {
            get { return this["typeName"].ToString(); }
            set { this["typeName"] = value; }
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        private string Method
        {
            get { return this["Method"].ToString(); }
            set { this["Method"] = value; }
        }
        #endregion


        public object Send()
        {
            string dllStr = (AppDomain.CurrentDomain.BaseDirectory + "\\" + this["BusinessName"] + ".dll").Replace("\\\\", "\\");
            Assembly assembly = Assembly.LoadFrom(dllStr);
            Type type = assembly.GetType(TypeName);
            object obj = assembly.CreateInstance(type.FullName, true);
            MethodInfo mt = obj.GetType().GetMethod(Method);

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
                else if (tType.Name == "Dictionary`2")
                {
                    objs[i] = Convert.ChangeType(JsonConvert.DeserializeObject<Dictionary<string, object>>(this[paramsInfo[i].Name].ToString()), tType);
                }
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
    }
}
