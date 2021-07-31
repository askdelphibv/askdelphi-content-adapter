using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    public class ImageTopic : TopicContentBase
    {
        /// <summary>
        /// Gets or sets the local path of the small image. This image is at most 256 pixels wide.
        /// </summary>
        public string LocalPathSmall { get; set; }

        /// <summary>
        /// Gets or sets the local path of the medium image. This image is at most 512 pixels wide.
        /// </summary>
        public string LocalPathMedium { get; set; }

        /// <summary>
        /// Gets or sets the local path of the large image. This image is at most 768 pixels wide.
        /// </summary>
        public string LocalPathLarge { get; set; }

        /// <summary>
        /// Extra large
        /// </summary>
        public string LocalPathExtraLarge { get; set; }

        /// <summary>
        /// Gets or sets the local path of the raw image, as it was uploaded to the CMS.
        /// </summary>
        public string LocalPathRaw { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the raw image as it was registered upon upload to the CMS.
        /// </summary>
        public string MimeType { get; set; }
    }
}
