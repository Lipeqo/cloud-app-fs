using CloudBackend.Models;
using Xunit;

namespace CloudBackend.Tests;

public class UnitTest1
{
    [Fact]
    public void NewTask_ShouldNotBeCompleted()
    {
        var task = new CloudTask
        {
            Name = "Przetestować bezpiecznik"
        };

        Assert.False(task.IsCompleted);
    }
}