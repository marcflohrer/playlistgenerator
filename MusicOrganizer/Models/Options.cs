using PowerArgs;

namespace MusicOrganizer.Models
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class Options
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgShortcut("m"), ArgRequired(PromptIfMissing = true), ArgExistingDirectory]
        public string? MusicDirectory { get; internal set; }

        [ArgShortcut("c"), ArgRequired(PromptIfMissing = true), ArgExistingDirectory]
        public string? CsvPlaylistDirectory { get; internal set; }
    }
}
