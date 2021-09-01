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
        /// Get changed item since specified time.
        /// </summary>
        /// <returns>List of workflows.</returns>
        public Task<List<dynamic>> GetChangedWorkflowsAsync();
    }
}