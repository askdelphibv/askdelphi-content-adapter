namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TopicContentBasicData
    {
        public string TopicTitle { get; set; }
        public string TopicTitleMarkup { get; set; }
        public string Description { get; set; }
        public string ModificationDate { get; set; }
        public string Version { get; set; }
        public string TopicType { get; set; }
        public string[] MetricsTags { get; set; }
        public bool Enabled { get; set; }
        public bool IsPublished { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsDescriptionCalculated { get; set; }
        public string Namespace { get; set; }
    }
}