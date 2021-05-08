
dotnet build Uranium.sln --configuration Release
dotnet test Uranium.sln
dotnet run  --project Source\Uranium.Main\Uranium.Main.csproj Test.urnm --boundTree --configuration Release
PAUSE