using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RulesEngineOnFunction.Models;
using RulesEngineOnFunction.Repositories;
using RulesEngineOnFunction.Services;

namespace RulesEngineOnFunction
{
    /// <summary>
    /// Function to judge if incoming telemetry has error or not.
    /// </summary>
    public class RulesEngineOnFunction
    {
        private readonly IErrorRuleEngine errorRuleEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesEngineOnFunction"/> class.
        /// </summary>
        /// <param name="errorRuleEngine">IErrorRuleEngine.</param>
        public RulesEngineOnFunction(IErrorRuleEngine errorRuleEngine)
        {
            this.errorRuleEngine = errorRuleEngine;
        }

        /// <summary>
        /// Execute RulesEngineOnFunction.
        /// </summary>
        /// <param name="req">incoming telemetry</param>
        /// <param name="executionContext">FunctionContext.</param>
        /// <returns>List of DetectedErrorRuleOutput.</returns>
        [Function("ExecuteRule")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(RulesEngineOnFunction));
            logger.LogInformation("C# HTTP trigger function processed a request.");

            StreamReader reader = new (req.Body);
            var body = reader.ReadToEnd();
            InputRule inputRule = JsonConvert.DeserializeObject<InputRule>(body);

            List<string> ruleNames = await this.errorRuleEngine.ExecuteRulesAsync(inputRule, logger).ConfigureAwait(false);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            if (ruleNames.Count > 0)
            {
                response.WriteString(JsonConvert.SerializeObject(ruleNames));
            }
            else
            {
                response.WriteString("No discount applied.");
            }

            return response;
        }

        /// <summary>
        /// Update RulesEngine workflow via Cosmos DB Change feed.
        /// </summary>
        /// <param name="input">Updated/Created RulesEngine workflow.</param>
        /// <param name="context">FunctionContext.</param>
        /// <returns>Task.</returns>
        [Function("UpdateRule")]
        public async Task ChangeFeed(
            [CosmosDBTrigger(
            databaseName: "rulesDb",
            collectionName: "rulesEngineWorkflows",
            ConnectionStringSetting = "CosmosDBConnectionString")] IReadOnlyList<CosmosEntity> input,
            FunctionContext context)
        {
            var logger = context.GetLogger(nameof(RulesEngineOnFunction));
            if (input != null && input.Count > 0)
            {
                logger.LogInformation("Documents modified: " + input.Count);
                foreach (CosmosEntity cosmosEntity in input)
                {
                    await this.errorRuleEngine.UpdateRuleAsync(cosmosEntity.Id);
                }
            }
        }
    }
}
