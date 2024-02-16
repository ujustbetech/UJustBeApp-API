using System;
using System.Collections.Generic;
using System.IO;


namespace UJBHelper.Common
{
    public class Utility
    {
        public static string UploadFilebytes(Byte[] bytes, string FileName, string fileDestination)
        {
         
            string fileName = string.Empty;
            string extn = string.Empty;
            string uploadFolderPath = fileDestination;
            //string uploadFolderPath = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings[fileDestination].ToString());
            extn = Path.GetFileName(FileName.Substring(FileName.LastIndexOf('.') + 1));
            fileName = string.Concat (DateTime.Now.ToString("ddMMyyyyHHmmssffff") + '.' + extn);

            if (!Directory.Exists(uploadFolderPath))
            {
                Directory.CreateDirectory(uploadFolderPath);
            }
            var fileUploadFullPath = Path.Combine(uploadFolderPath, fileName);
            System.IO.File.WriteAllBytes(fileUploadFullPath, bytes);
            return fileName;
        }

        public static List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();
            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }
            return randomList; //return the new random list
        }
    }
}
