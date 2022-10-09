using YoutubeDLSharp.Metadata;

namespace WpfDemoApp
{
    public partial class InformationWindow
    {
        public InformationWindow(VideoData videoData)
        {
            DataContext = videoData;
            InitializeComponent();
        }
    }
}
