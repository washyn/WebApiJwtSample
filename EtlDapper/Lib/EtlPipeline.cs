using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EtlDapper.Lib;

public interface IDataSource<T>
{
    Task<DataPage<T>> FetchPageAsync(int pageIndex, int pageSize);
}

public interface ITransform<TIn, TOut>
{
    Task<List<TOut>> TransformAsync(List<TIn> input);
}

public interface IDataDestination<T>
{
    Task InitializeAsync();
    Task WriteBatchAsync(List<T> items);
}

public class DataPage<T>
{
    public List<T> Items { get; set; }
    public int PageIndex { get; set; }

    public DataPage(List<T> items, int pageIndex)
    {
        Items = items;
        PageIndex = pageIndex;
    }
}

public class EtlPipeline<TIn, TOut>
{
    private readonly IDataSource<TIn> _source;
    private readonly ITransform<TIn, TOut> _transform;
    private readonly IDataDestination<TOut> _destination;
    private readonly int _pageSize;
    private readonly ILogger _logger;

    public EtlPipeline(IDataSource<TIn> source, ITransform<TIn, TOut> transform, IDataDestination<TOut> destination,
        int pageSize, ILogger logger)
    {
        _source = source;
        _transform = transform;
        _destination = destination;
        _pageSize = pageSize;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("Starting ETL process...");
        await _destination.InitializeAsync();
        int currentPage = 1;
        long totalProcessed = 0;

        while (true)
        {
            _logger.LogInformation("Fetching page {PageNumber} (Size: {PageSize})...", currentPage, _pageSize);
            var page = await _source.FetchPageAsync(currentPage, _pageSize);

            if (page.Items.Count == 0)
            {
                _logger.LogInformation("No more records found. Finishing ETL. Total records processed: {Total}",
                    totalProcessed);
                break;
            }

            _logger.LogInformation("Read {Count} records. Transforming...", page.Items.Count);
            var transformed = await _transform.TransformAsync(page.Items);

            _logger.LogInformation("Writing {Count} records to destination...", transformed.Count);
            await _destination.WriteBatchAsync(transformed);

            totalProcessed += transformed.Count;
            _logger.LogInformation("Page {PageNumber} processed successfully. Total processed so far: {Total}",
                currentPage, totalProcessed);
            currentPage++;
        }

        _logger.LogInformation("ETL process completed. Final count: {Total}", totalProcessed);
    }
}
