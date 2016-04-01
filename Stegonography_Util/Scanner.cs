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

        static public void ScanFile(string filepath)
        {
            byte[] fileBytes = null;
            int numFiles = 0;
            BinaryWriter bwNewImage = null;
            bool fileInProgress = false;
            bool thumbnailDone = false;
            try
            {
                if (File.Exists(filepath))
                {
                    //read file into byte array
                    fileBytes = File.ReadAllBytes(filepath);
                    for (int i = 0; i < fileBytes.Length; i++)
                    {

                        if (!fileInProgress)
                        {
                            //if an image is found:
                            if (fileBytes[i] == 0xFF && fileBytes[i + 1] == 0xD8 && fileBytes[i + 2] == 0xFF)
                            {
                                numFiles++;
                                string newFile = "Image_" + numFiles.ToString() + ".jpeg";
                                bwNewImage = new BinaryWriter(File.OpenWrite(newFile));
                                bwNewImage.Write(fileBytes[i]);
                                bwNewImage.Write(fileBytes[i + 1]);
                                bwNewImage.Write(fileBytes[i + 2]);
                                i += 2;
                                fileInProgress = true;
                                thumbnailDone = false;
                            }

                        }
                        if (fileInProgress)
                        {
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
