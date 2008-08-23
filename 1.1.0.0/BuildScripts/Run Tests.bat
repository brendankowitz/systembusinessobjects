@ECHO OFF
ECHO Requires NAnt-Contrib
ECHO Requires NAnt bin as environment variable
ECHO ------------------------------------------
ECHO Running NUnit Test Script...

nant -buildfile:Master.build coverage

pause