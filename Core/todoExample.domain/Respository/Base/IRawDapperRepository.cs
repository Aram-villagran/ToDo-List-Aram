namespace todoExample.domain.Respository.Base;

public interface IRawDapperRepository
{
    public Task<IEnumerable<T>> GenericQueryAsync<T>(string query, object? parameters = null);
    public Task<dynamic> ExecuteAsync(string command, object parameters);
}