# PSSecretStore

A cross-platform PowerShell wrapper around [SecretStore](https://github.com/neosmart/SecureStore)

# Installation 

```
Install-Module PSSecretStore
```

# Examples 

Generate a new key file. 

```
$Tempfile = [IO.Path]::GetTempFileName()

Export-SSKey -KeyPath $tempFile
```

Store and retrieve a secret by password 

```
$Store = [IO.Path]::GetTempFileName()

Set-SSSecret -Name "Secret" -Value "This is a secret" -Password "Password" -StorePath $Store

Get-SSSecret -Name 'Secret' -StorePath $Store -Password "Password"
```

Store and retrieve a secret by key file 

```
$Store = [IO.Path]::GetTempFileName()
$KeyFile = [IO.Path]::GetTempFileName()

Export-SSKey -KeyPath $KeyFile

Set-SSSecret -Name "Secret" -Value "This is a secret" -KeyPath $KeyFile -StorePath $Store

Get-SSSecret -Name 'Secret' -StorePath $Store -KeyPath $KeyFile
```
