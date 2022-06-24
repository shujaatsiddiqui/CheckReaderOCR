using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OCRCheckUPD
{
    static class Program
    {
        static string imgPath = "./halfimage.png";
        static void Main()
        {
            // GetRequest().Wait();
            var response = MakeRequestByte().Result;
            Thread.Sleep(5000);
            GetRequest(response).Wait();
            //Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        private static async Task GetRequest(HttpResponseMessage httpResponseMessage)
        {
            string responseUrl = httpResponseMessage.Headers.Where(r => r.Key == "Operation-Location").Select(r => r.Value).FirstOrDefault().ToList()[0];
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2aef25caad4f4f8dab673c3ce9be9404");
            var resp = await client.GetAsync(responseUrl);
            string text = await resp.Content.ReadAsStringAsync();
            // text = text.Replace("⑆", "|").Replace("⑈", "@");
            Console.WriteLine("Routing Number : " + Regex.Match(text, "(?<=⑆)(.*?)(?=⑆)").Value); // look around: find anything between ⑆ this.
            Console.WriteLine("Account Number : " + Regex.Match(text, "(\\d{8,12})(?=⑈)").Value); // positive lookahead : Most bank account numbers have between 8 and 12 digits, though they can range from 5 to 17
        }

        static async Task<HttpResponseMessage> MakeRequest()
        {
            #region bytes
            //var client = new HttpClient();
            //var queryString = HttpUtility.ParseQueryString(string.Empty);

            //// Request headers
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2aef25caad4f4f8dab673c3ce9be9404");

            //// Request parameters
            ////queryString["language"] = "{string}";
            ////queryString["pages"] = "{string}";
            ////queryString["readingOrder"] = "{string}";
            ////queryString["model-version"] = "latest";
            //var uri = "https://westus2.api.cognitive.microsoft.com/vision/v3.2/read/analyze?" + queryString;

            //HttpResponseMessage response;
            //Image image = Image.FromFile("./check.png");
            //// Request body
            //byte[] byteData = ImageToByteArray(image);//Encoding.UTF8.GetBytes("{body}");

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //    response = await client.PostAsync(uri, content);
            //    string rslt = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine(rslt);
            //} 
            #endregion

            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2aef25caad4f4f8dab673c3ce9be9404");

            // Request parameters
            //queryString["language"] = "{string}";
            //queryString["pages"] = "{string}";
            //queryString["readingOrder"] = "{string}";
            //queryString["model-version"] = "latest";
            var uri = "https://westus2.api.cognitive.microsoft.com/vision/v3.2/read/analyze?" + queryString;

            HttpResponseMessage response;
            var json = JsonConvert.SerializeObject(new root() { url = "https://shumystorageacc.blob.core.windows.net/images/check.png" });
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            //using (var content = new ByteArrayContent(byteData))
            //{
            //client.DefaultRequestHeaders.Accept content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(uri, data);
            //string rslt = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(rslt);
            ////}
            return response;

        }

        static async Task<HttpResponseMessage> MakeRequestByte()
        {
            #region bytes
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "2aef25caad4f4f8dab673c3ce9be9404");

            // Request parameters
            //queryString["language"] = "{string}";
            //queryString["pages"] = "{string}";
            //queryString["readingOrder"] = "{string}";
            //queryString["model-version"] = "latest";
            var uri = "https://westus2.api.cognitive.microsoft.com/vision/v3.2/read/analyze?" + queryString;

            HttpResponseMessage response;
            Image image = Image.FromFile(imgPath);
            // Request body
            byte[] byteData = ImageToByteArray(image);//Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                return response;
            }

            #endregion       

        }
        private class root
        {
            public string url { get; set; }
        }

        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }
    }
}
