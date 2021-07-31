using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public interface IAPIResponse
    {
        bool Success { get; }
        string Version { get; }
        string Id { get; }
    }
}
