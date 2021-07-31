using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class Nugget
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Uuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Application_ID { get; set; }
        public string Application { get; set; }
        public string Version { get; set; }
        public List<NuggetTextAndImage> Keywords { get; set; }
        public List<NuggetStep> Steps { get; set; }
        public NuggetVideo Video { get; set; }
        public List<NuggetRelation> Relations { get; set; }
        public string ContextTags { get; set; }
        public NuggetTextAndImage Tip { get; set; }

        public class NuggetTextAndImage
        {
            public string Text { get; set; }
            public string Href { get; set; }
        }

        public class NuggetStep
        {
            public string Href { get; set; }
            public string Alt { get; set; }
            public string Text { get; set; }
            public string Clickinstruction { get; set; }
            // What is DirectInstructions for type
        }

        public class NuggetVideo
        {
            public string Poster { get; set; }
            public List<VideoSource> Sources { get; set; }
            public List<VideoTrack> Tracks { get; set; }
        }

        public class VideoTrack
        {
            public string Href { get; set; }
            public string Language { get; set; }
            public string Label { get; set; }
        }

        public class VideoSource
        {
            public string Href { get; set; }
            public string Type { get; set; }
        }

        public class NuggetRelation
        {
            // TODO
        }
    }
}
