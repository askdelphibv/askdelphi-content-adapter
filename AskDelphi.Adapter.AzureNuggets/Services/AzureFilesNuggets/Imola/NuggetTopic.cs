using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    public class NuggetTopic : TopicContentBase
    {

        /// <summary>
        /// Gets or sets the video topic guid.
        /// </summary>
        public Guid VideoTopicGuid { get; set; }

        /// <summary>
        /// HTML text field for the Nugget description
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tip body HTML.
        /// </summary>
        public string TipHTML { get; set; }

        /// <summary>
        /// Gets or sets the tip image topic unique identifier.
        /// </summary>
        public Guid TipImageTopicGuid { get; set; }

        /// <summary>
        /// Gets or sets the steps.
        /// </summary>
        public List<NuggetStep> Steps { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Indicates whether the topic body is empty
        /// </summary>
        public bool IsBodyEmpty { get; set; }
    }
    }
