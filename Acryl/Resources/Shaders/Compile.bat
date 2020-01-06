@echo off

for /R ".\" %%f in (*.fx) do (
    ..\..\..\Tools\2MGFX.exe %%f %%~nf /Profile:OpenGL
)
