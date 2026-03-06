using System.Diagnostics;
using System.IO.Compression;
using System.Threading;

namespace Jx.Cms.Common.Utils;

public static class PackageImportHelper
{
    public static string CreateTempDirectory(string prefix)
    {
        var root = Path.Combine(Path.GetTempPath(), "jxcms", prefix);
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }

    public static void ExtractZipSafely(string zipPath, string destinationDirectory)
    {
        if (!File.Exists(zipPath)) throw new FileNotFoundException("Compressed package not found", zipPath);

        if (Directory.Exists(destinationDirectory)) DeleteDirectoryWithRetry(destinationDirectory);
        Directory.CreateDirectory(destinationDirectory);

        var basePath = Path.GetFullPath(destinationDirectory);
        if (!basePath.EndsWith(Path.DirectorySeparatorChar))
        {
            basePath += Path.DirectorySeparatorChar;
        }

        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            var entryName = (entry.FullName ?? string.Empty).Replace('\\', '/').TrimStart('/');
            if (string.IsNullOrWhiteSpace(entryName)) continue;

            var targetPath = Path.GetFullPath(Path.Combine(destinationDirectory, entryName));
            if (!targetPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Illegal zip entry path: {entry.FullName}");

            if (entry.FullName.EndsWith("/", StringComparison.Ordinal))
            {
                Directory.CreateDirectory(targetPath);
                continue;
            }

            var targetDir = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrWhiteSpace(targetDir)) Directory.CreateDirectory(targetDir);
            entry.ExtractToFile(targetPath, true);
        }
    }

    public static void CopyDirectory(string sourceDirectory, string targetDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

        Directory.CreateDirectory(targetDirectory);

        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            var targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
            CopyFileWithRetry(file, targetFile);
        }

        foreach (var dir in Directory.GetDirectories(sourceDirectory))
        {
            var childTarget = Path.Combine(targetDirectory, Path.GetFileName(dir));
            CopyDirectory(dir, childTarget);
        }
    }

    public static void DeleteDirectoryWithRetry(string directory, int retryCount = 6, int delayMs = 250)
    {
        if (TryDeleteDirectoryWithRetry(directory, retryCount, delayMs)) return;

        NormalizeDirectoryAttributes(directory);
        Directory.Delete(directory, true);
    }

    public static bool TryDeleteDirectoryWithRetry(string directory, int retryCount = 6, int delayMs = 250)
    {
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return true;

        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                NormalizeDirectoryAttributes(directory);
                Directory.Delete(directory, true);
                return true;
            }
            catch when (i < retryCount - 1)
            {
                Thread.Sleep(delayMs);
            }
        }

        return !Directory.Exists(directory);
    }

    public static void MoveDirectoryWithRetry(string sourceDirectory, string destinationDirectory, int retryCount = 10,
        int delayMs = 300)
    {
        if (!Directory.Exists(sourceDirectory))
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");

        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                Directory.Move(sourceDirectory, destinationDirectory);
                return;
            }
            catch (IOException) when (i < retryCount - 1)
            {
                Thread.Sleep(delayMs);
            }
            catch (UnauthorizedAccessException) when (i < retryCount - 1)
            {
                Thread.Sleep(delayMs);
            }
        }

        Directory.Move(sourceDirectory, destinationDirectory);
    }

    public static bool WaitForFileAvailable(string filePath, int timeoutMs = 6000, int delayMs = 250)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return true;

        var watch = Stopwatch.StartNew();
        while (watch.ElapsedMilliseconds < timeoutMs)
        {
            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return true;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }

            Thread.Sleep(delayMs);
        }

        return false;
    }

    private static void CopyFileWithRetry(string sourceFile, string targetFile, int retryCount = 10, int delayMs = 250)
    {
        for (var i = 0; i < retryCount; i++)
        {
            try
            {
                EnsureWritable(targetFile);
                File.Copy(sourceFile, targetFile, true);
                return;
            }
            catch (IOException) when (i < retryCount - 1)
            {
                Thread.Sleep(delayMs);
            }
            catch (UnauthorizedAccessException) when (i < retryCount - 1)
            {
                Thread.Sleep(delayMs);
            }
        }

        EnsureWritable(targetFile);
        File.Copy(sourceFile, targetFile, true);
    }

    private static void EnsureWritable(string filePath)
    {
        if (!File.Exists(filePath)) return;
        var attributes = File.GetAttributes(filePath);
        if ((attributes & FileAttributes.ReadOnly) == 0) return;
        File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
    }

    private static void NormalizeDirectoryAttributes(string directory)
    {
        if (!Directory.Exists(directory)) return;

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            try
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            catch
            {
                // ignore and keep retrying delete
            }
        }
    }
}
