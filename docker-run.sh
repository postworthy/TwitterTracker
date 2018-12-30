#!/bin/sh
dotnet TwitterTracker.dll -tx \#news | dotnet TwitterTracker.Extract.Urls.dll | dotnet TwitterTracker.Utilities.Frequency.dll
