namespace AskDelphi.Adapter.AzureNuggets.Services.Cache
{
    public abstract class CacheKey
    {
        /// <summary>
        /// Identifies the region in the cache where the data is to be stored. Is considered part of the key, and may be used bu the implementation to select the appropriate cache region to store the data in.
        /// </summary>
        public abstract string Region { get; }

        /// <summary>
        /// Returns a string that's unique for this cache key instance.
        /// </summary>
        /// <returns></returns>
        public virtual string AsString() => Base64Encode(Newtonsoft.Json.JsonConvert.SerializeObject(this));

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}