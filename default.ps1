properties {
    $Configuration="Release"
}

task default -depends Compile,CopyOutput

task Compile {
    Exec { msbuild /p:Configuration=$Configuration /t:Build ".\src\Malin.sln" }
}

task CopyOutput {
    ROBOCOPY /MIR ".\src\Malin\bin\$Configuration" ".\dist"
    if ($LastExitCode -gt 5) {
        throw "ROBOCOPY returned with exit code $LastExitCode"
    }

    Copy-Item ".\README.md" ".\dist"
    Copy-Item ".\LICENSE.txt" ".\dist"
}
