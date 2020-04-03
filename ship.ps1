# publish app
dotnet publish -c Release

$publishPath = '.\Dcidr.Blazor\bin\Release\netstandard2.1\publish\wwwroot';

# this requires azcopy to be logged in
azcopy sync $publishPath "https://dcidr.blob.core.windows.net/`$web" --delete-destination true