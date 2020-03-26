# publish app
dotnet publish -c Release

$publishPath = '.\Dcidr.Blazor\bin\Release\netstandard2.1\publish\wwwroot';

# bump sw cache version number
$serviceWOrkerPath = $publishPath + '\service-worker.js';
((Get-Content -path $serviceWOrkerPath -Raw) -replace 'VERSION', [guid]::NewGuid().ToString()) | Set-Content -Path $serviceWOrkerPath

# this requires azcopy to be logged in
azcopy sync $publishPath "https://dcidr.blob.core.windows.net/`$web" --delete-destination true