/*
* FILE : Scanner.cs
* PROJECT : ACS a3 CameraSteganography
* PROGRAMMER : Matt Warren & Steven Johnston
* FIRST VERSION : 2016-03-31
* DESCRIPTION :
* This class is used to scan a given .img file and recover the pictures from it.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Steganography_Util
{
    class Scanner
    {

        public Scanner()
        {

        }


        // DESCRIPTION :
        //              this function is used to scan an .img file a pull pictures found inside
        // PARAMETERS :
        //              string: filepath --- this is the filename/path of the photos to recover
        static public void ScanFile(string filepath)
        {
            //used to hold the .img file as bytes
            byte[] fileBytes = null;

            //counter for number of files
            int numFiles = 0;

            //writer for new images
            BinaryWriter bwNewImage = null;

            //if file is being written, true
            bool fileInProgress = false;

            //if thumbnail is finished, true
            bool thumbnailDone = false;


            try
            {
                if (File.Exists(filepath))
                {
                    //read file into byte array
                    fileBytes = File.ReadAllBytes(filepath);

                    //loop through byte array
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        
                        if (!fileInProgress)
                        {
                            //if an image is found (jpeg start with 0xFF then 0xD8, then the thumbnail is next (0xFF again))
                            if (fileBytes[i] == 0xFF && fileBytes[i + 1] == 0xD8 && fileBytes[i + 2] == 0xFF)
                            {
                                numFiles++;
                                //setup filename
                                string newFile = "Image_" + numFiles.ToString() + ".jpeg";
                                //write/create file
                                bwNewImage = new BinaryWriter(File.OpenWrite(newFile));
                                //add first 3 bytes from if statement
                                bwNewImage.Write(fileBytes[i]);
                                bwNewImage.Write(fileBytes[i + 1]);
                                bwNewImage.Write(fileBytes[i + 2]);
                                //increment past those
                                i += 2;
                                fileInProgress = true;
                                thumbnailDone = false;
                            }

                        }
                        if (fileInProgress)
                        {
                            //if end of the jpeg
                            if(fileBytes[i] == 0xFF && fileBytes[i+1] == 0xD9)
                            {
                                bwNewImage.Write(fileBytes[i]);
                                bwNewImage.Write(fileBytes[i + 1]);
                                i++;
                                if (!thumbnailDone)
                                {
                                    thumbnailDone = true;
                                }
                                else
                                {
                                    fileInProgress = false;
                                    bwNewImage.Close();
                                }
                            }
                            else
                            {
                                bwNewImage.Write(fileBytes[i]);
                            }
                        }
                        
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
