using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace HLib.Booru.Controls.ViewModel
{
    public class PostViewModel : ViewModelBase
    {
        public PostViewModel() : base()
        {
            post = new BasePost()
            {
                Id = 311605,
                PreviewImageUrl = new Uri("https://assets.yande.re/data/preview/c6/7b/c67bf7b605316e9d161c5da3f6d6f347.jpg"),
                PreviewImageHeight = 300,
                PreviewImageWidth = 246,
                SampleImageUrl = new Uri("https://files.yande.re/sample/c67bf7b605316e9d161c5da3f6d6f347/yande.re%20311605%20sample%20animal_ears%20bra%20cleavage%20hatsune_miku%20nekomimi%20see_through%20sen_ya%20tail%20vocaloid.jpg"),
                ImageUrl = new Uri("https://files.yande.re/image/c67bf7b605316e9d161c5da3f6d6f347/yande.re%20311605%20animal_ears%20bra%20cleavage%20hatsune_miku%20nekomimi%20see_through%20sen_ya%20tail%20vocaloid.jpg")
            };
        }

        public PostViewModel(BasePost post) :base ()
        {
            Post = post;
        }

        private BasePost post;

        public BasePost Post
        {
            get
            {
                return post;
            }

            set
            {
                Set<BasePost>(ref post, value);
            }
        }

        
    }
}
