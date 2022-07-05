using HtmlParseTools.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlParseTools.Attributes
{
    /// <summary>
    /// 网页节点属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class HtmlNodeAttribute : Attribute
    {
        /// <summary>
        ///选择器 ,路径、Id，类别
        /// </summary>
        public string Selector { get; set; }
        /// <summary>
        /// 是否只有一个节点，默认为true，true 只取第一个节点，false 取查到全部节点(当字段属性不是 <see cref="String"/> 时，返回第一个节点，是 <see cref="String"/> 时拼接后数据)
        /// </summary>
        public bool IsSingle { get; set; } = true;
        /// <summary>
        /// 要选择的节点 从1 开始,默认为1
        /// </summary>
        public int Index { get; set; } = 1;

        /// <summary>
        /// 要获取内容的来源，默认为<see cref="HtmlNode.Content"/>
        /// </summary>
        public HtmlNode ValueFrom { get; set; } = HtmlNode.Content;

        /// <summary>
        /// 要选择的属性名称,当 <see cref="ValueFrom"/> 为 <see cref="HtmlNode.Attribute"/> 时有效
        /// </summary>
        public string AttributeValue { get; set; }
        /// <summary>
        /// 是否忽略当前字段
        /// </summary>
        public bool Ignore { get; set; } = false;
        /// <summary>
        /// 要去除的字符,多个用","分割
        /// </summary>
        public string TrimCharacter { get; set; } = string.Empty;
    }
}
