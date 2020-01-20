dotnet publish -c Release

# this requires azcopy to be logged in
azcopy sync ".\Dcidr.Blazor\bin\Release\netstandard2.1\publish\Dcidr.Blazor\dist" "https://dcidr.blob.core.windows.net/`$web" --delete-destination true