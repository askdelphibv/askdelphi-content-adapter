using System;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    /// <summary>
    /// 
    /// </summary>
    public class Caption
    {
        /// <summary>
        /// The blobstore unique identifier
        /// </summary>
        public Guid BlobstoreGuid;

        /// <summary>
        /// The label
        /// </summary>
        public string Label;

        /// <summary>
        /// The language
        /// </summary>
        public string Language;

        /// <summary>
        /// The filename
        /// </summary>
        public string Filename;

        /// <summary>
        /// The local resource file path
        /// </summary>
        public string LocalResourceFilePath;

        /// <summary>
        /// Set to true to indicate the caption is externally hosted
        /// </summary>
        public bool ResourceIsExternal { get; set; }

        /// <summary>
        /// If externally hosted should contain the full remote resource URI for the caption file
        /// </summary>
        public string ResourceExternalContentUri { get; set; }
    }
}