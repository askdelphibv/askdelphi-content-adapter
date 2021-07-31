using AskDelphi.Adapter.AzureNuggets.DTO;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(IOperationContext operationContext, string purpose, ClaimTuple[] claims);

        Task InvalidateToken(IOperationContext operationContext);

        Task<string> Refresh(IOperationContext operationContext, string refreshToken);

        Task<bool> ValidateToken(IOperationContext operationContext);
    }
}