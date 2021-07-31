namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TopicMetadata
    {
        public string Description { get; set; }
        public TaxonomyValues[] Tags { get; set; }
        public string IndexContents { get; set; }
    }
}