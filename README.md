# Malin Deploy
A small OWIN/NancyFx based deployment tool (server and client)

http://github.com/adbre/malin

## License

    Malin Deployment Tool - Tiny Windows commandline tool for remote deployment
    Copyright (C) 2013-2014 Adam Brengesjö <ca.brengesjo@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

## Build

    > .\psake.cmd

## Installation (Server)

1. Copy files from dist/ into a folder, e.g. C:\Malin
2. Edit Malin.exe.config and set UnpackZipFileDestination to e.g. C:\Malin\Unpacked
3. Edit Malin.exe.config and set AuthorizationToken to a random value...

To run as a service,

    PS> New-Service Malin -BinaryPath C:\Malin\Malin.exe -Credential DOMAIN\Myuser
    PS> Start-Service Malin

To run in a console window,

    PS> C:\Malin\Malin.exe /host

## To upload a package

    PS> .\dist\Malin.exe deploy http://localhost:3030/ MyPackage.zip AUTHORIZATIONKEY

If MyPackage.zip contains a Deploy.ps1 file, it will be executed.

