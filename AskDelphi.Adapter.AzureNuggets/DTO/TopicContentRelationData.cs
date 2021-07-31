using System;
using System.Collections.Generic;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TopicContentRelationData
    {
        public string KeyVisualGuid { get; set; }
        public string keyVisualVisualization { get; set; }
        public string ThumbnailTopicGuid { get; set; }
        public List<TopicContentRelationReference> References { get; set; }
    }
}