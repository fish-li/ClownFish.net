using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;
using ClownFish.Base;

namespace ClownFish.UnitTest
{
    internal static class SomeUtils
    {
        public static object GetFieldValue(this object obj, string name)
        {
            FieldInfo field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if( field == null )
                field = obj.GetType().BaseType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return field.GetValue(obj);
        }

        public static object GetFieldValue(this Type type, string name)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return field.GetValue(null);
        }

        public static void SetFieldValue(this object obj, string name, object value)
        {
            FieldInfo field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if( field == null )
                field = obj.GetType().BaseType.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(obj, value);
        }

        public static void SetFieldValue(this Type type, string name, object value)
        {
            FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, value);
        }

        public static object InvokeMethod(this object obj, string name, params object[] args)
        {
            MethodInfo method = obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if( method == null )
                method = obj.GetType().BaseType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return method.Invoke(obj, args);
        }

        public static object InvokeMethod(this Type type, string name, params object [] args)
        {
            MethodInfo method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return method.Invoke(null, args);
        }

        public static string GetTempPath(this AppDomain app)
        {
            return Path.Combine(app.BaseDirectory, "temp");
        }


        public static T[] Add<T>(this T[] array, T item)
        {
            List<T> list = array.ToList();
            list.Add(item);
            return list.ToArray();
        }



        public static Dictionary<string, string> ToHeaderDictionary(this string headerText)
        {
            if( headerText.IsNullOrEmpty() )
                return new Dictionary<string, string>();

            var list = headerText.Trim().ToKVList(StringExtensions.LineSeparators, ':');
            Dictionary<string, string> collection = new Dictionary<string, string>(list.Count, StringComparer.OrdinalIgnoreCase);

            foreach( var nv in list )
                collection[nv.Name] = nv.Value;

            return collection;
        }
    }
}
