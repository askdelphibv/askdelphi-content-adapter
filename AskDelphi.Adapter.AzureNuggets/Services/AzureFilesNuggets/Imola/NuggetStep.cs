using System;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola
{
    public class NuggetStep
    {
        /// <summary>
        /// Reference to image topic
        /// </summary>
        public Guid ImageGuid { get; set; }
        /// <summary>
        ///  Gets or sets the click intruction HTML.
        /// </summary>
        public string ClickInstructionHTML { get; set; }
        /// <summary>
        ///  Gets or sets the step text HTML.
        /// </summary>
        public string TextHTML { get; set; }
    }
}