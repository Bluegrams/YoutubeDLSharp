using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WpfDemoApp.Views;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace WpfDemoApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private YoutubeDL _youtubeDL = new();
        private IProgress<DownloadProgress> _progress;
        private IProgress<string> _output;

        [ObservableProperty]
        private bool _audioOnly = false;

        [ObservableProperty]
        private string _url = "";

        [ObservableProperty]
        private string _options = "";

        [ObservableProperty]
        private string _state = "";

        [ObservableProperty]
        private float _progressValue = 0;

        [ObservableProperty]
        private string _progressText = "";

        [ObservableProperty]
        private string _outputText = "";

        public string Version => _youtubeDL.Version;

        public MainWindowViewModel()
        {
            _progress = new Progress<DownloadProgress>(ShowDownloadProgress);
            _output = new Progress<string>(ShowDownloadOutput);
        }

        [RelayCommand]
        private async Task Download()
        {
            await YoutubeDLSharp.Utils.DownloadBinaries();

            RunResult<string> result;

            var options = Options.Split('\n');
            var custom = OptionSet.FromString(options);
            // it seems like yt-dlp doesn't output progress bar by default
            custom.Progress = true;

            OutputText = "";
            if (AudioOnly)
            {
                result = await _youtubeDL.RunAudioDownload(Url, AudioConversionFormat.Mp3, progress: _progress, output: _output, overrideOptions: custom);
            }
            else
            {
                result = await _youtubeDL.RunVideoDownload(Url, progress: _progress, output: _output, overrideOptions: custom);
            }

            if (result.Success)
            {
                MessageBox.Show($"Successfully downloaded \"{Url}\" to:\n\"{result.Data}\".", "YoutubeDLSharp");
            }
            else
            {
                ShowErrorMessage(Url, string.Join("\n", result.ErrorOutput));
            }
        }

        [RelayCommand]
        private async Task FetchInfo()
        {
            await YoutubeDLSharp.Utils.DownloadBinaries();

            var result = await _youtubeDL.RunVideoDataFetch(Url);
            if (result.Success)
            {
                // doesn't follow MVVM pattern but project simple enough to use this trick
                var informationWindow = new InformationWindow(result.Data);
                informationWindow.ShowDialog();
            }
            else ShowErrorMessage(Url, string.Join("\n", result.ErrorOutput));
        }

        private void ShowDownloadProgress(DownloadProgress progress)
        {
            State = progress.State.ToString();
            ProgressValue = progress.Progress;
            ProgressText = $"speed: {progress.DownloadSpeed} | left: {progress.ETA}";
        }

        private void ShowDownloadOutput(string str)
        {
            if (str.Contains("[download]")) return;

            OutputText += (str + Environment.NewLine);
        }

        private void ShowErrorMessage(string url, string error)
            => MessageBox.Show($"Failed to process '{url}'. Output:\n\n{error}", "YoutubeDLSharp - ERROR",
                MessageBoxButton.OK, MessageBoxImage.Error);
    }
}