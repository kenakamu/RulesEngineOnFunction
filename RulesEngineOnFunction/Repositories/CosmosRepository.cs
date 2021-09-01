using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RulesEngineOnFunction.Models;

namespace RulesEngineOnFunction.Repositories
{
    /// <summary>
    /// Repository implementation.
    /// </summary>
    public class CosmosRepository : IRepository
    {
        private CosmosClient client;
        private Container workflowContainer;
        private string continuationToken = default(string);

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosRepository"/> class.
        /// </summary>
        /// <param name="client">Cosmos DB SDK Client.</param>
        /// <param name="databaseId">Cosmos DB DatabaseId.</param>
        /// <param name="containerId">Cosmos DB ContainerId.</param>
        public CosmosRepository(CosmosClient client, string databaseId, string containerId)
        {
            this.client = client;
            this.workflowContainer = client.GetContainer(databaseId, containerId);
        }

        /// <summary>
        /// Get changed item since specified time.
        /// </summary>
        /// <returns>List of workflows.</returns>
        public async Task<List<dynamic>> GetChangedWorkflowsAsync()
        {
            FeedIterator<Workflow> changedFeedIterator = this.workflowContainer.GetChangeFeedIterator<Workflow>(
                this.continuationToken == null ? ChangeFeedStartFrom.Beginning() : ChangeFeedStartFrom.ContinuationToken(this.continuationToken),
                ChangeFeedMode.Incremental);
            List<dynamic> results = new ();
            while (changedFeedIterator.HasMoreResults)
            {
                FeedResponse<Workflow> response = await changedFeedIterator.ReadNextAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    this.continuationToken = response.ContinuationToken;
                    break;
                }
                else
                {
                    foreach (Workflow workflow in response)
                    {
                        results.Add(workflow);
                    }
                }
            }

            return results.Select(x => x.Rule).ToList();
        }
    }
}