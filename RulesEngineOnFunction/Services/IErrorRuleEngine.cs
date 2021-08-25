using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RulesEngineOnFunction.Models;

namespace RulesEngineOnFunction.Services
{
    /// <summary>
    /// ErrorRuleEngine Interface.
    /// </summary>
    public interface IErrorRuleEngine
    {
        /// <summary>
        /// Initialize RulesEngine.
        /// </summary>
        /// <returns>Task.</returns>
        Task Init();

        /// <summary>
        /// ExecuteRules of RulesEngine.
        /// </summary>
        /// <param name="inputRule">Input telemetry data.</param>
        /// <param name="logger">Logger.</param>
        /// <returns>List of RuleName.</returns>
        Task<List<string>> ExecuteRulesAsync(InputRule inputRule, ILogger logger);

        /// <summary>
        /// Update a RulesEngine Workflow by workflow id.
        /// </summary>
        /// <param name="id">Workflow id.</param>
        /// <returns>Task.</returns>
        public Task UpdateRuleAsync(string id);
    }
}