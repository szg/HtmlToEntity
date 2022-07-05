using HtmlParseTools.Attributes;
using HtmlParseTools.Enums;

namespace ConsoleApp1
{

    [HtmlNode(Selector = ".sellListContent li", IsSingle = false)]
    public class FangJia
    {
        /// <summary>
        /// 标题
        /// </summary>
        [HtmlNode(Selector = ".title", ValueFrom = HtmlNode.Content)]
        public string Title { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [HtmlNode(Selector = "img[class='lj-lazy']", ValueFrom = HtmlNode.Attribute, AttributeValue = "data-original")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [HtmlNode(Selector = ".unitPrice", ValueFrom = HtmlNode.Attribute, AttributeValue = "data-price")]
        public int Price { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        [HtmlNode(Selector = ".totalPrice", ValueFrom = HtmlNode.Content, TrimCharacter = "万,元", IsSingle = true)]
        public decimal Total { get; set; }

        /// <summary>
        /// 小区名称
        /// </summary>
        [HtmlNode(Selector = ".positionInfo a", Index = 1, ValueFrom = HtmlNode.Content)]
        public string CommunityName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [HtmlNode(Selector = ".positionInfo a", Index = 2, ValueFrom = HtmlNode.Content)]
        public string Address { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [HtmlNode(Selector = ".tag span", IsSingle = false, ValueFrom = HtmlNode.Content)]
        public string Marks { get; set; }

        public override string ToString()
        {
            return $"{CommunityName}\t{Price}\t{Total}\t{Address}\t{Title}\t{ImageUrl}\t{Marks}";
        }
    }
}
