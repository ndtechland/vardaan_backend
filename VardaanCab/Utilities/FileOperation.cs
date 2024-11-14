using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VardaanCab.Utilities 
{
    public class FileOperation
    {
        public static string UploadImage(HttpPostedFileBase File, string folderName)
        {
            var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".JPEG", ".JPG", ".PNG" };
            return UploadFile(File, folderName, allowedExtensions);
        }

        public static string UploadPdf(HttpPostedFileBase File, string folderName)
        {
            var allowedExtensions = new[] {".pdf",".PDF",".jpeg", ".jpg", ".png", ".JPEG", ".JPG", ".PNG"};
            return UploadFile(File, folderName, allowedExtensions);
        }

        private static string UploadFile(HttpPostedFileBase File, string folderName, string[] allowedExtensions)
        {
            DateTime dt = DateTime.Now;
            //var allowedExtensions = new[] { ".pdf" };

            string savedFileName = "" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second + dt.Millisecond + Guid.NewGuid();

            string ImageName = Path.GetFileName(File.FileName);


            string ext = Path.GetExtension(ImageName);

            string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + folderName + "/" + savedFileName + ext);

            if (!allowedExtensions.Contains(ext))
            {
                return "not allowed";
            }
            // save image in folder
            File.SaveAs(physicalPath);
            return savedFileName + ext;
        }

        public static bool DeleteFile(string folder,string fileName)
        {
            string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~/" + folder + "/" + fileName);
            System.IO.FileInfo file = new System.IO.FileInfo(physicalPath);
            if (file.Exists)
            {
                file.Delete();
                return true;
            }
            return false;
        }

        public static string UploadFileWithBase64(string folderName, string imageName, string base64Image, string[] extensions)
        {
            byte[] imageBytes = Convert.FromBase64String(base64Image);
            string[] arr = imageName.Split('.');
            string ext = "." + arr[1];
            if (!extensions.Contains(ext))
                return "not allowed";
            string fileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + Guid.NewGuid().ToString();
            var filePath = HttpContext.Current.Server.MapPath("~/" + folderName);
            File.WriteAllBytes(filePath + "/" + fileName + ext, imageBytes);
            return fileName + ext;
        }

    }
}