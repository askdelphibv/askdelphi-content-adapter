namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class FolderDescriptor
    {
        /// <summary>
        /// Should provide enough information for the implementing system to uniquely identify the folder.
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// The display name of the folder, as it's shown to the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ISO8601 compatible timestamp in the form YYYY-MM-DDThh:mm:ssZ
        /// </summary>
        public string LastModified { get; set; }
    }
}