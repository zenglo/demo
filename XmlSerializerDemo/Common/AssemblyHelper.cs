using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Atom.Framework.Utility
{

    /// <summary>
    /// 程序集帮助类
    /// </summary>
    public static class AssemblyHelper
    {

        /// <summary>
        /// 遍历某目录中的dll(非递归)
        /// </summary>
        /// <param name="dllDirectory">要遍历的目录</param>
        /// <param name="action">action</param>
        public static void ForEachAssemblys(string dllDirectory, Action<Assembly> action)
        {
            if (!System.IO.Directory.Exists(dllDirectory))
                return;
            FileHelper.ForEachFile(dllDirectory,
                new string[] { ".dll", ".exe" },
                new FileHelper.DoFile(file =>
                {
                    try
                    {
                        action(Assembly.LoadFrom(file));
                    }
                    catch (BadImageFormatException exc)
                    {
                        System.Console.WriteLine(string.Format("-------------忽略了不合法的程序集：{0}", file));
                    }
                }));
        }


        /// <summary>
        /// 递归某目录中的dll数据类型，找到继承自指定基类型且具有无参构造函数的类类型，并创建实例
        /// </summary>
        /// <typeparam name="T">基类型</typeparam>
        /// <param name="dllDirectory">要递归的目录</param>
        /// <returns>满足要求的类类型</returns>
        public static List<T> RecursionNoParaConstructorClassInstances<T>(string dllDirectory)
        {
            List<T> list = new List<T>();
            foreach (var item in RecursionNoParaConstructorClassTypes<T>(dllDirectory))
            {
                list.Add((T)CreateInstanceByNoParaConstructor(item));
            }
            return list;
        }

        /// <summary>
        /// 递归某目录中的dll数据类型，获取继承自指定基类型且具有无参构造函数的类类型
        /// </summary>
        /// <typeparam name="T">基类型</typeparam>
        /// <param name="dllDirectory">要递归的目录</param>
        /// <returns>满足要求的类类型</returns>
        public static List<Type> RecursionNoParaConstructorClassTypes<T>(string dllDirectory)
        {
            return RecursionTypes(dllDirectory, item =>
            {
                if (!typeof(T).IsAssignableFrom(item))
                    return false;
                if (!item.IsPublic)
                    return false;
                if (item.IsAbstract)
                    return false;
                if (!item.IsClass)
                    return false;
                if (item.ContainsGenericParameters)
                    return false;
                if (GetNoParaConstructor(item) == null)
                    return false;
                return true;
            });
        }

        /// <summary>
        /// 递归某目录中的dll数据类型，获取满足条件的数据类型集合
        /// </summary>
        /// <param name="dllDirectory">要递归的目录</param>
        /// <param name="predicate">条件委托</param>
        /// <returns>满足要求的类类型</returns>
        public static List<Type> RecursionTypes(string dllDirectory, Func<Type, bool> predicate)
        {
            if (!System.IO.Directory.Exists(dllDirectory))
                return new List<Type>(0);
            List<Type> list = new List<Type>();
            FileHelper.RecursionFile(dllDirectory,
                new string[] { ".dll", ".exe" },
                new FileHelper.DoFile(file =>
            {
                try
                {
                    foreach (Type item in Assembly.LoadFrom(file).GetTypes())
                    {
                        if (predicate(item))
                        {
                            list.Add(item);
                        }
                    };
                }
                catch (BadImageFormatException exc)
                {
                    System.Console.WriteLine(string.Format("-------------忽略了不合法的程序集：{0}", file));
                }
            }));
            return list;
        }

        /// <summary>
        /// 遍历某目录中的dll数据类型(非递归)，找到继承自指定基类型且具有无参构造函数的类类型，并创建实例
        /// </summary>
        /// <typeparam name="T">基类型</typeparam>
        /// <param name="dllDirectory">要遍历的目录</param>
        /// <returns>满足要求的类类型</returns>
        public static List<T> ForEachNoParaConstructorClassInstances<T>(string dllDirectory)
        {
            List<T> list = new List<T>();
            foreach (var item in ForEachNoParaConstructorClassTypes<T>(dllDirectory))
            {
                list.Add((T)CreateInstanceByNoParaConstructor(item));
            }
            return list;
        }

        /// <summary>
        /// 遍历某目录中的dll数据类型(非递归)，获取继承自指定基类型且具有无参构造函数的类类型
        /// </summary>
        /// <typeparam name="T">基类型</typeparam>
        /// <param name="dllDirectory">要遍历的目录</param>
        /// <returns>满足要求的类类型</returns>
        public static List<Type> ForEachNoParaConstructorClassTypes<T>(string dllDirectory)
        {
            return ForEachTypes(dllDirectory, item =>
            {
                if (!typeof(T).IsAssignableFrom(item))
                    return false;
                if (!item.IsPublic)
                    return false;
                if (item.IsAbstract)
                    return false;
                if (!item.IsClass)
                    return false;
                if (item.ContainsGenericParameters)
                    return false;
                if (GetNoParaConstructor(item) == null)
                    return false;
                return true;
            });
        }

        /// <summary>
        /// 遍历某目录中的dll数据类型(非递归)，获取满足条件的数据类型集合
        /// </summary>
        /// <param name="dllDirectory">要遍历的目录</param>
        /// <param name="predicate">条件委托</param>
        /// <returns>满足要求的类类型</returns>
        public static List<Type> ForEachTypes(string dllDirectory, Func<Type, bool> predicate)
        {
            if (!System.IO.Directory.Exists(dllDirectory))
                return new List<Type>(0);
            List<Type> list = new List<Type>();
            FileHelper.ForEachFile(dllDirectory,
                new string[] { ".dll", ".exe" },
                new FileHelper.DoFile(file =>
                {
                    try
                    {
                        foreach (Type item in Assembly.LoadFrom(file).GetTypes())
                        {
                            if (predicate(item))
                            {
                                list.Add(item);
                            }
                        }
                    }
                    catch (BadImageFormatException exc)
                    {
                        System.Console.WriteLine(string.Format("-------------忽略了不合法的程序集：{0}", file));
                    }
                }));
            return list;
        }

        /// <summary>
        /// 获取某dll中满足条件的类型集合
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<Type> GetAssemblyTypes(string assemblyFile, Func<Type, bool> predicate)
        {
            if (!System.IO.File.Exists(assemblyFile))
                return new List<Type>(0);
            List<Type> list = new List<Type>();
            foreach (Type item in Assembly.LoadFrom(assemblyFile).GetTypes())
            {
                if (predicate(item))
                {
                    list.Add(item);
                }
            };
            return list;
        }



        /// <summary>
        /// 使用无参构造创建实例，无无参构造返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object CreateInstanceByNoParaConstructor(Type type)
        {
            return TypeHelper.CreateInstanceByNoParaConstructor(type);
        }

        /// <summary>
        /// 获取无参构造
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConstructorInfo GetNoParaConstructor(Type type)
        {
            return TypeHelper.GetNoParaConstructor(type);
        }


        /// <summary>
        /// 获取数据类型的去除版本的反射字符串，可通过 Type.GetType反射出该类型
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <returns>可用于通过Type.GetType获取类型的字符串</returns>
        public static string GetTypeRelationNameWithoutVision(Type type)
        {
            return TypeHelper.GetTypeRelationNameWithoutVision(type);
        }
    }
}
