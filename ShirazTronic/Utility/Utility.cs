using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShirazTronic
{
    public static class Utility
    {
       
        public const string defaultProductImage = "default.jpg";
        /// <summary>
        /// Save Files based on Input and then return saved File's name
        /// </summary>
        /// <param name="iWebHostEnvironment"></param>
        /// <param name="iWWWRootDesireFolder">
        /// folder name in WWWRoot folder
        /// </param>
        /// <param name="iFile">
        /// pass file control to this parameter
        /// </param>
        /// <param name="fileNameTobeSaved">
        /// desire file name
        /// </param>
        /// <returns></returns>
        public static string SaveFileThenGetFileName(IWebHostEnvironment iWebHostEnvironment,string iWWWRootDesireFolder, IFormFileCollection iFile,string fileNameTobeSaved)
        {
            string webRootPath = iWebHostEnvironment.WebRootPath;
            string extention = ".jpg";
            var uploadPath = Path.Combine(iWebHostEnvironment.WebRootPath,"image", iWWWRootDesireFolder);
            if (iFile.Count > 0)
            {
                 extention = Path.GetExtension(iFile[0].FileName);
                var filenameComplete = fileNameTobeSaved + extention;
                using (var fileStream = new FileStream(Path.Combine(uploadPath, filenameComplete), FileMode.Create))
                {
                    iFile[0].CopyTo(fileStream);
                }
            }
            else
            {
                var defaultDocument= Path.Combine(uploadPath+"\\" + Utility.defaultProductImage);

                System.IO.File.Copy(defaultDocument,Path.Combine(uploadPath , fileNameTobeSaved+".jpg"));

            }
            return @"\image\products\" + fileNameTobeSaved+ extention;
           
        }
    }
}
