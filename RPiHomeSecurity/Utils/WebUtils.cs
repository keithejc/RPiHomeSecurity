using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPiHomeSecurity.Utils
{
    public class WebUtils
    {
        public static string GetPublicIp()
        {
            var url = "http://checkip.dyndns.org";
            var req = System.Net.WebRequest.Create(url);
            var resp = req.GetResponse();
            var sr = new System.IO.StreamReader(resp.GetResponseStream());
            var response = sr.ReadToEnd().Trim();
            var a = response.Split(':');
            var a2 = a[1].Substring(1);
            var a3 = a2.Split('<');
            var a4 = a3[0];
            return a4;
        }
    }
}
