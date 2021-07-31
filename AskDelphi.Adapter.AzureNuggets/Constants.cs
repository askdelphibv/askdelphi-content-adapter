using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets
{
    public static class Constants
    {
        public const string APIVersion1 = "1";
        public const bool Success = true;
        public const bool Fail = false;

        public const string ClaimTypeSystemID = "http://tempuri.org/askdelphi/remote-system-id";

        public const string ErrorFailedCode = "E_FAIL";
        public const string ErrorFailedMessage = "The operation failed to complete";

        public const string ErrorInvalidCredentialsCode = "E_PERM";
        public const string ErrorInvalidCredentialsMessage = "Invalid credentials";

        public const string ErrorNotSupportedCode = "E_SUPP";
        public const string ErrorNotSupportedMessage = "Operation is not supported";
    }
}
