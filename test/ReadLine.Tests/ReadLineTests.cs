using System;
using System.Linq;
using Xunit;

namespace ReadLine.Tests
{
    public class ReadLineTests : IDisposable
    {
        void AddHistory(params string[] history) => ReadLine.Context.History.AddRange(history);
        System.Collections.Generic.List<string> GetHistory() => ReadLine.Context.History;

        public ReadLineTests()
        {
            string[] history = new string[] { "ls -a", "dotnet run", "git init" };
            AddHistory(history);
        }

        [Fact]
        public void TestNoInitialHistory() 
        {
            Assert.Equal(3, GetHistory().Count);
        }

        [Fact]
        public void TestUpdatesHistory() 
        {
            AddHistory("mkdir");
            Assert.Equal(4, GetHistory().Count);
            Assert.Equal("mkdir", GetHistory().Last());
        }

        [Fact]
        public void TestGetCorrectHistory() 
        {
            Assert.Equal("ls -a", GetHistory()[0]);
            Assert.Equal("dotnet run", GetHistory()[1]);
            Assert.Equal("git init", GetHistory()[2]);
        }

        public void Dispose()
        {
            // If all above tests pass
            // clear history works
            ReadLine.Context.History.Clear();
        }
    }
}
