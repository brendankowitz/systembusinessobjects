@ECHO OFF
ECHO Requires NAnt-Contrib
ECHO Requires NAnt bin as environment variable
ECHO ------------------------------------------
ECHO Running Build Script...

nant -buildfile:Master.build auto

pause