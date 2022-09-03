# YoutubeDLSharp

**A Modified Version of Bluegrams/YoutubeDLSharp**

A simple .NET wrapper library for [youtube-dl](https://github.com/ytdl-org/youtube-dl).

## adanvdo vs bluegrams - what's the difference?

This repo has been modified based on use with [yt-dlp](https://github.com/yt-dlp/yt-dlp)  
99% of the code is identical to bluegrams, with the following differences.

- Target Framework Updated to .NET 4.8
- Added Ffmpeg wrapper [Xabe.FFmpeg](https://github.com/tomaszzmuda/Xabe.FFmpeg) 
- Added Progress Reporting Support to `RunVideoDataFetch` and `RunWithOptions`
- Added a Second `RunWithOptions` that returns a `RunResult<string>` object instead of `RunResult<string[]>`
- Added fallback to ffmpeg for formats missing valid metadata

## What is it?

YoutubeDLSharp is a wrapper for the popular command-line video downloader youtube-dl.
It allows you to use the extensive features of youtube-dl in a .NET project.
For more about the features of youtube-dl, supported websites and anything else, visit its project page at http://ytdl-org.github.io/youtube-dl/.

## How do I use it?


To install - check [Releases](https://github.com/adanvdo/YoutubeDLSharp/releases) for package files and add YoutubeDLSharp.DLL and Xabe.Ffmpeg.DLL to your project references
To avoid conflicts with bluegrams, this project will not be published to nuget

Now, there are two ways to use YoutubeDLSharp: the class `YoutubeDL` provides high level methods for downloading and converting videos
while the class `YoutubeDLProcess` allows directer and flexibler access to the youtube-dl process.

### Convenient Approach

In the simplest case, initializing the downloader and downloading a video can be achieved like this:

```csharp
var ytdl = new YoutubeDL();
// set the path of the youtube-dl and FFmpeg if they're not in PATH or current directory
ytdl.YoutubeDLPath = "path\\to\\youtube-dl.exe";
ytdl.FFmpegPath = "path\\to\\ffmpeg.exe";
// optional: set a different download folder
ytdl.OutputFolder = "some\\directory\\for\\video\\downloads";
// download a video
var res = await ytdl.RunVideoDownload("https://www.youtube.com/watch?v=_QdPW8JrYzQ");
// the path of the downloaded file
string path = res.Data;
```

Instead of only downloading a video, you can also directly extract the audio track ...

```csharp
var res = await ytdl.RunAudioDownload(
    "https://www.youtube.com/watch?v=QUQsqBqxoR4",
    AudioConversionFormat.Mp3
);
```

... or selectively download videos from a playlist:

```csharp
var res = await ytdl.RunVideoPlaylistDownload(
    "https://www.youtube.com/playlist?list=PLPfak9ofGSn9sWgKrHrXrxQXXxwhCblaT",
    start: 52, end: 76
);
```

All of the above methods also allow you to track the download progress or cancel an ongoing download:

```csharp
// a progress handler with a callback that updates a progress bar
var progress = new Progress<DownloadProgress>(p => progressBar.Value = p.Progress);
// a cancellation token source used for cancelling the download
// use `cts.Cancel();` to perform cancellation
var cts = new CancellationTokenSource();
// ...
await ytdl.RunVideoDownload("https://www.youtube.com/watch?v=_QdPW8JrYzQ",
                            progress: progress, ct: cts.Token);
```

As youtube-dl also allows you to extract extensive metadata for videos, you can also fetch these (without downloading the video):

```csharp
var res = await ytdl.RunVideoDataFetch("https://www.youtube.com/watch?v=_QdPW8JrYzQ");
// get some video information
VideoData video = res.Data;
string title = video.Title;
string uploader = video.Uploader;
long? views = video.ViewCount;
// all available download formats
FormatData[] formats = video.Formats;
// ...
```

Sometimes youtube-dl / yt-dlp can return incomplete metadata.  You can set `RunVideoDataFetch` to fall-back to ffmpeg to obtain missing metadata information.

```csharp
/// <param name="url">The video URL passed to youtube-dl.</param>
/// <param name="ct">A CancellationToken used to cancel the process.</param>
/// <param name="flat">If set to true, does not extract information for each video in a playlist.</param>
/// <param name="overrideOptions">Override options of the default option set for this run.</param>
/// <param name="progress">A progress provider used to get download progress information.</param>
/// <param name="output">A progress provider used to capture the standard output.</param>
/// <param name="useFfmpegMetaDataFallback">If set to true, Ffmpeg will be used to fetch missing metadata values</param>
/// <returns>A RunResult object containing a VideoData object with the requested video information.</returns>
var res = await ytdl.RunVideoDataFetch("https://www.youtube.com/watch?v=_QdPW8JrYzQ", default, true, null, null, null, true);
// get some video information
VideoData video = res.Data;
string title = video.Title;
string uploader = video.Uploader;
long? views = video.ViewCount;
// all available download formats
FormatData[] formats = video.Formats;
// ...
```

This intro does not show all available options. Refer to the method documentations for more.

The project includes a demo WPF desktop app under [WpfDemoApp](WpfDemoApp/MainWindow.xaml.cs) that uses the `YoutubeDL` class.

### Advanced Usage

#### Working with options

YoutubeDLSharp uses the `OptionSet` class to model youtube-dl options.
The names of the option properties correspond to the names of youtube-dl, so defining a set of options can look like this:

```csharp
var options = new OptionSet()
{
    NoContinue = true,
    RestrictFilenames = true,
    Format = "best",
    RecodeVideo = VideoRecodeFormat.Mp4,
    Exec = "echo {}"
}
```

For documentation of all options supported by youtube-dl and their effects, visit https://github.com/ytdl-org/youtube-dl#options.

Additionally, YoutubeDLSharp allows you to pass **custom options** to the downloader program.
This is especially useful when a forked/ modified version of youtube-dl is used.
Custom can be specified like this:

```csharp
// add
options.AddCustomOption<string>("--my-custom-option", "value");
// set
options.SetCustomOption<string>("--my-custom-option", "new value");
```

#### `YoutubeDLProcess`

To run a youtube-dl process with the defined options, you can use the `YoutubeDLProcess` class:

```csharp
var ytdlProc = new YoutubeDLProcess();
// capture the standard output and error output
ytdlProc.OutputReceived += (o, e) => Console.WriteLine(e.Data);
ytdlProc.ErrorReceived += (o, e) => Console.WriteLine("ERROR: " + e.Data);
// start running
string[] urls = new[] { "https://github.com/ytdl-org/youtube-dl#options" };
await ytdlProc.RunAsync(urls, options);
```

#### Loading/ Saving configuration

You can persist a youtube-dl configuration to a file and reload it:

```csharp
// Save to file
var saveOptions = new OptionSet();
saveOptions.WriteConfigFile("path\\to\\file");

// Reload configuration
OptionSet loadOptions = OptionSet.LoadConfigFile("path\\to\\file");
```

The file format is compatible with the format used by youtube-dl itself.
For more, read https://github.com/ytdl-org/youtube-dl#configuration.

## Issues & Contributing

You are very welcome to contribute by reporting issues, fixing bugs or resolving inconsistencies to youtube-dl.
If you want to contribute a new feature to the library, please open an issue with your suggestion before starting to implement it.

All issues related to downloading specific videos, support for websites or downloading/ conversion features should better be reported to https://github.com/ytdl-org/youtube-dl/issues.

## License

This project is licensed under [BSD-3-Clause license](LICENSE.txt).
