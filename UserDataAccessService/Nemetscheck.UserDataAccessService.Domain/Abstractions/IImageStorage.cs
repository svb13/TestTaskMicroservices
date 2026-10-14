namespace Nemetscheck.UserDataAccessService.Domain.Abstractions;

public interface IImageStorage
{
    Task<string> SaveAsync(Stream content, string fileName, long length, CancellationToken cancellationToken);

    Task DeleteAsync(string path, CancellationToken cancellationToken);
}
