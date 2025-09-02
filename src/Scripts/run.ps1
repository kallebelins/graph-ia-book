param(
    [int]$chapter = 1,
    [string]$mode = "b"
)

dotnet run --project ..\book.csproj -- -c $chapter -m $mode


