using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleImageDownloader
{
    class Program
    {
        const string searchKeyword = "แพทริเซีย ธัญชนก กู๊ด";
        const string outputPath = @"C:\Users\u6039635\Desktop\outputPath\";
        static void Main(string[] args)
        {
            var outputDir = string.Format(@"{0}{1}", outputPath, searchKeyword);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(string.Format(@"{0}{1}", outputPath, searchKeyword));
            }


            string html = GetHtmlCode();
            List<string> urls = GetUrls(html);
            var rnd = new Random();

            List<byte[]> images = new List<byte[]>();
            foreach(var url in urls)
            {
                byte[] image = GetImage(url);

                if (image != null)
                {
                    images.Add(image);
                }
            }

            foreach(var img in images)
            {
                using (var ms = new MemoryStream(img))
                {
                    var filePath = string.Format(@"{0}{1}\{2}.jpg", outputPath, searchKeyword, images.IndexOf(img));
                    using (FileStream file = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.ReadWrite))
                    {
                        byte[] bytes = new byte[ms.Length];
                        ms.Read(bytes, 0, (int)ms.Length);
                        file.Write(bytes, 0, bytes.Length);
                        ms.Close();
                    }
                }
            }
        }

        private static string GetHtmlCode()
        {
            var rnd = new Random();

            string url = "https://www.google.com/search?q=" + searchKeyword + "&tbm=isch";
            string data = "";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                if (dataStream == null)
                    return "";
                using (var sr = new StreamReader(dataStream))
                {
                    data = sr.ReadToEnd();
                }
            }
            return data;
        }

        private static List<string> GetUrls(string html)
        {
            var urls = new List<string>();

            int ndx = html.IndexOf("\"ou\"", StringComparison.Ordinal);

            while (ndx >= 0)
            {
                ndx = html.IndexOf("\"", ndx + 4, StringComparison.Ordinal);
                ndx++;
                int ndx2 = html.IndexOf("\"", ndx, StringComparison.Ordinal);
                string url = html.Substring(ndx, ndx2 - ndx);
                urls.Add(url);
                ndx = html.IndexOf("\"ou\"", ndx2, StringComparison.Ordinal);
            }
            return urls;
        }

        private static byte[] GetImage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream == null)
                        return null;
                    using (var sr = new BinaryReader(dataStream))
                    {
                        byte[] bytes = sr.ReadBytes(100000000);

                        return bytes;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
