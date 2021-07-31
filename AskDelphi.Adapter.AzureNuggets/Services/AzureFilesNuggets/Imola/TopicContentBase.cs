using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    /// <summary>
    /// Topic Content Base
    /// </summary>
    public abstract class TopicContentBase
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title markup.
        /// </summary>
        public string TitleMarkup { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the topic guid.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// The topic type
        /// </summary>
        public string TopicType { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public TopicVersion Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is published.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets the topic learning content for this topic
        /// </summary>
        public TopicLearning TopicLearning { get; set; }
    }
}
