#!/bin/bash

StyleCop.Baboon Settings.StyleCop ./ ./bin ./obj ./libgame
gendarme --ignore gendarme.ignore --html gendarme_report.html ./bin/Debug/PersonalFont.exe
