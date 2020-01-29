using System;
using System.IO;
using System.Management.Automation;
using NeoSmart.SecureStore;
using System.Collections;

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
            try {
                if (File.Exists(StorePath)) {
                    using (var sman = SecretsManager.LoadStore(StorePath)) {
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

                        StorePath = GetUnresolvedProviderPathFromPSPath(StorePath);

                        sman.SaveStore(StorePath);
                    }   
                }
            }
            catch {
                //either the file doesn't exist, or it is a non-secretful file
                using (var sman = SecretsManager.CreateStore())
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
                    
                    StorePath = GetUnresolvedProviderPathFromPSPath(StorePath);

                    sman.SaveStore(StorePath);
                }
            }
        }
    }

    [Cmdlet(VerbsCommon.Get, "SSSecret")]
    public class GetSecretCommand : PSCmdlet
    {
        [Parameter(Mandatory = false)]
        public string Name { get; set; }
        [Parameter(Mandatory = true)]
        public string StorePath { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter All {get; set;}

        [Parameter(Mandatory = true, ParameterSetName = "KeyPath")]
        public string KeyPath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Password")]
        public string Password { get; set; }

        protected override void ProcessRecord() 
        {
            StorePath = GetUnresolvedProviderPathFromPSPath(StorePath);

            using (var sman = SecretsManager.LoadStore(StorePath))
            {
                if (All) {
                    if (ParameterSetName == "KeyPath")
                    {
                        KeyPath = GetUnresolvedProviderPathFromPSPath(KeyPath);
                        sman.LoadKeyFromFile(KeyPath);
                    }

                    if (ParameterSetName == "Password")
                    {
                        sman.LoadKeyFromPassword(Password);                
                    }

                    Hashtable toReturn = new Hashtable();
                    foreach (var k in sman.Keys) {
                        toReturn.Add(k, sman.Get(k));
                    }
                    
                    WriteObject(toReturn);
                }
                else {
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
}
