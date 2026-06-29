using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.Dapper;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore;

public class BookDapperRepository : DapperRepository<BookStoreDbContext>, ITransientDependency
{
    public BookDapperRepository(IDbContextProvider<BookStoreDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
    
    public virtual async Task<List<string>> GetAllPersonNamesAsync()
    {
        var dbConnection = await GetDbConnectionAsync();
        var sql = """select * from People""";
        return (await dbConnection.QueryAsync<string>(sql, transaction:  await GetDbTransactionAsync())).ToList();
    }
}