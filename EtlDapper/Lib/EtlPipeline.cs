using System.Collections.Generic;
using System.Threading.Tasks;

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

    public EtlPipeline(IDataSource<TIn> source, ITransform<TIn, TOut> transform, IDataDestination<TOut> destination,
        int pageSize)
    {
        _source = source;
        _transform = transform;
        _destination = destination;
        _pageSize = pageSize;
    }

    public async Task RunAsync()
    {
        await _destination.InitializeAsync();
        int currentPage = 1;

        while (true)
        {
            var page = await _source.FetchPageAsync(currentPage, _pageSize);
            if (page.Items.Count == 0) break;

            var transformed = await _transform.TransformAsync(page.Items);
            await _destination.WriteBatchAsync(transformed);

            currentPage++;
        }
    }
}
