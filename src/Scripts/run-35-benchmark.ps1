param(
    [int]$Chapter = 15
)

dotnet run --project "$PSScriptRoot/../book.csproj" -- --chapter $Chapter --mode benchmark | cat


