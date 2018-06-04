using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace GetLinkPhim
{
    public class PhimStudymovie : AbsGetLinkPhim
    {
        private const string key = "AM0tDdGMmqIHJHyBkS9dTwTJJjyPn3Dj";

        public PhimStudymovie(string urlPage, WebBrowser web1) : base(@"http://studymovie.net", urlPage)
        {
            web = web1;
        }


        public override void GetAllListUrl()
        {
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            HtmlAgilityPack.HtmlDocument doc = htmlWeb.Load(UrlPage);
            var nodes = doc.DocumentNode.QuerySelectorAll(@"div#content > a").ToList();
            if (nodes.Count == 0)
            {
                LstUrls.Add(UrlPage);
            }
            else
            {
                var lstItems = from n in nodes
                    select Host + n.Attributes["href"].Value;
                LstUrls = lstItems.ToList();
            }
        }

        public override PhimInfo GetPhimOnPage(string url)
        {
            PhimInfo phim = new PhimInfo();
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            HtmlAgilityPack.HtmlDocument doc = htmlWeb.Load(url);
            phim.Link = url;

            var enUrl = doc.DocumentNode.QuerySelector("input#video").Attributes["value"].Value;
            var enSub1 = doc.DocumentNode.QuerySelector("input#en").Attributes["value"].Value;
            var enSub2 = doc.DocumentNode.QuerySelector("input#vi").Attributes["value"].Value;

            if (!string.IsNullOrWhiteSpace(enUrl))
                phim.GetUrl = DeCompressJavascript(enUrl);

            if (!string.IsNullOrWhiteSpace(enSub1))
                phim.Sub1 = Host + DeCompressJavascript(enSub1);

            if (!string.IsNullOrWhiteSpace(enSub2))
                phim.Sub2 = Host + DeCompressJavascript(enSub2);

            var chap = Regex.Match(url, @"(?:=)(\d+)").Groups[1].Value;
            chap = chap.Length == 1 ? "0" + chap : chap;
            var name = Utils.ChuanHoaFileName(doc.DocumentNode.QuerySelector("title").InnerText);
            phim.Name = (name.Split('-')[0].Trim().Replace(" ", "-") + "-" + chap).ToUpper();

            return phim;
        }

        public override void GetAllPhim()
        {
            GetAllListUrl();
            foreach (string item in LstUrls)
            {
                if(!item.Contains("="))
                    continue;
                LstPhims.Add(GetPhimOnPage(item));
            }
        }

        public override string DeCompressJavascript(string text)
        {
            try
            {
                var obj = MyInvokeScript("MyInvokeScript", text, key);
                return obj.ToString();
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        private WebBrowser web;

        private object MyInvokeScript(string name, params object[] args)
        {
            return web.Document?.InvokeScript(name, args);
        }
    }
}