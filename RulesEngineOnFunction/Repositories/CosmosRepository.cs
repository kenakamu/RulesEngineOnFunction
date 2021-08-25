using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace RulesEngineOnFunction.Repositories
{
    /// <summary>
    /// Repository implementation.
    /// </summary>
    public class CosmosRepository : IRepository
    {
        private CosmosClient client;
        private Container workflowContainer;

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
        /// Get RulesEngine workflow by id.
        /// </summary>
        /// <param name="id">Cosmos DB Id.</param>
        /// <returns>Workflow contents.</returns>
        public async Task<dynamic> GetWorkflowByIdAsync(string id)
        {
            Console.WriteLine($"read cosmosdb to get {id}");
            QueryDefinition query = new QueryDefinition(
                "SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);

            FeedIterator<dynamic> policyIterator = this.workflowContainer.GetItemQueryIterator<dynamic>(query);

            FeedResponse<dynamic> response = await policyIterator.ReadNextAsync().ConfigureAwait(false);
            return response.Select(x => x.rule).FirstOrDefault();
        }

        /// <summary>
        /// Get all RulesEngine workflows.
        /// </summary>
        /// <returns>List of Workflow contents.</returns>
        public async Task<List<dynamic>> GetAllWorkflowsAsync()
        {
            List<dynamic> policies = new ();
            Console.WriteLine("read cosmosdb to get all workflows");
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            FeedIterator<dynamic> policyIterator = this.workflowContainer.GetItemQueryIterator<dynamic>(query);

            while (policyIterator.HasMoreResults)
            {
                FeedResponse<dynamic> response = await policyIterator.ReadNextAsync().ConfigureAwait(false);
                policies.AddRange(response);
            }

            return policies.Select(x => x.rule).ToList();
        }
    }
}