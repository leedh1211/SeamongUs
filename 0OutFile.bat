@echo off
setlocal

set "source_dir=./Assets/02_Scripts"
set "output_file=merged_output.txt"

if exist "%output_file%" del "%output_file%"

:: UTF-8 인코딩으로 주석 제거 후 병합
for /r "%source_dir%" %%f in (*.cs) do (
    echo ===== 파일: %%~dpnxF ===== >> "%output_file%"
    powershell -Command ^
        "Get-Content -LiteralPath '%%f' | Where-Object { -not ($_ -match '^\s*//') } | Out-File -Append -Encoding utf8 '%output_file%'"
    echo. >> "%output_file%"
)

echo [완료] 모든 .cs 파일을 병합하였고, 주석은 제거되었습니다.
pause