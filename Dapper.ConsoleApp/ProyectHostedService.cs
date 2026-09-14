using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Volo.Abp;

namespace Dapper.ConsoleApp;

public class ProjectHostedService : IHostedService
{
    private readonly IAbpApplicationWithExternalServiceProvider _abpApplication;
    private readonly NewWay _newWay;
    private readonly ILogger<ProjectHostedService> _logger;
    public ProjectHostedService(IAbpApplicationWithExternalServiceProvider abpApplication,
        NewWay newWay,
        ILogger<ProjectHostedService> logger)
    {
        _abpApplication = abpApplication;
        _newWay = newWay;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ProjectHostedService StartAsync");


        #region new way

        // var resultData = _newWay.GetUsers();
        // _logger.LogInformation("Users: {@Users}", resultData);

        #endregion

        #region old way

        var data = new OldWay();
        var resultData2 = data.GetUsers();
        _logger.LogInformation("Users: {@Users}", resultData2);

        #endregion

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}