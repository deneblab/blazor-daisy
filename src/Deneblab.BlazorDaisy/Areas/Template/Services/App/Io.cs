#pragma warning disable IDE0130
namespace Deneblab.SimpleEnv;

public static class Io
{
    public static void CreateDirIfNotExist(string? path)
    {
        if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static async Task CopyFileAsync(string sourcePath, string destinationPath)
    {
        using var source = File.Open(sourcePath, FileMode.Open);
        var dir = Path.GetDirectoryName(destinationPath);
        CreateDirIfNotExist(dir);

        using var destination = File.Create(destinationPath);
        await source.CopyToAsync(destination);
    }

    public static void DeleteRecursivelyWithMagicDust(string destinationDir)
    {
        const int magicDust = 10;
        for (var gnomes = 1; gnomes <= magicDust; gnomes++)
        {
            try
            {
                Directory.Delete(destinationDir, true);
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(50);
                continue;
            }

            return;
        }
    }

    public static async Task CopyFilesAsync(List<(string src, string dst)> files, Action<int> percentageProgressFn)
    {
        var count = 0;
        var numberEntries = files.Count;

        foreach (var file in files)
        {
            count++;
            await CopyFileAsync(file.src, file.dst);
            percentageProgressFn(GetNormalizedValue(numberEntries, count));
        }
    }

    private static int GetNormalizedValue(int max, int current)
    {
        return current * 100 / max;
    }

    public static void RenameFile(string src, string dst)
    {
        File.Move(src, dst);
    }

    public static void RemoveFilesFromFolder(string path, string pattern)
    {
        var dir = new DirectoryInfo(path);

        foreach (var fi in dir.GetFiles(pattern))
        {
            fi.IsReadOnly = false;
            fi.Delete();
        }
    }

    public static void RemoveFile(string path)
    {
        var fileInfo = new FileInfo(path);
        fileInfo.Delete();
    }

    public static void ClearFolder(string path)
    {
        var dir = new DirectoryInfo(path);

        foreach (var fi in dir.GetFiles())
        {
            fi.IsReadOnly = false;
            fi.Delete();
        }

        foreach (var di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            di.Delete();
        }
    }

    public static void RemoveFolder(string path)
    {
        var dir = new DirectoryInfo(path);

        foreach (var fi in dir.GetFiles())
        {
            fi.IsReadOnly = false;
            fi.Delete();
        }

        foreach (var di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            di.Delete();
        }

        dir.Delete();
    }

    public static string? FindDir(string targetDirectoryName, string? startPath = null)
    {
        for (var str = string.IsNullOrEmpty(startPath) ? Path.GetDirectoryName(Environment.ProcessPath) : startPath;
             str != null;
             str = Path.GetDirectoryName(str))
        {
            var path = Path.Combine(str, targetDirectoryName);
            if (Directory.Exists(path))
                return path;
        }

        return null;
    }

    public static string? FindParentDir(string targetDirectoryName, string? startPath = null)
    {
        var dir = FindDir(targetDirectoryName, startPath);
        return dir == null ? null : new DirectoryInfo(dir).Parent?.FullName;
    }

    public static string? FindFirstFile(string startPath, string searchPattern,
        SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (string.IsNullOrEmpty(startPath) || !Directory.Exists(startPath))
            return null;

        try
        {
            var files = Directory.GetFiles(startPath, searchPattern, searchOption);
            return files.Length > 0 ? files[0] : null;
        }
        catch
        {
            return null;
        }
    }
}
