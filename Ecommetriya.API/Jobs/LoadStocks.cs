using Ecommetriya.API.Services;
using Quartz;

namespace Ecommetriya.API.Jobs
{
    [DisallowConcurrentExecution]
    public class LoadStocks(IDataRepository dataRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context) => await dataRepository.LoadStocks();
    }
}
