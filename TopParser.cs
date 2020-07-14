using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace wform_v3.Models
{
    public class TopParser
    {
        public int Depth { get; set; }

        public delegate Task Info(string info, bool clear);

        public Info Notify;

        private List<string> _pages = new List<string>();

        public TopParser(int depth, Info info)
        {
            Depth = depth;
            Notify = info;
        }

        private async Task<string> GetPages(string key)
        {
            await Notify("getting google pages", true);

            string searchurl = "http://google.com/search?q=" + key;

            string resp = await HttpClientX.Find(searchurl);

            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(resp);

            var table = html.GetElementbyId("foot");

            foreach(var td in table.QuerySelectorAll("a"))
            {
                string href = td.GetAttributeValue("href", "");
                
                _pages.Add("http://google.com"+href);
            }
            return resp;
        }

        public async Task<List<string>> GetTop(string key)
        {
            string resp= await GetPages(key);


            List<string> result = new List<string>();

            string searchurl = "http://google.com/search?q=" + key;

            int page = 0;

            while (true)
            {
                await Notify("getting website links", true);
                
                HtmlAgilityPack.HtmlDocument html;
                try
                {
                    if (page != 0)
                    resp = await HttpClientX.Find(_pages[page]);

                    html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(resp);
                    var hs = html.DocumentNode.QuerySelectorAll("#search div[class=\"g\"] div[class=\"r\"]  a");
                    
                    foreach (var h in hs)
                    {
                        if (h.Name == "a" && h.QuerySelectorAll("h3").Count!=0)
                        {
                            string href = h.GetAttributeValue("href", "");
                            if (href != "")
                            {
                                result.Add(href);
                            }
                        }
                    }

                    
                        if (_pages.Count > page && result.Count < Depth)
                        {
                            page++;
                            continue;
                        }
                        else
                            break;
                    
                }
                catch
                {

                }
                finally
                {
                    
                }
            }
            await Notify("list created", true);
            return result.Where(x=>x.Contains("http:") || x.Contains("https:")).ToList();
        }

    }
}
