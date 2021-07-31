namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TaxonomyValues
    {
        /// <summary>
        /// Unique ID of the taxonomy. If this taxonomy should be linked to the AskDelphi system, this should be a topic Guid of a Hierarchy-type topic in AskDelphi
        /// </summary>
        public string TaxonomyId { get; set; }

        /// <summary>
        /// The name of the taxonomy that the nodes belong to.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of tag-values representing nodes in the taxonomy to which the item is tagged.
        /// </summary>
        public TagValue[] Values { get; set; }
    }
}