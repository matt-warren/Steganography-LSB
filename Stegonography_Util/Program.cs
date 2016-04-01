/*
* FILE : Program.cs
* PROJECT : ACS a3 CameraSteganography
* PROGRAMMER : Joe Student
* FIRST VERSION : 2012-05-01
* DESCRIPTION :
* The functions in this file are used to …
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography_Util
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                if (args[0] == "scan")
                {
                    Scanner.ScanFile(args[1]);
                }
                else if (args[0] == "encrypt")
                {
                    Encoder.Encrypt(args[1]);
                }
                else if (args[0] == "decrypt")
                {
                    Encoder.Decrypt(args[1]);
                }
            }
            else
            {
                Console.WriteLine("Usage: Stegonography_Util.exe scan|encrypt|decrypt filename");
                
            }
        }
    }
}
