@REM HINT: SET SECOND ARGUMENT TO /NOPAUSE WHEN AUTOMATING THE BUILD.

@SET Config=%1%
@IF [%1] == [] SET Config=Debug

@IF DEFINED VisualStudioVersion GOTO SkipVcvarsall
IF "%VS140COMNTOOLS%" NEQ "" CALL "%VS140COMNTOOLS%VsDevCmd.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS120COMNTOOLS%" NEQ "" CALL "%VS120COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS110COMNTOOLS%" NEQ "" CALL "%VS110COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
IF "%VS100COMNTOOLS%" NEQ "" CALL "%VS100COMNTOOLS%\..\..\VC\vcvarsall.bat" x86 && GOTO EndVcvarsall || GOTO Error0
ECHO ERROR: Cannot detect Visual Studio, missing VSxxxCOMNTOOLS variable.
GOTO Error0
:EndVcvarsall
@ECHO ON
:SkipVcvarsall

PUSHD "%~dp0" || GOTO Error0
CALL packages\Rhetos\UpdateRhetosDlls.bat /nopause || GOTO Error1
CALL ChangeVersions.bat || GOTO Error1
IF EXIST Build.log DEL Build.log || GOTO Error1
DevEnv.com Rhetos.I18NFormatter.sln /rebuild %Config% /out Build.log || TYPE Build.log && GOTO Error1
CALL CreatePackage.bat || GOTO Error1
CALL ChangeVersions.bat /RESTORE || GOTO Error1
POPD

@REM ================================================

@ECHO.
@ECHO %~nx0 SUCCESSFULLY COMPLETED.
@EXIT /B 0

:Error1
@POPD
:Error0
@ECHO.
@ECHO %~nx0 FAILED.
@IF /I [%2] NEQ [/NOPAUSE] @PAUSE
@EXIT /B 1
