using System;
using System.IO;
using Xunit;
using FileData;

namespace Tests;

/// <summary>
/// Tests for verifying <see cref="FileContext"/> behavior when persisting data to disk.
/// </summary>
public class FileContextTests
{
    /// <summary>
    /// Ensures the context starts with empty collections when no data file is present.
    /// </summary>
    [Fact]
    public void TodosAndUsers_ReturnEmptyCollections_WhenNoFile()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(tempDir);

        try
        {
            var context = new FileContext();
            Assert.Empty(context.Todos);
            Assert.Empty(context.Users);
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, true);
        }
    }

    /// <summary>
    /// Verifies that calling <see cref="FileContext.SaveChanges"/> creates the JSON data file.
    /// </summary>
    [Fact]
    public void SaveChanges_CreatesDataFile()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        string originalDir = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(tempDir);

        try
        {
            var context = new FileContext();
            var _ = context.Users; // initialize container
            context.SaveChanges();
            Assert.True(File.Exists("data.json"));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDir);
            Directory.Delete(tempDir, true);
        }
    }
}
