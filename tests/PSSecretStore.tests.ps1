Import-Module "$PSScriptRoot/../output/PSSecretStore.psd1"

Describe "PSSecretStore" {
    Context "Export-SSKey" {
        It "exports a key file" {
            $Tempfile = [IO.Path]::GetTempFileName()

            Export-SSKey -KeyPath $tempFile

            Get-ChildItem $tempFile | should not be $null
        }
    }

    Context "Set-SSSecret" {
        It "sets a secret by password" {
            $Store = [IO.Path]::GetTempFileName()

            Set-SSSecret -Name "Secret" -Value "This is a secret" -Password "Password" -StorePath $Store

            Get-SSSecret -Name 'Secret' -StorePath $Store -Password "Password" | Should be "This is a secret"
        }

        It "sets a secret by key file" {
            $Store = [IO.Path]::GetTempFileName()
            $KeyFile = [IO.Path]::GetTempFileName()

            Export-SSKey -KeyPath $KeyFile

            Set-SSSecret -Name "Secret" -Value "This is a secret" -KeyPath $KeyFile -StorePath $Store
            Set-SSSecret -Name "Secret2" -Value "This is a secret" -KeyPath $KeyFile -StorePath $Store

            Get-SSSecret -Name 'Secret' -StorePath $Store -KeyPath $KeyFile | Should be "This is a secret"
            Get-SSSecret -Name 'Secret2' -StorePath $Store -KeyPath $KeyFile | Should be "This is a secret"
        }
    }
}