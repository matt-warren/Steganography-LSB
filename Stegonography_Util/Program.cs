/*
* FILE : Program.cs
* PROJECT : ACS a3 CameraSteganography
* PROGRAMMER : Matt Warren & Steven Johnston
* FIRST VERSION : 2016-03-31
* DESCRIPTION :
* This file is used to illustrate the use of the static functions in Encoder.cs and Scanner.cs
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
