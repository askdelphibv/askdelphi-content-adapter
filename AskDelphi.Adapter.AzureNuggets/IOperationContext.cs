using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets
{
    public interface IOperationContext : IDisposable
    {
        string OperationId { get; }

        string GetAuthToken();
        string GetAskDelphiSystemID();
        void InitializeFromRequest(HttpRequest request);
    }
}
