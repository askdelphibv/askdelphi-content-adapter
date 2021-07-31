using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ContentSearchRequest
    {
        public string FolderId { get; set; }
        public string Query { get; set; }
        public List<string> TopicTypes { get; set; }
        public List<TaxonomyValues> Tags { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string ContinuationToken { get; set; }

    }
}
