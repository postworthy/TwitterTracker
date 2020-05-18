#dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | ForEach-Object { [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($_)) | ConvertFrom-Json }
$track="CVE-"
dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx $track | ForEach-Object { [System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String($_)) | ConvertFrom-Json } | select text

#dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx github | ForEach-Object { $_ | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll }

#Also from powershell you can execute the pipes like this:
#cmd /C 'dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | dotnet TwitterTracker.Filter\bin\Debug\netcoreapp2.1\TwitterTracker.Filter.dll "$.root[?(@.user.followers_count>400)]" | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll''

#This example will begin building as list of the top 50 most populat URLs that pass through the stream
#cmd /C 'dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | dotnet TwitterTracker.Filter\bin\Debug\netcoreapp2.1\TwitterTracker.Filter.dll "$.root[?(@.user.followers_count>400)]" | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll | dotnet TwitterTracker.Utilities.Frequency\bin\Debug\netcoreapp2.1\TwitterTracker.Utilities.Frequency.dll -console'