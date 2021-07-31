using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ClaimTuple
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public ClaimTuple() { }

        public ClaimTuple(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"ClaimTuple(\"{Type}\",\"{Value}\")";
        }
    }
}
