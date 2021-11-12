using System.Collections.Generic;

namespace Api.Contracts.Requests
{
    public class TriggerPipelineRequest
    {
        public string PipelineName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
