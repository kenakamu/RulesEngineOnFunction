using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RulesEngine.Models;
using RulesEngineOnFunction.Models;
using RulesEngineOnFunction.Repositories;

namespace RulesEngineOnFunction.Services
{
    /// <summary>
    /// ErrorRuleEngine implementation.
    /// </summary>
    public class ErrorRuleEngine : IErrorRuleEngine
    {
        private IRepository repository;
        private RulesEngine.RulesEngine ruleEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorRuleEngine"/> class.
        /// </summary>
        /// <param name="repository">IRepository</param>
        public ErrorRuleEngine(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Initializate RulesEngine.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task Init()
        {
            List<dynamic> workflows = await this.repository.GetChangedWorkflowsAsync().ConfigureAwait(false);
            dynamic workflowRules = JsonConvert.DeserializeObject<List<WorkflowRules>>(JsonConvert.SerializeObject(workflows));
            this.ruleEngine = new RulesEngine.RulesEngine(workflowRules.ToArray());
        }

        /// <summary>
        /// ExecuteRules of RulesEngine.
        /// </summary>
        /// <param name="inputRule">Input rule data.</param>
        /// <param name="logger">Logger.</param>
        /// <returns>List of RuleName.</returns>
        public async Task<List<string>> ExecuteRulesAsync(InputRule inputRule, ILogger logger)
        {
            ExpandoObjectConverter converter = new ();
            dynamic input1 = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(inputRule.BasicInfo), converter);
            dynamic input2 = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(inputRule.OrderInfo), converter);
            dynamic input3 = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(inputRule.TelemetryInfo), converter);
            var inputs = new dynamic[]
            {
                input1,
                input2,
                input3,
            };
            List<RuleResultTree> resultList = default(List<RuleResultTree>);

            try
            {
                if (!this.ruleEngine.GetAllRegisteredWorkflowNames().Contains(inputRule.WorkflowName))
                {
                    logger.LogInformation($"Could not find registered workflow with name '{inputRule.WorkflowName}' in the Rule Engine. ");
                    return default;
                }

                resultList = await this.ruleEngine.ExecuteAllRulesAsync(inputRule.WorkflowName, inputs).ConfigureAwait(false);
                List<string> failedRuleNames = resultList.Where(r => r.IsSuccess == true)
                    .Select(x => x.Rule.RuleName)
                    .ToList();

                return failedRuleNames;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update a RulesEngine Workflow by checking latest change.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task UpdateRulesAsync()
        {
            // Get the latest changes from database.
            List<dynamic> changedWorkflows = await this.repository.GetChangedWorkflowsAsync();

            foreach (dynamic workflow in changedWorkflows)
            {
                dynamic workflowRule = JsonConvert.DeserializeObject<WorkflowRules>(JsonConvert.SerializeObject(workflow));
                this.ruleEngine.RemoveWorkflow(new string[] { workflowRule.WorkflowName });
                this.ruleEngine.AddWorkflow(new WorkflowRules[] { workflowRule });
            }
        }
    }
}
