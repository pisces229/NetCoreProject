using System;

namespace NetCoreProject.Backend.Csp
{
    public class CspOptions
    {
        public bool ReadOnly { get; set; }
        public CspDirective DefaultSrc { get; set; } = new CspDirective("default-src");
        public CspDirective ConnectSrc { get; set; } = new CspDirective("connect-src");
        public CspDirective FontSrc { get; set; } = new CspDirective("font-src");
        public CspDirective ChildSrc { get; set; } = new CspDirective("child-src");
        public CspDirective FrameSrc { get; set; } = new CspDirective("frame-src");
        public CspDirective ImageSrc { get; set; } = new CspDirective("img-src");
        public CspDirective MediaSrc { get; set; } = new CspDirective("media-src");
        public CspDirective ObjectSrc { get; set; } = new CspDirective("object-src");
        public CspDirective ScriptSrc { get; set; } = new CspDirective("script-src");
        public CspDirective StyleSrc { get; set; } = new CspDirective("style-src");
        public CspDirective FrameAncestors { get; set; } = new CspDirective("frame-ancestors");
        public string ReportURL { get; set; }
    }
}
