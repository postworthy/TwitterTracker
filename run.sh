#!/bin/bash
mono TwitterTracker/bin/Debug/TwitterTracker.exe -tx \#news | mono TwitterTracker.Extract.Urls/bin/Debug/TwitterTracker.Extract.Urls.exe | mono TwitterTracker.Utilities.Frequency/bin/Debug/TwitterTracker.Utilities.Frequency.exe | mono CloudUtility/bin/Debug/CloudUtility.exe -u
