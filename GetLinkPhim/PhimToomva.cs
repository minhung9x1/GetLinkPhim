using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetLinkPhim
{
    public class PhimToomva : AbsGetLinkPhim
    {
        public PhimToomva(string urlPage) : base(@"https://toomva.com", urlPage)
        {
        }

        public override void GetAllListUrl()
        {
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            HtmlAgilityPack.HtmlDocument doc = htmlWeb.Load(UrlPage);
            var nodes = doc.DocumentNode.QuerySelectorAll("div.scroll-eps > div.grid-search-video").ToList();
            if (nodes.Count == 0)
            {
                LstUrls.Add(UrlPage);
            }
            else
            {
                var lstItems = from n in nodes
                    select n.QuerySelector("div.search-video-content > a").Attributes["href"].Value;
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
            phim.GetUrl = doc.DocumentNode.QuerySelector("#video source").Attributes["src"].Value;
            var name = doc.DocumentNode.QuerySelector("#container h2.title").InnerText.ToUpper();
            phim.Name = Utils.ChuanHoaFileName(name).Trim().Replace(" ","-");
            var script = doc.DocumentNode.SelectNodes("/html/body/script[4]").FirstOrDefault();
            if (script != null)
            {
                var matches = Regex.Matches(script.InnerText, @"\/Data\/.+\.vtt");
                if (matches.Count > 1)
                    phim.Sub1 = Host + matches[0].Value;
                if (matches.Count >= 2)
                    phim.Sub2 = Host + matches[1].Value;
            }
            return phim;
        }

        protected string GetFileNameFromUrl(string url)
        {
            int start = url.LastIndexOf(@"/");
            int end = url.LastIndexOf("-season") > -1 ? url.LastIndexOf("-season") : url.LastIndexOf("=");
            if (start == -1 || end == -1)
                return string.Empty;
            return url.Substring(start + 1, end - start - 1);
        }

        public override void GetAllPhim()
        {
            GetAllListUrl();
            foreach (string item in LstUrls)
            {
                LstPhims.Add(GetPhimOnPage(item));
            }
        }

        public override string DeCompressJavascript(string text)
        {
            throw new NotImplementedException();
        }
    }
}