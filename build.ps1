Set-Location $PSScriptRoot
dotnet publish -c Release

Remove-Item output -Force -ErrorAction Ignore -Recurse
New-Item -ItemType Directory output
Copy-Item bin/Release/netstandard2.0/publish/*.* output
Copy-Item PSSecretStore.psd1 output