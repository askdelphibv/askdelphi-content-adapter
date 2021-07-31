namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class TagValue
    {
        /// <summary>
        /// An identifier of the tag. If specified must match the identifier of a node in a taxonomy as defined in the AskDelphi authoring environment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display-name for the node.
        /// </summary>
        public string Name { get; set; }
    }
}