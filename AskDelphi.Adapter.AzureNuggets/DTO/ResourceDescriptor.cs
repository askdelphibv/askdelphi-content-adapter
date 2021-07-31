namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ResourceDescriptor
    {
        /// <summary>
        /// Should provide enough information for the implementing system to uniquely identify the resource.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// The display name of the file, as it’s shown to the user.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// ISO8601 compatible timestamp in the form YYYY-MM-DDThh:mm:ssZ
        /// </summary>
        public string LastModified { get; set; }

        /// <summary>
        /// Mime type identification
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Total length of the resource.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// A short string identifying the status of this document. This is shown in the AskDelphi authoring environment as additional information to help the user select the resource they want.
        /// </summary>
        public string Status { get; set; }
    }
}