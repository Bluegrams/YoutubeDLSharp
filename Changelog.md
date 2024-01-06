# YoutubeDLSharp Changelog

### v.1.1.0 (2024-01)
- **New**/ **Changed:** Include new and changed yt-dlp options (version 2023-12-30).
- **New:** Add support for option string aliases (e.g. for parsing).
- **New:** Specify `PythonInterpreterPath` in `YoutubeDL` class.
- **Fixed:** Catch json deserialization errors in video data fetch.
- **Fixed:** `HasDRM` attribute in `FormatData`.
- **Fixed:** Passing of video IDs with hyphens.

### v.1.0.0 (2023-07)
- **New:** Use yt-dlp instead of youtube-dl as default downloader.
- **New:** Methods for automatically downloading yt-dlp and FFmpeg.
- **New:** Addition of options, video & format data attributes for yt-dlp.
- **New:** `MultiOption` & `MultiValue` classes for options that can be set multiple times.
- **New:** Add .NET 6 as build target.
- **New:** Support extraction of chapter & comment information.
- **New:** Addition of various missing video & format data attributes (e.g. episode, album, track information).
- **New:** Additional `RunWithOptions()` for single URL & with progress info.
- **Changed:** Better support for various video data attributes (e.g. time stamps, status enums, ...)
- **Changed:** Apply override options after setting other options in `YoutubeDL` methods.
- **Changed:** Changed default output template to be in line with the yt-dlp format.
- **Fixed:** Error when using executable path with spaces.

### v.0.4.3 (2022-11)
- **Fixed:** Windows issues with non-ASCII chars in output.

### v.0.4.2 (2022-03)
- **Fixed:** Starting youtube-dl process blocks main thread

### v.0.4.1 (2022-01)
- **New:** Methods for adding, modifying & deleting custom options in `OptionSet`
- **Fixed:** Download state reporting in `YoutubeDLProcess`

### v.0.4.0 (2021-10)
- **New:** Support custom options
- **Fixed:** `start_time` & `end_time` properties in metadata

### v.0.3.1 (2021-06)
- **Fixed:** `MetadataType` in `VideoData`
- **Changed:** Include new changes in youtube-dl options

### v.0.3 (2020-09)
- **New:** Override options in OptionSet & YoutubeDL class.
- **New:** More download progress information (transfer rate, ...)
- **Fixed:** Converting option set to & from string.

### v.0.2 (2020-03)
- **New:** Option to capture output in YoutubeDL class.
- **New:** Added PythonPath to YoutubeDLProcess.
- **New:** Load configuration from file or string.
- **Fixed:** FormatData.FrameRate.

### v.0.1 (2020-01)
- First released version.
