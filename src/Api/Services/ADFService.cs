using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.Options;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public class ADFService : IADFService
    {
        private readonly AzureOptions _azureOptions;
        private readonly DataFactoryManagementClient _dataFactoryManagementClient;

        public ADFService(IOptions<AzureOptions> azureOptions, DataFactoryManagementClient dataFactoryManagementClient)
        {
            _azureOptions = azureOptions.Value;
            _dataFactoryManagementClient = dataFactoryManagementClient;
        }

        public async Task<PipelineRun> GetPipelineStatusAsync(string runId, CancellationToken cancellationToken = default)
        {
            var pipelineRun = await _dataFactoryManagementClient.PipelineRuns.GetAsync(
                _azureOptions.ResourceGroup,
                _azureOptions.DataFactoryName,
                runId,
                cancellationToken);

            return pipelineRun;
        }

        public async Task<CreateRunResponse> RunPipelineAsync(string pipelineName,
            IDictionary<string, object> parameters,
            CancellationToken cancellationToken = default
        )
        {
            var runResponse = await _dataFactoryManagementClient.Pipelines.CreateRunAsync(
                _azureOptions.ResourceGroup,
                _azureOptions.DataFactoryName,
                pipelineName,
                parameters: parameters,
                cancellationToken: cancellationToken);

            return runResponse;
        }
    }
}
