using System.ComponentModel.DataAnnotations;

namespace Nemetscheck.UserDataAccessService.Infrastructure.Storage;

public sealed class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    [Required]
    public string RootPath { get; set; } = "uploads";

    [Range(typeof(long), "1", "52428800")]
    public long MaxBytes { get; set; } = 5 * 1024 * 1024;
}
