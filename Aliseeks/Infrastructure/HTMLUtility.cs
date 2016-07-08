using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HtmlAgilityPack;

namespace Aliseeks.Infrastructure
{
    public static class HTMLUtility
    {
        public static HtmlNode GetNode_ByClass(HtmlNode node, string _class)
        {
            return node.Descendants().FirstOrDefault(p => p.Attributes.Contains("class") && p.Attributes["class"].Value.Contains(_class));
        }
    }
}