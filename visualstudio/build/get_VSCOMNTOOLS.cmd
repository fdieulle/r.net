REM get a VSCOMNTOOLS
set VSCOMNTOOLS=%VS120COMNTOOLS%
@if "%VSCOMNTOOLS%"=="" VSCOMNTOOLS=%VS110COMNTOOLS%
@if "%VSCOMNTOOLS%"=="" VSCOMNTOOLS=%VS100COMNTOOLS%
@if "%VSCOMNTOOLS%"=="" goto error_no_VSCOMNTOOLS

@goto end

:error_no_VSCOMNTOOLS
@echo ERROR: Unable to get location of VS Common Tools folder please check your environment variable. Need to have VS120COMNTOOLS or VS110COMNTOOLS or VS100COMNTOOLS
@goto end

:end
@echo %VSCOMNTOOLS%