@echo OFF

@set CURRENT_DIR=%~d0%~p0
@set BUILD_DIR=%CURRENT_DIR%build

REM Create rdll.lib
@call %BUILD_DIR%\create_rdll.lib.cmd %BUILD_DIR%\

REM Restore Nuget dependencies
@call %CURRENT_DIR%.nuget\Nuget.exe restore -PackagesDirectory "%CURRENT_DIR%libfiles" "%CURRENT_DIR%RDotNet.ClrProxy\packages.config"
@call %CURRENT_DIR%.nuget\Nuget.exe restore -PackagesDirectory "%CURRENT_DIR%libfiles" "%CURRENT_DIR%RDotNet.ClrProxyTests\packages.config"

REM Get MsBuild.exe
@call %BUILD_DIR%\get_msbuildpath.cmd

set MODE=Build
set BUILD_CONFIGURATION=Release
REM set MSB_OPTIONS=/p:VisualStudioVersion=11.0 /consoleloggerparameters:ErrorsOnly
set MSB_OPTIONS=/p:VisualStudioVersion=11.0

set PROXY_CSPROJ=%CURRENT_DIR%\RDotNet.ClrProxy\RDotNet.ClrProxy.csproj
set RHOSTCLR_CSPROJ=%CURRENT_DIR%\RDotNet.RHostClr\RDotNet.RHostClr.vcxproj

%MSBuildToolsPath% %PROXY_CSPROJ% /t:%MODE% /p:Configuration=%BUILD_CONFIGURATION% %MSB_OPTIONS%
%MSBuildToolsPath% %RHOSTCLR_CSPROJ% /t:%MODE% /p:Configuration=%BUILD_CONFIGURATION% /p:Platform="x64" %MSB_OPTIONS%
%MSBuildToolsPath% %RHOSTCLR_CSPROJ% /t:%MODE% /p:Configuration=%BUILD_CONFIGURATION% /p:Platform="Win32" %MSB_OPTIONS%