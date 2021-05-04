using Uranium;
using Xunit;

namespace Uranium.Tests
{
    public static class UraniumTests
    {
        [Fact]
        public static void ItRuns()
        {
            var text = new string[] { "dotnet run --project Source\\Uranium.Main\\Uranium.Main.csproj", @"
{
    var a = 10;
    var b = 100;
    for(; a < b; a += 1)
    {
        b -= 1;
    }
}", "--tree" };
            Uranium.Emit(text);
        }
    }
}
