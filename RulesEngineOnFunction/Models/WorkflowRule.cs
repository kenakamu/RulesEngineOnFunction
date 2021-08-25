using System.Collections.Generic;
using Newtonsoft.Json;
using RulesEngine.Models;

namespace RulesEngineOnFunction.Models
{
    /// <summary>
    /// RulesEngine WorkflowRule Model.
    /// </summary>
    public class WorkflowRule
    {
        /// <summary>
        /// Gets or sets WorkflowName.
        /// </summary>
        [JsonProperty("workflowName")]
        public string WorkflowName { get; set; }

        /// <summary>
        /// Gets or sets Rules.
        /// </summary>
        [JsonProperty("rules")]
        public List<Rule> Rules { get; set; }
    }
}
