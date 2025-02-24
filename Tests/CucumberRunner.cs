using NUnit.Framework;
using Reqnroll;

namespace qa_dotnet_cucumber.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)] // Enable parallel execution for all tests
    public class CucumberRunner
    {
        // Reqnroll will discover and run feature files automatically
    }
}