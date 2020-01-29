﻿using System;
using System.IO;
using System.Management.Automation;
using NeoSmart.SecureStore;

namespace PSSecretStore
{
    [Cmdlet(VerbsData.Export, "SSKey")]
    public class ExportKeyCommand : PSCmdlet 
    {
        [Parameter(Mandatory = true)]
        public string KeyPath { get; set; }

        protected override void ProcessRecord() 
        {
            KeyPath = GetUnresolvedProviderPathFromPSPath(KeyPath);

            using (var sman = SecretsManager.CreateStore())
            {
                 sman.GenerateKey();
                 sman.ExportKey(KeyPath);
            }
        }
    }

    [Cmdlet(VerbsCommon.Set, "SSSecret")]
    public class SetSecretCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter(Mandatory = true)]
        public string StorePath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "KeyPath")]
        public string KeyPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Password")]
        public string Password { get; set; }

        protected override void ProcessRecord() 
        {
            StorePath = GetUnresolvedProviderPathFromPSPath(StorePath);

            var validFile = false; 
            if (File.Exists(StorePath))
            {
                var text = File.ReadAllText(StorePath);
                validFile = text.Length > 0;
            }

            using (var sman = (validFile ? SecretsManager.LoadStore(StorePath) : SecretsManager.CreateStore()))
            {
                if (ParameterSetName == "KeyPath")
                {
                    KeyPath = GetUnresolvedProviderPathFromPSPath(KeyPath);
                    sman.LoadKeyFromFile(KeyPath);
                }

                if (ParameterSetName == "Password")
                {
                    sman.LoadKeyFromPassword(Password);                
                }

                sman.Set(Name, Value);

                sman.SaveStore(StorePath);
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "SSSecret")]
    public class GetSecretCommand : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name { get; set; }
        [Parameter(Mandatory = true)]
        public string StorePath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "KeyPath")]
        public string KeyPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Password")]
        public string Password { get; set; }

        protected override void ProcessRecord() 
        {
            StorePath = GetUnresolvedProviderPathFromPSPath(StorePath);

            using (var sman = SecretsManager.LoadStore(StorePath))
            {
                if (ParameterSetName == "KeyPath")
                {
                    KeyPath = GetUnresolvedProviderPathFromPSPath(KeyPath);
                    sman.LoadKeyFromFile(KeyPath);
                }

                if (ParameterSetName == "Password")
                {
                    sman.LoadKeyFromPassword(Password);                
                }

                WriteObject(sman.Get(Name));
            }
        }
    }
}
