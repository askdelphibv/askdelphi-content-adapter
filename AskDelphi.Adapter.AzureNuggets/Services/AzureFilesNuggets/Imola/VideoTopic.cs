using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    public class VideoTopic : TopicContentBase
    {
        /// <summary>
        /// Gets or sets the body HTML.
        /// </summary>
        public string BodyHTML { get; set; }

        /// <summary>
        /// Indicates whether the topic body is empty
        /// </summary>
        public bool IsBodyEmpty { get; set; }

        /// <summary>
        /// Gets or sets the embed HTML.
        /// </summary>
        public string EmbedCodeHTML { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a local stored file or an embed code html fragment.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is strored; otherwise, <c>false</c>.
        /// </value>
        public bool IsStored { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable automatic play].
        /// </summary>
        public bool EnableAutoPlay { get; set; }

        /// <summary>
        /// Gets or sets the text only browser description.
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail topic unique identifier.
        /// </summary>
        public Guid ThumbnailTopicGuid { get; set; }

        /// <summary>
        /// Gets or sets the captions.
        /// </summary>
        public List<Caption> Captions { get; set; }
        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// The path of the local resource file.
        /// </summary>
        public string LocalResourceFilePath { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the document.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the content type of the document
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the alternative text of the document
        /// </summary>
        public string AlternativeText { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio of the document
        /// </summary>
        public string AspectRatio { get; set; }
    }
}
