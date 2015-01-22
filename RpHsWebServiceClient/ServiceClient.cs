using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RpHsWebServiceLib;

namespace HSControl
{
    internal class ServiceClient
    {
        private static String baseAddress = "http://192.168.1.131:33331/RpHsWebService/";

        public static String Arm(bool armed)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(baseAddress + "ArmedSet");
            req.Method = "POST";
            req.ContentType = @"application/json; charset=utf-8";

            String body = "{\"armed\":\"" + (armed ? "true" : "false") + "\"}";
            byte[] data = Encoding.UTF8.GetBytes(body);

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(data, 0, data.Length);
            dataStream.Close();

            var response = req.GetResponse() as HttpWebResponse;

            String json;
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            bool result = JsonConvert.DeserializeObject<bool>(json);
            return result.ToString();
        }

        public static string GetStatus()
        {
            WebRequest req = System.Net.WebRequest.Create(baseAddress + "Status");
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());

            String json;
            using (var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                json = reader.ReadToEnd();
            }

            AlarmStatus status = JsonConvert.DeserializeObject<AlarmStatus>(json);

            return "Arm: " + status.Armed + " alm: " + status.InAlarm;
        }
    }
}