using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public class ConvertPictureToBase64
    {
        public static string _ConvertPictureToBase64(string path)
        {
            try
            {
                Bitmap bitmap1 = new Bitmap(path);
                MemoryStream streamQR = new MemoryStream();
                bitmap1.Save(streamQR, System.Drawing.Imaging.ImageFormat.Jpeg);
                var imgStr = "data:image/jpg;base64," + Convert.ToBase64String(streamQR.ToArray());
                bitmap1.Dispose();
                return imgStr;
            }
            catch
            {
                return "";
            }
        }
    }
}
