# cef-binaries-downloader
Scripts to download CEF binaries from http://opensource.spotify.com/cefbuilds/index.html

### Usage
Run chromelycef.exe or chromelycef.dll  on command line using the options below. You can get a release version from [binaries folder](https://github.com/chromelyapps/cef-binaries-downloader/tree/master/binaries). 

### Using .NET chromelycef.dll

To get help:
- chromelycef.exe download -h

Samples:
**Chromely versions: (v66, v68, v70 etc)**

- chromelycef.exe download v60 -v=3.3325.1751.ge5b78a5 -c=x64 
- chromelycef.exe download v60 -v=3.3325.1751.ge5b78a5 -c=x86 -d="bin\x86" 
- chromelycef.exe download v60 --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64 --dest="bin\x86"
- chromelycef.exe download v60 --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64
- chromelycef.exe download v60 --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64 --dest="C:\ChromelyDlls"
 
**For non Chromely versions:**
 
- chromelycef.exe download -v=3.3325.1751.ge5b78a5 -c=x64 
- chromelycef.exe download -v=3.3325.1751.ge5b78a5 -c=x86 -d="bin\x86" 
- chromelycef.exe download --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64 --dest="bin\x86"
- chromelycef.exe download --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64
- chromelycef.exe download --cef-binary-version=3.3325.1751.ge5b78a5 --cpu=x64 --dest="C:\ChromelyDlls"

### Using .NET Core chromelycef.dll

To get help:
- dotnet chromelycef.dll download -h

Samples:

**Chromely versions: (v66, v68, v70 etc)**

- dotnet chromelycef.dll download v60 -v=3.3325.1751.ge5b78a5 -o=win -d="bin\x86"
- dotnet chromelycef.dll download v60 -v=3.3325.1751.ge5b78a5 -o=linux -c=x64 
- dotnet chromelycef.dll download v60 -v=3.3325.1751.ge5b78a5 -o=win -c=x64 -d="bin\x86"
- dotnet chromelycef.dll download v60 --os=linux --cpu=x86 --dest="bin\x86"
- dotnet chromelycef.dll download v60 --cef-binary-version=3.3325.1751.ge5b78a5 --os=linux --cpu=x64 --dest="bin\x86"
- dotnet chromelycef.dll download v60 --dest="C:\ChromelyDlls"
- dotnet chromelycef.dll download v60 --cef-binary-version=3.3325.1751.ge5b78a5 --os=linux --cpu=x64 --dest="C:\ChromelyDlls"

**For non Chromely versions:**

- dotnet chromelycef.dll download  -v=3.3325.1751.ge5b78a5 -o=win -d="bin\x86"
- dotnet chromelycef.dll download  -v=3.3325.1751.ge5b78a5 -o=linux -c=x64 
- dotnet chromelycef.dll download  -v=3.3325.1751.ge5b78a5 -o=win -c=x64 -d="bin\x86"
- dotnet chromelycef.dll download  --os=linux --cpu=x86 --dest="bin\x86"
- dotnet chromelycef.dll download  --cef-binary-version=3.3325.1751.ge5b78a5 --os=linux --cpu=x64 --dest="bin\x86"
- dotnet chromelycef.dll download  --dest="C:\ChromelyDlls"
- dotnet chromelycef.dll download  --cef-binary-version=3.3325.1751.ge5b78a5 --os=linux --cpu=x64 --dest="C:\ChromelyDlls"
