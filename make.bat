if "%1%" == "" goto ReleaseBuild
%windir%\Microsoft.NET\Framework\v3.5\msbuild.exe build\build.proj %1%


:ReleaseBuild
%windir%\Microsoft.NET\Framework\v3.5\msbuild.exe build\build.proj /filelogger /p:configuration=Release
pause