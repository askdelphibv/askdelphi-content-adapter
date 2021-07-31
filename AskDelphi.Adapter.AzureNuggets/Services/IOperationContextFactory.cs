using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    public interface IOperationContextFactory
    {
        IOperationContext CreateOperationContext(HttpContext context);
        IOperationContext CreateBackgroundOperationContext();
    }
}
