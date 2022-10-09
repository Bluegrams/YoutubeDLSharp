using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace WpfDemoApp
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _isNotDownloading = true;
        private bool _audioOnly;

        private readonly IProgress<DownloadProgress> _progress;
        private readonly IProgress<string> _output;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            _progress = new Progress<DownloadProgress>(ShowProgress);
            _output = new Progress<string>(s => TxtOutput.AppendText(s + Environment.NewLine));
        }

        private YoutubeDl YoutubeDl { get; } = new() { YoutubeDlPath = "yt-dlp" };

        public bool IsNotDownloading
        {
            get => _isNotDownloading;
            set
            {
                _isNotDownloading = value;
                PropertyChangedVoid();
            }
        }

        public bool AudioOnly
        {
            get => _audioOnly;
            set
            {
                _audioOnly = value;
                PropertyChangedVoid();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropertyChangedVoid([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var url = TxtUrl.Text;
            IsNotDownloading = false;
            TxtOutput.Clear();
            // Parse custom arguments
            var custom = OptionSet.FromString(TxtOptions.Text.Split('\n'));
            RunResult<string> result;
            if (AudioOnly)
            {
                result = await YoutubeDl.RunAudioDownload(
                    url, AudioConversionFormat.Mp3, progress: _progress,
                    output: _output, overrideOptions: custom
                );
            }
            else
                result = await YoutubeDl.RunVideoDownload(url, progress: _progress, output: _output,
                    overrideOptions: custom);

            if (result.Success)
                MessageBox.Show($"Successfully downloaded \"{url}\" to:\n\"{result.Data}\".", "YoutubeDLSharp");
            else ShowErrorMessage(url, String.Join("\n", result.ErrorOutput));
            IsNotDownloading = true;
        }

        private void ShowProgress(DownloadProgress p)
        {
            TxtState.Text = p.State.ToString();
            ProgDownload.Value = p.Progress;
            TxtProgress.Text = $"speed: {p.DownloadSpeed} | left: {p.Eta}";
        }

        private async void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            var url = TxtUrl.Text;
            var result = await YoutubeDl.RunVideoDataFetch(url);
            if (result.Success)
            {
                var infoWindow = new InformationWindow(result.Data);
                infoWindow.ShowDialog();
            }
            else ShowErrorMessage(url, string.Join("\n", result.ErrorOutput));
        }

        private void ShowErrorMessage(string url, string error)
            => MessageBox.Show($"Failed to process '{url}'. Output:\n\n{error}", "YoutubeDLSharp - ERROR",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
