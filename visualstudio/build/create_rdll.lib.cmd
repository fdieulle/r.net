@echo OFF

if [%1]==[] (
	@set CUR_DIR=%~d0%~p0
) else (
	@set CUR_DIR=%1
)

@echo %CUR_DIR%

REM get a VSCOMNTOOLS
@call %CUR_DIR%get_VSCOMNTOOLS.cmd

set VSDEVENV="%VSCOMNTOOLS%..\..\VC\vcvarsall.bat"
@if not exist %VSDEVENV% goto error_no_vcvarsall
@call %VSDEVENV%

REM Prepare output folders for Rdll.lib files by arch
@set LIBDIR32=%CUR_DIR%..\libfiles\i386
@set LIBDIR64=%CUR_DIR%..\libfiles\x64
@if not exist %LIBDIR32% mkdir %LIBDIR32%
@if not exist %LIBDIR64% mkdir %LIBDIR64%

REM Generate Rdll.lib files from R.def
@set LIB_EXE=lib

%LIB_EXE% /nologo /def:%CUR_DIR%R.def /out:%LIBDIR32%\Rdll.lib /machine:x86
%LIB_EXE% /nologo /def:%CUR_DIR%R.def /out:%LIBDIR64%\Rdll.lib /machine:x64

@goto end

:error_no_vcvarsall
@echo ERROR: Cannot find script file %VSDEVENV%
@goto end

:end