using AngleSharp.Html.Parser;
using HtmlParseTools.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace HtmlParseTools.HtmlHelper
{
    internal class HtmlConverter
    {
        internal static Dictionary<string, List<string>> ParseFirstSuit<T>(string htmlContent, out bool success) where T : class, new()
        {
            success = false;
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            var rootAttr = AttributeHelper.GetRootAttribute<T>();
            if (string.IsNullOrEmpty(rootAttr.Selector))
                return dic;
            var attrList = HtmlEntityConverter.GetNodesAttributes<T>();
            try
            {
                var document = new HtmlParser().ParseDocument(htmlContent);
                if (document.ChildElementCount < 1)
                    return dic;
                IElement root = GetRootElement(rootAttr, document);
                if (root == null)
                    return dic;
                foreach (var attr in attrList)
                {
                    if (attr.Value.Ignore)
                        continue;
                    if (string.IsNullOrEmpty(attr.Value.Selector))
                        continue;
                    var list = GetElementValue(attr.Value, root);
                    dic.Add(attr.Key, list);
                }
                success = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return dic;
        }

        internal static List<Dictionary<string, List<string>>> ParseAllSuitable<T>(string htmlContent, out bool success) where T : class, new()
        {
            success = false;
            List<Dictionary<string, List<string>>> list = new List<Dictionary<string, List<string>>>();
            success = false;
            var rootAttr = AttributeHelper.GetRootAttribute<T>();
            if (string.IsNullOrEmpty(rootAttr.Selector))
                return list;
            var attrList = HtmlEntityConverter.GetNodesAttributes<T>();
            try
            {
                var document = new HtmlParser().ParseDocument(htmlContent);
                if (document.ChildElementCount < 1)
                    return list;
                if (rootAttr.IsSingle)
                {
                    IElement root = GetRootElement(rootAttr, document);
                    if (root == null)
                        return list;
                    var dic = new Dictionary<string, List<string>>();
                    foreach (var attr in attrList)
                    {
                        if (attr.Value.Ignore)
                            continue;
                        if (string.IsNullOrEmpty(attr.Value.Selector))
                            continue;
                        var values = GetElementValue(attr.Value, root);
                        if (values != null && values.Count > 0)
                        {
                            dic.Add(attr.Key, values);
                        }
                    }
                    list.Add(dic);
                }
                else
                {
                    var roots = GetRootElements(rootAttr, document);
                    if (roots == null || roots.Length < 1)
                        return list;
                    foreach (var root in roots)
                    {
                        var dic = new Dictionary<string, List<string>>();
                        foreach (var attr in attrList)
                        {
                            if (attr.Value.Ignore)
                                continue;
                            if (string.IsNullOrEmpty(attr.Value.Selector))
                                continue;
                            var values = GetElementValue(attr.Value, root);
                            if (values != null && values.Count > 0)
                            {
                                dic.Add(attr.Key, values);
                            }
                        }
                        list.Add(dic);
                    }
                }
                success = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return list;
        }

        private static IElement GetRootElement(HtmlNodeAttribute attribute, IHtmlDocument document)
        {
            IElement e = null;

            if (attribute.Index == 1)
            {
                e = document.QuerySelector(attribute.Selector);
            }
            else
            {
                var ec = document.QuerySelectorAll(attribute.Selector);
                if (ec.Length < attribute.Index)
                    return null;
                return ec[attribute.Index - 1];
            }
            return e;
        }

        private static IHtmlCollection<IElement> GetRootElements(HtmlNodeAttribute attribute, IHtmlDocument document)
        {
            IHtmlCollection<IElement> elements = document.QuerySelectorAll(attribute.Selector);
            return elements;
        }
        private static List<string> GetElementValue(HtmlNodeAttribute attribute, IElement element)
        {
            List<string> valueList = new List<string>();
            if (attribute.IsSingle)
            {
                if (attribute.Index == 1)
                {
                    var e = element.QuerySelector(attribute.Selector);
                    if (e == null)
                        return valueList;
                    valueList = GetElementInnerValue(attribute, e);
                }
                else
                {
                    var e = element.QuerySelectorAll(attribute.Selector);
                    if (e.Length < attribute.Index)
                        return valueList;
                    valueList = GetElementInnerValue(attribute, e[attribute.Index - 1]);
                }
            }
            else
            {
                var elements = element.QuerySelectorAll(attribute.Selector);
                if (elements != null && elements.Length > 0)
                    valueList = GetElementInnerValue(attribute, elements);
            }
            return valueList;
        }

        private static List<string> GetElementInnerValue(HtmlNodeAttribute attribute, IElement element)
        {
            List<string> valueList = new List<string>();
            string value = null;
            switch (attribute.ValueFrom)
            {
                case Enums.HtmlNode.Html:
                    value = element.InnerHtml;
                    break;
                case Enums.HtmlNode.Content:
                    value = element.TextContent;
                    break;
                case Enums.HtmlNode.Attribute:
                    if (!string.IsNullOrEmpty(attribute.AttributeValue) && element.HasAttribute(attribute.AttributeValue))
                        value = element.GetAttribute(attribute.AttributeValue);
                    break;
            }
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(attribute.TrimCharacter))
            {
                var splist = attribute.TrimCharacter.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var sp in splist)
                    value = value.Replace(sp, "");
            }
            if (!string.IsNullOrEmpty(value))
                valueList.Add(value.Trim());
            return valueList;
        }
        private static List<string> GetElementInnerValue(HtmlNodeAttribute attribute, IHtmlCollection<IElement> elements)
        {
            List<string> valueList = new List<string>();
            foreach (var element in elements)
            {
                string value = null;
                switch (attribute.ValueFrom)
                {
                    case Enums.HtmlNode.Html:
                        value = element.InnerHtml;
                        break;
                    case Enums.HtmlNode.Content:
                        value = element.TextContent;
                        break;
                    case Enums.HtmlNode.Attribute:
                        if (!string.IsNullOrEmpty(attribute.AttributeValue) && element.HasAttribute(attribute.AttributeValue))
                            value = element.GetAttribute(attribute.AttributeValue);
                        break;
                }
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(attribute.TrimCharacter))
                {
                    var splist = attribute.TrimCharacter.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var sp in splist)
                        value = value.Replace(sp, "");
                }
                if (!string.IsNullOrEmpty(value))
                    valueList.Add(value.Trim());
            }
            return valueList;
        }

    }
}
