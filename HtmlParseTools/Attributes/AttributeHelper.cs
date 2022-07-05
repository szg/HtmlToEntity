using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HtmlParseTools.Attributes
{
    internal class AttributeHelper
    {
        /// <summary>
        /// 获取表名属性内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static HtmlNodeAttribute GetRootAttribute<T>() where T : class, new()
        {
            HtmlRootAttribute rootAttribute = new HtmlRootAttribute();

            var type = typeof(T);
            var customerAtributes = type.GetCustomAttributes(typeof(HtmlNodeAttribute), false);

            if (customerAtributes != null)
            {
                var attr = (customerAtributes[0] as HtmlNodeAttribute);
                rootAttribute.Selector = attr.Selector;
                rootAttribute.IsSingle = attr.IsSingle;
                rootAttribute.Index = attr.Index;
                rootAttribute.ValueFrom = attr.ValueFrom;
                rootAttribute.AttributeValue = attr.AttributeValue;
                rootAttribute.Ignore = attr.Ignore;
                rootAttribute.TrimCharacter = attr.TrimCharacter.Trim();
            }
            rootAttribute.TableName = type.Name;
            return rootAttribute;
        }
        /// <summary>
        /// 获取字段名属性内容集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static List<KeyValuePair<string, HtmlNodeAttribute>> GetNodesAttributes<T>() where T : class, new()
        {
            List<KeyValuePair<string, HtmlNodeAttribute>> list = new List<KeyValuePair<string, HtmlNodeAttribute>>();
            var type = typeof(T);
            var propertyInfos = type.GetProperties();
            foreach (var property in propertyInfos)
            {
                HtmlNodeAttribute nodeAttribute = new HtmlNodeAttribute();
                var nodeAttributes = property.GetCustomAttributes(typeof(HtmlNodeAttribute), false);
                if (nodeAttributes == null || nodeAttributes.Length < 1)
                {
                    nodeAttribute.Ignore = true;
                }
                else
                {
                    var attr = nodeAttributes[0] as HtmlNodeAttribute;
                    nodeAttribute = attr;
                    if (nodeAttribute.ValueFrom == Enums.HtmlNode.None)
                        nodeAttribute.Ignore = true;
                }
                nodeAttribute.Selector = nodeAttribute.Selector.Trim();
                nodeAttribute.TrimCharacter = nodeAttribute.TrimCharacter.Trim();
                list.Add(new KeyValuePair<string, HtmlNodeAttribute>(property.Name, nodeAttribute));
            }
            return list;
        }
    }
}