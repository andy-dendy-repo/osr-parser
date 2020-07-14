using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;

namespace wform_v3.Models
{
    public class TagHunter
    {
        public delegate Task Info(string info, bool clear);

        public Info Notify;

        private object _locker = new object();

        private List<Link> _links = new List<Link>();

        public TagHunter(List<Link> links, Info info)
        {
            _links = links;
            Notify = info;
        }

        public async Task Perform()
        {
            int len = _links.Count;
            await Notify("Finished:", true);
            await Task.Run(()=> { 
            Parallel.For(0, 5, i => {
            
                while(_links.Exists(x=>!x.Checked))
                {
                    Link link;
                    lock(_locker)
                    {
                        link = _links.FirstOrDefault(x => !x.Checked);
                        if(link!=null)
                        link.Checked = true;
                    }
                    string content = HttpClientX.Find(link.Url).Result;
                    string result = string.Empty;
                    try
                    {
                        HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                        html.LoadHtml(content);
                        var hs = html.DocumentNode.SelectNodes(".//*");
                        foreach (var h in hs)
                        {
                            string tag = h.Name;
                            if (tag.Equals("h1") || tag.Equals("h2") || tag.Equals("h3") || tag.Equals("h4") || tag.Equals("h5") || tag.Equals("h6"))
                            {
                                result += "\r\n"+h.OuterHtml + "\r\n";
                            }
                        }
                    }
                    catch
                    {

                    }

                    Notify(link.Number.ToString()+" ; ", false).Wait();

                    Application.Current.Dispatcher.Invoke(()=> { link.Content =link.Url+"\r\n"+ result; });
                }

            });
            });

        }
    }
}
