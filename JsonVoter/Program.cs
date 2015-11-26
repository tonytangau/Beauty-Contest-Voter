using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace JsonVoter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var tenTimes = Enumerable.Range(0, 10);

            tenTimes.Select(t => Task.Factory.StartNew(() => VoteForAll()));

            //for (var i = 0; i < Int32.MaxValue; i++)
            //{
            //    foreach (var id in ids)
            //    {
            //        VoteFor(id);
            //    }
            //}
        }

        private static void VoteForAll()
        {
            string[] ids = new string[10];

            ids[0] = "144604547405143";
            ids[1] = "144604360323430";
            ids[2] = "144608298159520";
            ids[3] = "144608335822659";
            ids[4] = "14464335834701";
            ids[5] = "144593937871883";

            while (true)
            {
                foreach (var id in ids)
                {
                    VoteFor(id);
                }
            }
        }

        private static void VoteFor(string candidateId)
        {
            var uri = new Uri("http://bla/huiCoffee/voteSubmit");

            try
            {
                using (var webClient = new CookieWebClient())
                {
                    var document = webClient.DownloadString("http://bla/huiCoffee/voteIndex");

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(document);

                    var id = htmlDoc.DocumentNode.Descendants("input").FirstOrDefault(d => d.Attributes["id"].Value == "uky").Attributes["value"].Value;
                    var votes = htmlDoc.DocumentNode.Descendants("label").FirstOrDefault(d => d.Attributes["applicantid"].Value == candidateId).InnerText;

                    webClient.CookieContainer.Add(new Cookie("voterId", id) { Domain = "www.huilc.cn" });

                    Console.WriteLine("Current Votes: " + candidateId + " - " + votes);

                    for (var i = 0; i < 10; i++)
                    {
                        using (var postClient = new CookieWebClient())
                        {
                            var cookie = webClient.CookieContainer.GetCookies(new Uri("http://bla/huiCoffee/voteSubmit"));

                            postClient.CookieContainer.Add(cookie);

                            try
                            {
                                var values = new NameValueCollection { { "voterId", id }, { "applyId", candidateId } };
                                postClient.UploadValues(uri, "POST", values);
                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine(ex.Message);
                                //Console.ForegroundColor = ConsoleColor.Red;
                                //Console.Write("x ");
                                //Console.ResetColor();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }

    public class CookieWebClient : WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// This will instanciate an internal CookieContainer.
        /// </summary>
        public CookieWebClient()
        {
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// Use this if you want to control the CookieContainer outside this class.
        /// </summary>
        public CookieWebClient(CookieContainer cookieContainer)
        {
            this.CookieContainer = cookieContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}
