using System;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TopicContentRelationReference
    {
        public KeyValuePair[] Metadata { get; set; }
        public int SequenceNumber { get; set; }
        public string View { get; set; }
        public string Use { get; set; }
        public string TargetTopicNamespaceUri { get; set; }
        public string TargetTopicTitle { get; set; }
        public Guid TargetTopicGuid { get; set; }
        public Guid RelationTypeKey { get; set; } // from content design topic type allowed relations????
    }
}