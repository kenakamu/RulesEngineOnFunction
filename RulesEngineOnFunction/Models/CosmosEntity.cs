using Newtonsoft.Json;

namespace RulesEngineOnFunction.Models
{
    /// <summary>
    /// RulesEngine Workflow Model.
    /// </summary>
    public class CosmosEntity
    {
        /// <summary>
        /// Gets or sets Cosmos DB Id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
