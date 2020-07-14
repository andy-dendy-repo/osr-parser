using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wform_v3.Models;

namespace wform_v3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            bt_search.IsEnabled = false;
            Init();
        }
        private async void Init()
        {
            await Info("launching...", true);
            await HttpClientX.Init();
            bt_search.IsEnabled = true;
            await Info("ready",true);
        }
        private async Task Info(string info, bool clear)
        {
            await Application.Current.Dispatcher.InvokeAsync(()=> { if (clear) lb_info.Content = string.Empty; lb_info.Content += info; });
        }
        
        private async void bt_search_Click(object sender, RoutedEventArgs e)
        {
            lw_links.Items.Clear();
            tb_tags.Text = "";
            int num = int.Parse(tb_depth.Text);

            TopParser topParser = new TopParser(num, Info);

            var list = await topParser.GetTop(tb_phase.Text);

            list = list.Take(num).ToList();

            List<Link> links = list.Select((x, y) => new Link() {Number=y, Url=x }).ToList();

            int len = links.Count;

            for (int i = 0; i < len; i++)
                lw_links.Items.Add(links[i]);

            TagHunter hunter = new TagHunter(links, Info);
            await hunter.Perform();

            cb_keys.Items.Add(tb_phase.Text);
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                HttpClientX.Close();
            }
            catch
            {

            }
            base.OnClosing(e);
        }

        

        private void lw_links_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = lw_links.SelectedItem;
            
            if (obj != null)
            {
                Link link = obj as Link;
                if (link.Content != null)
                    tb_tags.Text = link.Content;
                else
                    tb_tags.Text = "Processing...";
            }
        }
    }
}
