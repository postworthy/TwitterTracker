@echo off
TwitterTracker\bin\Debug\TwitterTracker.exe -t %* | TwitterTracker.Filter\bin\Debug\TwitterTracker.Filter.exe "$..retweeted_status" "$..hashtags[?(@.text=='news')]" | TwitterTracker.Extract.Urls\bin\Debug\TwitterTracker.Extract.Urls.exe | TwitterTracker.Utilities.Frequency\bin\Debug\TwitterTracker.Utilities.Frequency.exe -console
