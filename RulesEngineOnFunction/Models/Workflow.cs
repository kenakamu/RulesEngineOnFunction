using Newtonsoft.Json;

namespace RulesEngineOnFunction.Models
{
    /// <summary>
    /// RulesEngine Workflow Model.
    /// </summary>
    public class Workflow : CosmosEntity
    {
        /// <summary>
        /// Gets or sets Rule.
        /// </summary>
        [JsonProperty("rule")]
        public WorkflowRule Rule { get; set; }
    }
}
