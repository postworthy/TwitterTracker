#!/bin/bash
#dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | dotnet TwitterTracker.Filter\bin\Debug\netcoreapp2.1\TwitterTracker.Filter.dll "$..retweeted_status" "$..hashtags[?(@.text=='news')]" "$.root[?(@.user.followers_count>400)]" "$.root[?(@.retweeted_status.retweet_count>10)]" | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll | dotnet TwitterTracker.Utilities.Frequency\bin\Debug\netcoreapp2.1\TwitterTracker.Utilities.Frequency.dll | dotnet HttpPostUtility\bin\Debug\netcoreapp2.1\HttpPostUtility.dll https://posttestserver.com/post.php?dir=postworthy data
#dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | dotnet TwitterTracker.Filter\bin\Debug\netcoreapp2.1\TwitterTracker.Filter.dll "$.root[?(@.user.followers_count>400)]" | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll | dotnet TwitterTracker.Utilities.Frequency\bin\Debug\netcoreapp2.1\TwitterTracker.Utilities.Frequency.dll
dotnet TwitterTracker\bin\Debug\netcoreapp2.1\TwitterTracker.dll -tx news | dotnet TwitterTracker.Filter\bin\Debug\netcoreapp2.1\TwitterTracker.Filter.dll "$.root[?(@.user.followers_count>400)]" | dotnet TwitterTracker.Extract.Urls\bin\Debug\netcoreapp2.1\TwitterTracker.Extract.Urls.dll