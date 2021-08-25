using System.Collections.Generic;
using Newtonsoft.Json;

namespace RulesEngineOnFunction.Models
{
    /// <summary>
    /// InputRule Model for ErrorJudge.
    /// </summary>
    public class InputRule
    {
        /// <summary>
        /// Gets or sets WorkflowName.
        /// </summary>
        [JsonProperty("workflowName")]
        public string WorkflowName { get; set; }

        /// <summary>
        /// Gets or sets BasicInfo.
        /// </summary>
        [JsonProperty("basicInfo")]
        public Dictionary<string, object> BasicInfo { get; set; }

        /// <summary>
        /// Gets or sets OrderInfo.
        /// </summary>
        [JsonProperty("orderInfo")]
        public Dictionary<string, object> OrderInfo { get; set; }

        /// <summary>
        /// Gets or sets TelemetryInfo.
        /// </summary>
        [JsonProperty("telemetryInfo")]
        public Dictionary<string, object> TelemetryInfo { get; set; }
    }
}
