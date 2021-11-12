using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory.Models;

namespace Api.Services
{
    public interface IADFService
    {
        Task<CreateRunResponse> RunPipelineAsync(string pipelineName, IDictionary<string, object> parameters, CancellationToken cancellationToken);
        Task<PipelineRun> GetPipelineStatusAsync(string runId, CancellationToken cancellationToken);
    }
}
