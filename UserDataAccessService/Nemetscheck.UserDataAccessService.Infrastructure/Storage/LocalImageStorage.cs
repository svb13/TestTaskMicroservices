using Microsoft.Extensions.Options;
using Nemetscheck.UserDataAccessService.Domain.Abstractions;
using Nemetscheck.UserDataAccessService.Domain.Exceptions;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Storage;

internal sealed class LocalImageStorage(IOptions<ImageStorageOptions> options) : IImageStorage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    public async Task<string> SaveAsync(Stream content, string fileName, long length, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);

        var settings = options.Value;
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new ValidationException("Image must be a .jpg, .jpeg, .png or .webp file");

        if (length <= 0 || length > settings.MaxBytes)
            throw new ValidationException($"Image size must be between 1 and {settings.MaxBytes} bytes");

        Directory.CreateDirectory(settings.RootPath);

        var storedName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(settings.RootPath, storedName);

        try
        {
            var header = new byte[HeaderLength];
            var headerRead = await content.ReadAtLeastAsync(header, HeaderLength, throwOnEndOfStream: false, cancellationToken);

            if (!HasValidSignature(header.AsSpan(0, headerRead), extension))
                throw new ValidationException("Image content does not match a valid .jpg, .jpeg, .png or .webp file");

            await using var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await file.WriteAsync(header.AsMemory(0, headerRead), cancellationToken);
            await CopyWithLimitAsync(content, file, settings.MaxBytes - headerRead, cancellationToken);
        }
        catch
        {
            File.Delete(fullPath);
            throw;
        }

        return storedName;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var fullPath = Path.Combine(options.Value.RootPath, Path.GetFileName(path));
        File.Delete(fullPath);
        return Task.CompletedTask;
    }

    private const int HeaderLength = 12;

    private static bool HasValidSignature(ReadOnlySpan<byte> header, string extension) =>
        extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => header.StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }),
            ".png" => header.StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" => header.Length >= HeaderLength
                && header[..4].SequenceEqual("RIFF"u8)
                && header[8..12].SequenceEqual("WEBP"u8),
            _ => false
        };

    private static async Task CopyWithLimitAsync(Stream source, Stream destination, long remaining, CancellationToken cancellationToken)
    {
        var buffer = new byte[81920];
        int read;
        
        while ((read = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            remaining -= read;
            if (remaining < 0)
                throw new ValidationException("Image exceeds the maximum allowed size");

            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }
}
