using System;
using System.IO;

namespace Malin
{
    internal static class MalinLicense
    {
        public static void WriteDisclaimer(TextWriter textWriter = null)
        {
            textWriter = textWriter ?? Console.Out;
            textWriter.WriteLine("Malin Deployment Tool - Tiny Windows commandline tool for remote deployment");
            textWriter.WriteLine("Copyright (C) 2013-2014 Adam Brengesjö <ca.brengesjo@gmail.com>");
            textWriter.WriteLine();
            textWriter.WriteLine("This program is free software: you can redistribute it and/or modify");
            textWriter.WriteLine("it under the terms of the GNU General Public License as published by");
            textWriter.WriteLine("the Free Software Foundation, either version 3 of the License, or");
            textWriter.WriteLine("(at your option) any later version.");
            textWriter.WriteLine();
            textWriter.WriteLine("This program is distributed in the hope that it will be useful,");
            textWriter.WriteLine("but WITHOUT ANY WARRANTY; without even the implied warranty of");
            textWriter.WriteLine("MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the");
            textWriter.WriteLine("GNU General Public License for more details.");
            textWriter.WriteLine();
            textWriter.WriteLine("You should have received a copy of the GNU General Public License");
            textWriter.WriteLine("along with this program.  If not, see <http://www.gnu.org/licenses/>.");
            textWriter.WriteLine();
        }
    }
}