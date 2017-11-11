using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atom.Framework.Utility
{
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class TypeHelper
    {
        /// <summary>
        /// 判断某类型是否某类型或者是否继承自某类型，支持泛型定义类型之间的比较
        /// </summary>
        /// <param name="valiType">要判断的类型</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static bool IsOrInheritFrom(this Type valiType, Type targetType)
        {
            if (valiType.GUID == targetType.GUID)
                return true;
            if (valiType.IsGenericTypeDefinition && targetType.IsGenericTypeDefinition)
            {
                if (targetType.IsInterface)
                {
                    Type t = valiType.GetInterface(targetType.FullName);
                    if (t == null)
                        return false;
                    return t.GUID == targetType.GUID;
                }
                else
                {
                    Type tempBaseType = valiType;
                    while (tempBaseType != null)
                    {
                        if (tempBaseType.GUID == targetType.GUID)
                            return true;
                        tempBaseType = tempBaseType.BaseType;
                    }
                    return false;
                }
            }
            else if (!valiType.IsGenericTypeDefinition && targetType.IsGenericTypeDefinition)
                return false;
            else
            {
                return targetType.IsAssignableFrom(valiType);
            }
        }

        /// <summary>
        /// 判断某数据是否某类型或者是否继承自某类型，支持泛型定义类型之间的比较
        /// </summary>
        /// <param name="valiType">要判断的类型</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static bool IsOrInheritFrom(this object obj, Type targetType)
        {
            return IsOrInheritFrom(obj.GetType(), targetType);
        }

        /// <summary>
        /// 判断某类型是否与某类型关联，支持泛型及泛型定义处理
        /// </summary>
        /// <param name="valiType">要判断的类型</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static bool IsRelevanceFrom(this Type valiType, Type targetType)
        {
            if (valiType.GUID == targetType.GUID)
                return true;
            if (valiType.IsGenericTypeDefinition && targetType.IsGenericTypeDefinition)
            {
                return IsOrInheritFrom(valiType, targetType);
            }
            else if ((!valiType.IsGenericTypeDefinition) && (!targetType.IsGenericTypeDefinition))
            {
                return IsOrInheritFrom(valiType, targetType);
            }
            else if ((valiType.IsGenericTypeDefinition) && (!targetType.IsGenericTypeDefinition))
            {
                return IsOrInheritFrom(valiType, targetType);
            }
            //valiType不是泛型定义，targetType是泛型定义
            else
            {
                if (targetType.IsInterface)
                {
                    foreach (Type intType in valiType.GetInterfaces(true))
                    {
                        if (intType.IsGenericType)
                        {
                            if (IsOrInheritFrom(intType.GetGenericTypeDefinition(), targetType)) return true;
                        }
                    }
                    return false;
                }
                else if (valiType.IsInterface)
                {
                    return false;
                }
                //valiType是非接口非泛型定义类型，targetType是泛型定义
                else
                {
                    Type tempBaseType = valiType;
                    while (tempBaseType != null)
                    {
                        if (tempBaseType.IsGenericType)
                        {
                            if (IsOrInheritFrom(tempBaseType.GetGenericTypeDefinition(), targetType))
                            {
                                return true;
                            }
                        }
                        tempBaseType = tempBaseType.BaseType;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// 根据泛型定义获取指定类型继承链中的直属泛型类型
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="genericDefineType">泛型定义类型</param>
        /// <returns>如果不存在则返回null</returns>
        public static Type GetGenericTypeFromDefine(this Type type, Type genericDefineType)
        {
            if (!genericDefineType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("genericDefineType并非泛型定义类型");
            }
            else
            {
                if (genericDefineType.IsInterface)
                {
                    foreach (Type intType in type.GetInterfaces(true))
                    {
                        if (intType.IsGenericType)
                        {
                            if (intType.GetGenericTypeDefinition().GUID == genericDefineType.GUID)
                            {
                                return intType;
                            }
                        }
                    }
                    return null;
                }
                else if (type.IsInterface)
                {
                    return null;
                }
                else
                {
                    Type tempBaseType = type;
                    while (tempBaseType != null)
                    {
                        if (tempBaseType.IsGenericType)
                        {
                            if (tempBaseType.GetGenericTypeDefinition().GUID == genericDefineType.GUID)
                            {
                                return tempBaseType;
                            }
                        }
                        tempBaseType = tempBaseType.BaseType;
                    }
                    return null;
                }
            }
        }


        /// <summary>
        /// 获取指令类型实现的所有接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="includeSelft">如果自身是接口是否应包含</param>
        /// <returns>所有接口</returns>
        public static Type[] GetInterfaces(this Type type, bool includeSelft)
        {
            Type[] result = type.GetInterfaces();
            if (type.IsInterface)
            {
                Type[] temp = new Type[result.Length + 1];
                temp[0] = type;
                result.CopyTo(temp, 1);
                result = temp;
            }
            return result;
        }



        /// <summary>
        /// 判断某数据是否与某类型关联，支持泛型及泛型定义处理
        /// </summary>
        /// <param name="obj">要判断的数据</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static bool IsRelevanceFrom(this object obj, Type targetType)
        {
            return IsRelevanceFrom(obj.GetType(), targetType);
        }

        /// <summary>
        /// 获取类型继承自泛型定义类型下的泛型类型时所给定的泛型实参类型数组
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="genericDefineType">泛型定义类型</param>
        /// <returns>泛型实参类型数组</returns>
        public static Type[] GetGenericArguments(this Type type, Type genericDefineType)
        {
            Type genType = GetGenericTypeFromDefine(type, genericDefineType);
            if (genType == null)
            {
                throw new TargetInvocationException("指定的类型并非继承自指定的泛型定义类型下的泛型类型", null);
            }
            return genType.GetGenericArguments();
        }

        /// <summary>
        /// 获取数据类型的去除版本的反射字符串，可通过 Type.GetType反射出该类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>可用于通过Type.GetType获取类型的字符串</returns>
        public static string GetTypeRelationNameWithoutVision(this Type type)
        {
            string assemblyFullName = type.Assembly.FullName;
            string typeFullName = type.FullName;
            //去除程序集全名中包含的版本号和签名信息
            assemblyFullName = Regex.Replace(assemblyFullName, @", Version([\.=a-zA-Z_0-9, ]|-)+", "");
            //去除类型全名中包含的版本号和签名信息，主要针对泛型
            typeFullName = Regex.Replace(typeFullName, @", Version([\.=a-zA-Z_0-9, ]|-)+]", "]");
            return typeFullName + "," + assemblyFullName;
        }


        private static readonly Dictionary<string, string> _specificAssimbleFullName = new List<KeyValuePair<string, string>>()
        {
         new KeyValuePair<string,string>("System",typeof(ObservableCollection<int>).Assembly.FullName)
        }.ToDictionary(m => m.Key, m => m.Value);

        /// <summary>
        /// 根据类型全名称获取对应类型
        /// </summary>
        /// <param name="typeName">类型全名称，可不包含版本</param>
        /// <param name="throwOnError">如果获取失败是否抛出异常，如果不抛出异常则返回null</param>
        /// <returns>类型</returns>
        public static Type GetType(string typeName, bool throwOnError = true)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            Type type = Type.GetType(typeName, false);
            if (type == null)
            {
                //解决特殊程序集无法通过不包含版本的全名称反射获取问题
                foreach (var item in _specificAssimbleFullName)
                {
                    typeName = typeName.Replace(string.Format(", {0}", item.Key), string.Format(", {0}", item.Value));
                }
                type = Type.GetType(typeName, throwOnError);
            }
            return type;
        }

        /// <summary>
        /// 创建某类型的无参构造实例
        /// </summary>
        /// <returns>实例</returns>
        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// 创建某类型的无参构造实例，如果是泛型定义则需要指定实参类型
        /// </summary>
        /// <param name="type">创建实例的类型</param>
        /// <param name="genericArgumentTypes">如果是泛型，则指定泛型的实参类型</param>
        /// <returns>实例</returns>
        public static object CreateInstance(this Type type, params Type[] genericArgumentTypes)
        {
            if (type.IsInterface) throw new InvalidOperationException("要创建的泛型类型不能为接口");
            if (type.IsAbstract) throw new InvalidOperationException("要创建的泛型类型不能为抽象类");
            Type objType = null;
            if (type.IsGenericTypeDefinition)
            {
                objType = GetGenericType(type, genericArgumentTypes);
            }
            else
            {
                objType = type;
            }
            return Activator.CreateInstance(objType);
        }

        /// <summary>
        /// 获取泛型定义类型的泛型类型
        /// </summary>
        /// <param name="genericDefineType">泛型定义类型</param>
        /// <param name="genericArgumentTypes">泛型的实参类型</param>
        /// <returns>泛型类型</returns>
        public static Type GetGenericType(this Type genericDefineType, params Type[] genericArgumentTypes)
        {
            if (!genericDefineType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(string.Format("指定的类型[{0}]并非泛型定义类型", genericDefineType));
            }
            Type[] argTypes = genericDefineType.GetGenericArguments();
            if (genericArgumentTypes == null || genericArgumentTypes.Length != argTypes.Length)
            {
                throw new InvalidOperationException("泛型定义类型的泛型参数类型个数与给定的个数不匹配");
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < argTypes.Length; i++)
            {
                foreach (Type consType in argTypes[0].GetGenericParameterConstraints())
                {
                    if (!consType.IsAssignableFrom(genericArgumentTypes[i]))
                    {
                        throw new InvalidOperationException(string.Format("第{0}个泛型参数类型[{1}]不满足参数类型约束[{2}]", i, genericArgumentTypes[i], argTypes[i]));
                    }
                }
                sb.AppendFormat("[{0}]", genericArgumentTypes[i].AssemblyQualifiedName);
                if (i < argTypes.Length - 1)
                    sb.Append(",");
            }
            return Type.GetType(
                 string.Format("{0}[{1}],{2}", genericDefineType.FullName, sb.ToString(), genericDefineType.Assembly.FullName));
        }


        /// <summary>
        /// 使用无参构造创建实例，无无参构造返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstanceByNoParaConstructor(this Type type)
        {
            ConstructorInfo info = GetNoParaConstructor(type);
            if (info == null)
                return null;
            else
                return info.Invoke(null);
        }

        /// <summary>
        /// 获取无参构造
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConstructorInfo GetNoParaConstructor(this Type type)
        {
            foreach (var item in type.GetConstructors())
            {
                if (!item.ContainsGenericParameters && item.GetParameters().Length == 0)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型的所有属性，和Type.GetProperties的区别在于如果是接口则同时包含检测接口的所有基接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 BindingFlags 组成</param>
        /// <param name="withInterfaceInheritanceChain">如果是接口，是否应包含接口继承链中的所有成员</param>
        /// <returns></returns>
        public static System.Reflection.PropertyInfo[] GetProperties(this Type type, BindingFlags bindingAttr, bool withInterfaceInheritanceChain)
        {
            if (type.IsInterface && withInterfaceInheritanceChain)
            {
                return
                    type.GetInterfaces()
                        .Union(new Type[] { type })
                        .SelectMany(m => m.GetProperties(bindingAttr))
                        .ToArray();
            }
            else
            {
                return type.GetProperties(bindingAttr);
            }
        }

        /// <summary>
        /// 获取指定类型的所有方法，和Type.GetMethods的区别在于如果是接口则同时包含检测接口的所有基接口
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 BindingFlags 组成</param>
        /// <param name="withInterfaceInheritanceChain">如果是接口，是否应包含接口继承链中的所有成员</param>
        /// <returns></returns>
        public static System.Reflection.MethodInfo[] GetMethods(this Type type, BindingFlags bindingAttr, bool withInterfaceInheritanceChain)
        {
            if (type.IsInterface && withInterfaceInheritanceChain)
            {
                return
                    type.GetInterfaces()
                        .Union(new Type[] { type })
                        .SelectMany(m => m.GetMethods(bindingAttr))
                        .ToArray();
            }
            else
            {
                return type.GetMethods(bindingAttr);
            }
        }

        /// <summary>
        /// 获取指定类型的所有字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 BindingFlags 组成</param>
        /// <param name="withInheritanceChain">是否应包含继承链中的所有字段</param>
        /// <returns>key为字段的层级全名称，如是基类中的字段aaa，则为aaa`1，如果是基类的基类中的字段bbb字段，则为bbb`2</returns>
        public static Dictionary<string, System.Reflection.FieldInfo> GetFields(this Type type, BindingFlags bindingAttr, bool withInheritanceChain)
        {
            if (withInheritanceChain)
            {
                Dictionary<string, FieldInfo> fList = new Dictionary<string, FieldInfo>();
                foreach (var item in type.GetFields(bindingAttr))
                {
                    fList.Add(item.Name, item);
                }
                Type[] chain = GetInheritanceChain(type, true);
                if (chain.Length > 1)
                {
                    for (int i = chain.Length - 2; i >= 0; i--)
                    {
                        Type ty = chain[i];
                        foreach (var item in ty.GetFields(bindingAttr))
                        {
                            fList.Add(string.Format("{0}`{1}", item.Name, i), item);
                        }
                    }
                }
                return fList;
            }
            else
            {
                return type.GetFields(bindingAttr).ToDictionary(m => m.Name);
            }
        }

        /// <summary>
        /// 获取指定类型的所有字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bindingAttr">一个位屏蔽，由一个或多个指定搜索执行方式的 BindingFlags 组成</param>
        /// <param name="withInheritanceChain">是否应包含继承链中的所有字段</param>
        /// <returns>字段对象，如果未找到则返回null</returns>
        public static System.Reflection.FieldInfo GetField(this Type type, string name, BindingFlags bindingAttr, bool withInheritanceChain)
        {
            if (withInheritanceChain)
            {
                int levelIndex = -1;
                int dotIndex = name.IndexOf('`');
                string realName = name;
                if (dotIndex > -1)
                {
                    realName = name.Remove(dotIndex);
                    levelIndex = Convert.ToInt32(name.Substring(dotIndex + 1));
                    if (levelIndex < 0)
                        levelIndex = -1;
                }
                if (levelIndex == -1)
                {
                    FieldInfo result = type.GetField(realName, bindingAttr);
                    if (result != null)
                        return result;
                }
                Type[] chain = GetInheritanceChain(type, true);
                if (levelIndex > -1 && levelIndex + 1 <= chain.Length)
                {
                    FieldInfo result = chain[levelIndex].GetField(realName, bindingAttr);
                    if (result != null)
                        return result;
                }
                for (int i = chain.Length - 1; i >= 0; i--)
                {
                    FieldInfo result = chain[i].GetField(realName, bindingAttr);
                    if (result != null)
                        return result;
                }
                return null;
            }
            else
            {
                return type.GetField(name, bindingAttr);
            }
        }

        /// <summary>
        /// 获取某指定类型的继承链
        /// </summary>
        /// <param name="type">要遍历继承链的类型</param>
        /// <param name="doAction">action</param>
        /// <param name="ascOrder">是否应从从顶端开始，否则从末端开始</param>
        public static Type[] GetInheritanceChain(this Type type, bool ascOrder = true)
        {
            List<Type> list = new List<Type>();
            ForEachInheritanceChain(type, (i, t) => list.Add(t), ascOrder);
            return list.ToArray();
        }



        /// <summary>
        /// 遍历某制定类型的继承链，从Object开始循环对类型调用doAction
        /// </summary>
        /// <param name="type">要遍历继承链的类型</param>
        /// <param name="doAction">action,第一参数为包含特性方法的索引</param>
        /// <param name="ascOrder">是否应从从顶端开始，否则从末端开始</param>
        public static void ForEachInheritanceChain(this Type type, Action<int, Type> doAction, bool ascOrder = true)
        {
            if (ascOrder)
            {
                Stack<Type> parents = new Stack<Type>();
                for (Type tempType = type; tempType != null; tempType = tempType.BaseType)
                {
                    parents.Push(tempType);
                }
                int index = 0;
                while (parents.Count > 0)
                {
                    doAction(index++, parents.Pop());
                }
            }
            else
            {
                int index = 0;
                for (Type tempType = type; tempType != null; tempType = tempType.BaseType)
                {
                    doAction(index++, tempType);
                }
            }
        }

    }
}
