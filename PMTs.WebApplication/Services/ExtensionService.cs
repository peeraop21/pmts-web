using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PMTs.DataAccess.Tracing;
using PMTs.WebApplication.Services.Interfaces;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services
{
    [TraceAspect]
    public class ExtensionService : IExtensionService
    {
        IHostingEnvironment _hostingEnvironment;

        public ExtensionService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string[] UploadPicture(IFormFile Picture)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "Picture");
            string[] result = new string[3];
            string fileName = null;
            string newFileName = null;
            string base64String = null;
            try
            {

                if (Picture == null)
                {
                    result[0] = newFileName;
                    result[1] = base64String;
                    return result;
                }
                //Create Folder 
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }

                if (Picture != null)
                {
                    //Getting FileName
                    fileName = ContentDispositionHeaderValue.Parse(Picture.ContentDisposition).FileName.Trim('"');

                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var FileExtension = Path.GetExtension(fileName);

                    // concating  FileName + FileExtension
                    newFileName = myUniqueFileName + FileExtension;



                    // Combines two strings into a path.
                    fileName = path + $@"\{newFileName}";

                    // if you want to store path of folder in database

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        //Picture.CopyToAsync(fs);
                        Picture.CopyTo(fs);

                        //   fs.Flush();


                    }

                    Byte[] bytes = File.ReadAllBytes(fileName);
                    string src = "data:" + Picture.ContentType + ";base64,";
                    base64String = src + Convert.ToBase64String(bytes);

                }


                result[0] = newFileName;
                result[1] = base64String;
                return result;
            }
            catch (Exception ex)
            {
                return result = null;
            }

        }

        public string Base64String(IHostingEnvironment environmentstring, string fileName)
        {
            string base64String;

            try
            {
                var path = Path.Combine(environmentstring.WebRootPath, "Picture");
                string FullfileName, type;
                Byte[] bytes;
                string src;
                int len;
                FullfileName = path + $@"\{fileName}";
                type = Path.GetExtension(fileName);
                len = type.Length;
                src = "data:image/" + type.Substring(1, len - 1) + ";base64,";
                bytes = File.ReadAllBytes(FullfileName);
                base64String = src + Convert.ToBase64String(bytes);

            }
            catch (Exception ex)
            {
                base64String = null;
            }
            return base64String;

        }

        public string DecryptPassKiwi(string cipherString)
        {
            try
            {
                byte[] keyArray;

                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(System.Text.UTF8Encoding.UTF8.GetBytes("PMTs"));
                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();

                return System.Text.UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
                // log error
            }
        }

        public string ConvertImageToBase64(IFormFile image)
        {
            // Convert to Bytes
            byte[] byteImage = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            byteImage = reader.ReadBytes((int)image.Length);

            // Convert to Base64String
            string base64StringImage = "data:" + image.ContentType + ";base64," + Convert.ToBase64String(byteImage);
            return base64StringImage;
        }

        public void DeleteFile(string Path)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(Path);
            try
            {
                fi.Delete();
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public string GetLogId()
        {
            Random generatorLogId = new Random();
            String logId = generatorLogId.Next(0, 9999999).ToString("D7");

            return logId;
        }

        public string ResizeImageRatio(IFormFile file, int maxWidth, int maxHeight)
        {
            if (file is not null && !file.ContentType.StartsWith("image/")) return string.Empty;
            MemoryStream input = new MemoryStream();
            MemoryStream output = new MemoryStream();
            if (file != null)
            {
                file.CopyTo(input);

                if (file.FileName.Contains(".gif") || file.FileName.Contains(".GIF")) return "data:image/jpg;base64," + Convert.ToBase64String(input.ToArray());

                using (Image sourceImage = Image.FromStream(input))
                {
                    int newWidth, newHeight;
                    double aspectRatio = (double)sourceImage.Width / sourceImage.Height;

                    if (sourceImage.Width > maxWidth || sourceImage.Height > maxHeight)
                    {
                        if (aspectRatio > 1)
                        {
                            newWidth = maxWidth;
                            newHeight = (int)(maxWidth / aspectRatio);
                        }
                        else
                        {
                            newHeight = maxHeight;
                            newWidth = (int)(maxHeight * aspectRatio);
                        }
                        using (Bitmap resizedImage = new Bitmap(newWidth, newHeight))
                        using (Graphics graphic = Graphics.FromImage(resizedImage))
                        {
                            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            graphic.DrawImage(sourceImage, 0, 0, newWidth, newHeight);

                            resizedImage.Save(output, sourceImage.RawFormat);
                        }
                        var dataUrl = "data:image/jpg;base64," + Convert.ToBase64String(output.ToArray());
                        return dataUrl;
                    }
                    else
                    {
                        var dataUrl = "data:image/jpg;base64," + Convert.ToBase64String(input.ToArray());
                        return dataUrl;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public void UploadImage(string base64String, string pathTarget)
        {
            string directoryPath = Path.GetDirectoryName(pathTarget);

            if (!(Directory.Exists(directoryPath))) Directory.CreateDirectory(directoryPath);

            byte[] bytes = Convert.FromBase64String(base64String);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (Bitmap bm = new Bitmap(ms))
                {
                    bm.Save(pathTarget);
                }
            }
        }

        private static Bitmap ResizeImage(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(resizedImage))
            {
                gfx.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            return resizedImage;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }
    }
}
