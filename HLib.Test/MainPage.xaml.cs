using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HLib.Booru;
using HLib.Booru.Controls;
using HLib.Booru.Controls.ViewModel;
using HLib;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HLib.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void LoadPost()
        {
            var time = await TimeHelper.GetInternetTimeUseUrl("http://higan.me/time.php");
            var data = await HttpHelper.GetString("http://konachan.com/post.json?limit=100&tags=rating:s");
            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MoePost>>(data);
            var itemsList = list.Select(p => new BasePost() { Id = (int)p.id, PreviewImageHeight = p.preview_height, PreviewImageWidth = p.preview_width, PreviewImageUrl = new Uri(p.preview_url) })
                .Select(p => new PostViewModel(p))
                .Select(p => new PostItem() { DataContext = p });
            foreach (var item in itemsList)
            {
                postList.Items.Add(new ListViewItem() { Padding = new Thickness(0), Content = item });
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPost();
        }
    }

    /// <summary>
    /// 表示一个 Moebooru 所使用的Post
    /// </summary>
    public class MoePost
    {
        public long id { get; set; }
        public string tags { get; set; }
        public long? creator_id { get; set; }
        public String author { get; set; }
        public long change { get; set; }
        public String source { get; set; }
        public long score { get; set; }
        public String md5 { get; set; }
        public long file_size { get; set; }
        public String file_url { get; set; }
        public bool is_shown_in_index { get; set; }
        public String preview_url { get; set; }
        public int preview_width { get; set; }
        public int preview_height { get; set; }
        public int actual_preview_width { get; set; }
        public int actual_preview_height { get; set; }
        public String sample_url { get; set; }
        public int sample_width { get; set; }
        public int sample_height { get; set; }
        public long sample_file_size { get; set; }
        public String jpeg_url { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
        public long jpeg_file_size { get; set; }
        public string rating { get; set; }
        public bool has_children { get; set; }
        public String parent_id { get; set; }
        public String status { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool is_held { get; set; }
    }
}
