@echo off

SET dotnet="C:\Program Files\dotnet\dotnet.exe"  
SET opencover="C:\Users\mleandro\AppData\Local\Apps\OpenCover\OpenCover.Console.exe"
SET reportgenerator="C:\Program Files (x86)\ReportGenerator\ReportGenerator.exe"

SET targetargs="test"  
SET filter="+[*]StoneCo.Utils.* -[*.Test]* -[xunit.*]* -[FluentValidation]*"  
SET coveragefile=Coverage.xml  
SET coveragedir=Coverage

REM Run code coverage analysis  
%opencover% -oldStyle -register:user -target:%dotnet% -output:%coveragefile% -targetargs:%targetargs% -filter:%filter% -skipautoprops -hideskipped:All

REM Generate the report  
%reportgenerator% -targetdir:%coveragedir% -reporttypes:Html;Badges -reports:%coveragefile% -verbosity:Error

REM Open the report  
start "report" "%coveragedir%\index.htm"