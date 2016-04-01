using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.IO;

namespace Steganography_Util
{
    class Encoder
    {
        public Encoder()
        {
        }

        //Enum to for determining what color is going to be worked on
        enum encodeColor
        {
            red,
            green,
            blue
        }


        // DESCRIPTION :
        //              this function is used to encrypt a message in a file. It takes a filepath for input
        //              and then reads it as binary and changes the LSB of each pixel to the message the user
        //              inputs.
        // PARAMETERS :
        //              string: filename --- this is the filename/path of the image to encrypt
        static public void Encrypt(string filename)
        {
            //make sure the file exists first
            if (File.Exists(filename))
            {
                //get user's message
                Console.WriteLine("Enter the message to encode: ");
                string message = Console.ReadLine();

                //open file as bitmap
                Bitmap inBitmap = new Bitmap(filename);

                //for holding a pixel's rgb values
                int r = 0;
                int g = 0;
                int b = 0;

                //current character as int that will be put into the file
                int currChar = 0;

                //number of characters encrypted so far
                int charNum = 0;

                //number of zeros at the end so far
                int endZeros = 0;

                //if still hiding text, this is true
                bool hidingText = false;

                //if done the message, adding zeros at the end, this is true
                bool endMessage = false;

                //if finished, this is true
                bool finished = false;

                //used to check when to move onto the next character
                int pixelIndex = 0;

                //used to keep track of what color will be worked on next/currently
                encodeColor currColor = encodeColor.red;

                try
                {
                    //loop for the width of the bitmap
                    for (int w = 0; w < inBitmap.Width; w++)
                    {
                        //loop for the height of the bitmap
                        for (int h = 0; h < inBitmap.Height; h++)
                        {
                            //get a pixel
                            Color pixel = inBitmap.GetPixel(w, h);

                            //make the LSB 0
                            r = pixel.R - (pixel.R & 0x00000001);
                            b = pixel.B - (pixel.B & 0x00000001);
                            g = pixel.G - (pixel.G & 0x00000001);
                            
                            //loop to go through the bits
                            for (int i = 0; i < 3; i++)
                            {
                                //if at end and 1 byte worth of 0s have been added
                                if (endMessage && endZeros == 8)
                                {
                                    if (currColor != encodeColor.red)
                                    {
                                        inBitmap.SetPixel(w, h, Color.FromArgb(r, g, b));
                                    }
                                    finished = true;
                                    break;
                                }
                                if (pixelIndex % 8 == 0)
                                {
                                    if (charNum >= message.Length)
                                    {
                                        //stop encoding and just do the rest of the pixels
                                        hidingText = false;
                                        endMessage = true;
                                    }
                                    else {
                                        currChar = message[charNum++];

                                        hidingText = true;
                                    }
                                }
                                switch (currColor)
                                {
                                    case encodeColor.red:
                                        if (hidingText)
                                        {
                                            //get the last bit of the letter as a number
                                            r += (currChar & 0x00000001) | (r & 0x00000001);
                                            currChar >>= 1; //shift the bits so you can access the next bit to the left
                                        }
                                        currColor = encodeColor.green;
                                        break;
                                    case encodeColor.green:
                                        if (hidingText)
                                        {
                                            g += (currChar & 0x00000001) | (g & 0x00000001);
                                            currChar >>= 1;
                                        }
                                        currColor = encodeColor.blue;
                                        break;
                                    case encodeColor.blue:
                                        if (hidingText)
                                        {
                                            b += (currChar & 0x00000001) | (b & 0x00000001);
                                            currChar >>= 1;
                                        }
                                        currColor = encodeColor.red;
                                        inBitmap.SetPixel(w, h, Color.FromArgb(r, g, b)); //set the pixel and move on
                                        break;
                                }
                                pixelIndex++;
                                if (endMessage)
                                {
                                    endZeros++;
                                }
                            }


                        }
                        if (finished)
                        {
                            Console.WriteLine("Encryption successful");
                            Bitmap outBitmap = new Bitmap(inBitmap);
                            inBitmap.Dispose();
                            filename = Regex.Replace(filename, @"\..+", ".png").ToString();
                            outBitmap.Save(("encrypted_" + filename), ImageFormat.Png);
                            outBitmap.Dispose();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\nUnable to encrypt text into given file.");
                }
            }
            else
            {
                Console.WriteLine("Unable to find file.");
            }

        }

        static public void Decrypt(string filename)
        {
            if (File.Exists(filename))
            {
                encodeColor currColor = encodeColor.red;
                Bitmap inBitmap = new Bitmap(filename);
                int bitsDone = 0;
                int currByte = 0;
                string message = String.Empty;
                bool finished = false;


                for (int w = 0; w < inBitmap.Width; w++)
                {
                    for (int h = 0; h < inBitmap.Height; h++)
                    {
                        Color pixel = inBitmap.GetPixel(w, h);

                        for (int i = 0; i < 3; i++)
                        {
                            //this is to add the bits to the currB
                            switch (currColor)
                            {
                                case encodeColor.red:
                                    //shift left 1 bit, opens LSB, and then write R text to it
                                    currByte = currByte * 2 + pixel.R % 2;
                                    currColor = encodeColor.green;
                                    break;
                                case encodeColor.green:
                                    //shift left 1 bit, opens up LSB, then write G text to it
                                    currByte = currByte * 2 + pixel.G % 2;
                                    currColor = encodeColor.blue;
                                    break;
                                case encodeColor.blue:
                                    //shift left 1 bit, opens up LSB, then write B text to it
                                    currByte = currByte * 2 + pixel.B % 2;
                                    currColor = encodeColor.red;
                                    break;
                            }

                            bitsDone++;
                            if (bitsDone % 8 == 0)
                            {
                                currByte = reverseBits(currByte);

                                if (currByte == 0)
                                {
                                    finished = true;
                                    break;
                                }

                                message += ((char)currByte).ToString();
                            }
                        }
                        if (finished)
                        {
                            break;
                        }
                    }
                    if (finished)
                    {
                        break;
                    }
                }
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine("Unable to open file.");
            }
        }

        static private int reverseBits(int n)
        {
            int reversed = 0;
            for(int i = 0; i < 8; i++)
            {
                reversed = reversed * 2 + n % 2;
                n /= 2;
            }
            return reversed;
        }
    }
}
