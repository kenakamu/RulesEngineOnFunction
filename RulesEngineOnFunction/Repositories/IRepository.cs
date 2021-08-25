using System.Collections.Generic;
using System.Threading.Tasks;

namespace RulesEngineOnFunction.Repositories
{
    /// <summary>
    /// Repository interface
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Get RulesEngine workflow by name.
        /// </summary>
        /// <param name="id">Cosmos DB Id.</param>
        /// <returns>Workflow contents.</returns>
        Task<dynamic> GetWorkflowByIdAsync(string id);

        /// <summary>
        /// Get all RulesEngine workflows.
        /// </summary>
        /// <returns>List of Workflow contents.</returns>
        Task<List<dynamic>> GetAllWorkflowsAsync();
    }
}