using System.Windows;
using YoutubeDLSharp.Metadata;

namespace WpfDemoApp.Views
{
    public partial class InformationWindow : Window
    {
        public InformationWindow(VideoData videoData)
        {
            InitializeComponent();
            DataContext = videoData;
        }
    }
}