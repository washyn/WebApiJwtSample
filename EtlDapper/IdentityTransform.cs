using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtlDapper;

public class IdentityTransform<T> : ITransform<T, T>
{
    public Task<List<T>> TransformAsync(List<T> input)
    {
        return Task.FromResult(input);
    }
}
