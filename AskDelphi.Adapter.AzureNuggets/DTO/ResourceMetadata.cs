using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ResourceMetadata
    {
        /// <summary>
        /// A plain-text description of the resource’s contents.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Identifies the taxonomies and nodes to which the resource is tagged. It’s possible to link these nodes and taxonomies to AskDelphi-defined resources, provided the correct identifiers are used.
        /// </summary>
        public TaxonomyValues[] Tags { get; set; }

        /// <summary>
        /// A plain text representation of the resource. Used for indexing purposes only, never displayed to the end-user.
        /// </summary>
        public string Content { get; set; }
    }
}
