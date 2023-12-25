using PowerArgs;

namespace MusicOrganizer.Models
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class Options
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgShortcut("p"), ArgRequired(PromptIfMissing = true), ArgExistingDirectory]
        public string? MusicDirectory { get; internal set; }

        [ArgShortcut("l"), ArgRequired(PromptIfMissing = true), ArgExistingDirectory]
        public string? PlaylistDirectory { get; internal set; }

        [ArgShortcut("n"), ArgRequired(PromptIfMissing = true)]
        public string? PlaylistName { get; internal set; }

        [ArgShortcut("f"), ArgRequired(PromptIfMissing = true)]
        public string? ResumeFiles { get; internal set; }

        [ArgShortcut("t"), ArgRequired(PromptIfMissing = true)]
        public string? ResumeTags { get; internal set; }

        [ArgShortcut("o"), ArgRequired(PromptIfMissing = true)]
        public string? LogFile { get; internal set; }

        [ArgShortcut("d")]
        public string? DeletionScript { get; internal set; }

        [ArgShortcut("c")]
        public string? CsvInputPlaylistFile { get; internal set; }
    }
}

