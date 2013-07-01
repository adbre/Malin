# Malin Deploy
A small OWIN/NancyFx based deployment tool (server and client)

http://github.com/adbre/malin

## Build

    > .\psake.cmd

## Installation (Server)

1. Copy files from dist/ into a folder, e.g. C:\Malin
2. Edit Malin.exe.config and set AuthorizationToken to a random value...

To run as a service,

    PS> New-Service Malin -BinaryPath C:\Malin\Malin.exe -Credential DOMAIN\Myuser
    PS> Start-Service Malin

To run in a console window,

    PS> C:\Malin\Malin.exe /host

## To upload a package

    PS> .\dist\Malin.exe http://localhost:3030/ MyPackage.zip AUTHORIZATIONKEY

If MyPackage.zip contains a Deploy.ps1 file, it will be executed.

