using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetLinkPhim
{
   public abstract class AbsGetLinkPhim
    {
   
        protected AbsGetLinkPhim(string host,string urlPage)
        {
            LstUrls= new List<string>();
            LstPhims=new List<PhimInfo>();
            Host = host;
            UrlPage = urlPage;
        
        }
        public string UrlPage { get; set; }
        public string Host { get; set; }
        public List<string> LstUrls { get; set; }
        public List<PhimInfo> LstPhims { get; set; }
        public abstract void GetAllListUrl();
        public abstract PhimInfo GetPhimOnPage(string url);
        public abstract void GetAllPhim();


       
        public virtual void DownloadWithIdm(PhimInfo phim,string localsave)
        {
            var local = localsave.Trim();
            var linkvideo = phim.GetUrl;
            var filevideo = Path.GetFileName(phim.GetUrl)?.Replace(" ", "-").Replace(@"%20","-");
            var sub1 = phim.Chap + "-" + Path.GetFileName(phim.Sub1);
            var sub2 = phim.Chap + "-" + Path.GetFileName(phim.Sub2);
            if (string.IsNullOrWhiteSpace(linkvideo) || string.IsNullOrWhiteSpace(local))
            {
                MessageBox.Show("Chưa đặt đường dẫn lưu hoặc không tồn tại link video");
                return;
            }
            var localChap = local;
            if (!Directory.Exists(localChap))
            {
                Directory.CreateDirectory(localChap);
            }
            IdmDownload(linkvideo.Trim().Replace(" ", "%20"), localChap, filevideo);
            if (!string.IsNullOrWhiteSpace(phim.Sub1))
                IdmDownload(phim.Sub1, localChap, sub1);

            if (!string.IsNullOrWhiteSpace(phim.Sub2))
                IdmDownload(phim.Sub2, localChap, sub2);
        }
        protected void IdmDownload(string link, string localPath, string filename)
        {
            Process IDM = new Process();
            IDM.StartInfo.FileName = @"C:\Program Files (x86)\Internet Download Manager\\IDMan.exe";
            IDM.StartInfo.Arguments = $"/d {link} /p {localPath} /f {filename} /a";
            IDM.Start();
        }
        public abstract string DeCompressJavascript(string text);
      
    }
}
