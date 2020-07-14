using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace wform_v3.Models
{
    public static class HttpClientX
    {
        private static object _locker = new object();
        private static List<BoxIWebDriver> _drivers = new List<BoxIWebDriver>();
        public static void Close()
        {
            foreach(var d in _drivers)
            {
                try
                {
                    d.Driver.Quit();
                }
                catch
                {

                }
            }
        }
        public static async Task Init()
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(
                Task.Run(() => 
                {
                    IWebDriver driver;
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments(new List<string>() { "headless", "disable-gpu" });
                    var chromeDriverService = ChromeDriverService.CreateDefaultService();
                    chromeDriverService.HideCommandPromptWindow = true;
                    driver = new ChromeDriver(chromeDriverService, options);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    BoxIWebDriver box = new BoxIWebDriver(driver);
                    _drivers.Add(box);
                }));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task<string> Find(string url)
        {
            BoxIWebDriver box;
            string resp = string.Empty;

            await Task.Run(() => {
                while (true)
                    lock (_locker)
                    {
                        box = _drivers.FirstOrDefault(x => x.Free == true);
                        if (box != null)
                        {
                            box.Free = false;
                            break;
                        }
                        else
                            Thread.Sleep(1000);
                    }

                IWebDriver driver = box.Driver;
                Actions action = new Actions(driver);

                try
                {
                    driver.Navigate().GoToUrl(url);
                    action.MoveByOffset(5, 5).Perform();
                    action.MoveByOffset(10, 15).Perform();
                    action.MoveByOffset(20, 15).Perform();
                    resp = driver.PageSource;
                }
                catch
                {

                }
                box.Free = true;
            });

            return resp;

        }
        class BoxIWebDriver
        {
            public IWebDriver Driver;
            public bool Free = true;

            public BoxIWebDriver(IWebDriver driver)
            {
                Driver = driver;
            }

        }
    }
}
