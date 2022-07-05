using HtmlParseTools.Attributes;
using HtmlParseTools.HtmlHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

public static class HtmlEntityConverter
{
    private static ConcurrentDictionary<Type, object> CacheInfo = new ConcurrentDictionary<Type, object>();
    private static ConcurrentDictionary<Type, List<KeyValuePair<string, HtmlNodeAttribute>>> CacheRowsInfo = new ConcurrentDictionary<Type, List<KeyValuePair<string, HtmlNodeAttribute>>>();
    internal static Dictionary<string, Action<T, object>> GetSetter<T>() where T : class, new()
    {
        var type = typeof(T);
        if (CacheInfo.TryGetValue(type, out var result))
        {
            if (result is Dictionary<string, Action<T, object>>)
                return result as Dictionary<string, Action<T, object>>;
        }
        var list = new Dictionary<string, Action<T, object>>();
        var propertyInfos = type.GetProperties();
        foreach (var property in propertyInfos)
        {
            if (!property.CanWrite) continue;
            ParameterExpression parameter = Expression.Parameter(type, property.Name);
            ParameterExpression value = Expression.Parameter(typeof(object), property.Name);
            MethodInfo setter = type.GetMethod("set_" + property.Name);
            MethodCallExpression call = Expression.Call(parameter, setter, Expression.Convert(value, property.PropertyType));
            var lambda = Expression.Lambda<Action<T, object>>(call, parameter, value);
            var action = lambda.Compile();
            list.Add(property.Name, action);
        }
        CacheInfo[type] = list;
        return list;
    }

    internal static List<KeyValuePair<string, HtmlNodeAttribute>> GetNodesAttributes<T>() where T : class, new()
    {
        var type = typeof(T);
        if (CacheRowsInfo.TryGetValue(type, out var result))
        {
            if (result is List<KeyValuePair<string, HtmlNodeAttribute>>)
                return result as List<KeyValuePair<string, HtmlNodeAttribute>>;
        }
        var attrList = AttributeHelper.GetNodesAttributes<T>();
        if (attrList != null && attrList.Count > 0)
            CacheRowsInfo[type] = attrList;
        return attrList;
    }

    public static List<T> ToList<T>(this string htmlContent) where T : class, new()
    {
        List<T> list = new List<T>();
        if (string.IsNullOrEmpty(htmlContent))
        {
            return list;
        }
        var setterList = GetSetter<T>();
        var attrList = GetNodesAttributes<T>();
        var type = typeof(T);
        var propertyInfos = type.GetProperties();
        var parsedDoc = HtmlConverter.ParseAllSuitable<T>(htmlContent, out var success);
        if (!success)
            return list;

        foreach (var doc in parsedDoc)
        {
            T t = new T();
            foreach (var kvp in doc)
            {
                var property = propertyInfos.FirstOrDefault(m => m.Name == kvp.Key);
                if (property == null)
                    continue;
                var attr = attrList.FirstOrDefault(m => m.Key == kvp.Key);
                if (attr.Value == null | attr.Value.Ignore)
                    continue;
                var setter = setterList.FirstOrDefault(m => m.Key == kvp.Key);
                if (setter.Value == null)
                    continue;
                if (kvp.Value == null || kvp.Value.Count < 1)
                    continue;
                SetValue(t, kvp, setter, property);
            }
            list.Add(t);
        }
        return list;
    }

    public static T ToEntity<T>(this string htmlContent) where T : class, new()
    {
        if (string.IsNullOrEmpty(htmlContent))
            return default(T);

        var setterList = GetSetter<T>();
        var attrList = GetNodesAttributes<T>();
        var type = typeof(T);
        var propertyInfos = type.GetProperties();
        var parsedDoc = HtmlConverter.ParseFirstSuit<T>(htmlContent, out var success);
        if (!success)
            return default(T);
        T t = new T();
        foreach (var kvp in parsedDoc)
        {
            var property = propertyInfos.FirstOrDefault(m => m.Name == kvp.Key);
            if (property == null)
                continue;
            var attr = attrList.FirstOrDefault(m => m.Key == kvp.Key);
            if (attr.Value == null | attr.Value.Ignore)
                continue;
            var setter = setterList.FirstOrDefault(m => m.Key == kvp.Key);
            if (setter.Value == null)
                continue;
            if (kvp.Value == null || kvp.Value.Count < 1)
                continue;
            SetValue(t, kvp, setter, property);
        }
        return t;
    }

    private static void SetValue<T>(T t, KeyValuePair<string, List<string>> kvp, KeyValuePair<string, Action<T, object>> setter, PropertyInfo property) where T : class, new()
    {
        string value = null;

        if (kvp.Value.Count == 1)
        {
            value = kvp.Value[0];
        }
        else
        {
            if (property.PropertyType == typeof(string))
                value = new StringBuilder().AppendJoin(",", kvp.Value).ToString();
            else
                value = kvp.Value[0];
        }

        var nullable = IsNullableType(property.PropertyType);
        if (!nullable)
        {
            if (value == null)
                throw new Exception($"{nameof(property.Name)} 不能为空");

            object safeValue = Convert.ChangeType(value, property.PropertyType);
            setter.Value(t, safeValue);
        }
        else
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                object safeValue = Convert.ChangeType(value, property.PropertyType);
                setter.Value(t, safeValue);
            }
        }
    }

    internal static bool IsNullableType(Type theType)
    {
        return (theType.IsGenericType && theType.
        GetGenericTypeDefinition().Equals
        (typeof(Nullable<>)));
    }
}