using System;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TopicContent
    {
        public Guid Guid { get; set; }

        public TopicContentBasicData BasicData { get; set; }

        public TopicContentRelationData Relations { get; set; }

        public string TopicId { get; set; }

        public string Content { get; set; }
    }
}