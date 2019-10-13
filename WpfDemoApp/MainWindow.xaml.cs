using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using YoutubeDLSharp.Options;

namespace WpfDemoApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isNotDownloading = true;
        private bool audioOnly = false;

        private IProgress<DownloadProgress> progress;

        public MainWindow()
        {
            this.YoutubeDL = new YoutubeDL();
            this.DataContext = this;
            InitializeComponent();
            progress = new Progress<DownloadProgress>((p) => progDownload.Value = p.Progress);
        }

        public YoutubeDL YoutubeDL { get; }

        public bool IsNotDownloading
        {
            get => isNotDownloading;
            set
            {
                isNotDownloading = value;
                propertyChanged();
            }
        }

        public bool AudioOnly
        {
            get => audioOnly;
            set
            {
                audioOnly = value;
                propertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void propertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text;
            RunResult<string> result;
            IsNotDownloading = false;
            if (AudioOnly)
            {
               result = await YoutubeDL.RunAudioDownload(url, AudioConversionFormat.Mp3,progress: progress);
            }
            else
            {
                result = await YoutubeDL.RunVideoDownload(url, progress: progress);
            }
            if (result.Success)
            {
                MessageBox.Show($"Successfully downloaded \"{url}\" to:\n\"{result.Data}\".", "YoutubeDLSharp");
            }
            else showErrorMessage(url, String.Join("\n", result.ErrorOutput));
            IsNotDownloading = true;
        }

        private async void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text;
            RunResult<VideoData> result = await YoutubeDL.RunVideoDataFetch(url);
            if (result.Success)
            {
                var infoWindow = new InformationWindow(result.Data);
                infoWindow.ShowDialog();
            }
            else showErrorMessage(url, String.Join("\n", result.ErrorOutput));
        }

        private void showErrorMessage(string url, string error)
            => MessageBox.Show($"Failed to process '{url}'. Output:\n\n{error}", "YoutubeDLSharp - ERROR",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
