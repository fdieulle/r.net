@echo off
echo "Install package r.net"

set R="%R_HOME%\bin\R.exe"

%R% CMD REMOVE "r.net"
%R% CMD INSTALL "%cd%" --build 

pause