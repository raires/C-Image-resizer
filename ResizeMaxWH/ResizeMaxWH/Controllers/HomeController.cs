using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using System.IO;
using System.Drawing;
using System.Drawing.Imaging;



namespace ResizeMaxWH.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //
        // POST: /Home/
        [HttpPost]
        public ActionResult Index(string imagem)
        {
            try
            {
                string[] keys = Request.Form.AllKeys;
                for (int i = 0; i < keys.Length; i++)
                {
                    Console.Write(keys[i] + ": " + Request.Form[keys[i]] + "<br>");
                }
                int filesSaved = 0;
                HttpPostedFileBase filePosted = Request.Files["file"];
                if (Request.Files.Count > 0 && int.Parse(Request.Form["w"]) > 0 && int.Parse(Request.Form["h"]) > 0)
                {
                    HttpPostedFileBase file = Request.Files[filesSaved];
                    //Save File
                    if (file.ContentLength > 0)
                    {
                        byte[] thePictureAsBytes = new byte[file.ContentLength];
                        using (BinaryReader theReader = new BinaryReader(file.InputStream))
                        {
                            thePictureAsBytes = theReader.ReadBytes(file.ContentLength);
                        }                        
                        //Reduce the size of image
                        thePictureAsBytes = ImageFillerMaxWH(thePictureAsBytes, int.Parse(Request.Form["w"]), int.Parse(Request.Form["h"]));
                        string thePictureDataAsStringAfter = Convert.ToBase64String(thePictureAsBytes);
                        ViewBag.imgUploadedAfter = null;
                        if (thePictureDataAsStringAfter != null && thePictureDataAsStringAfter != "")
                        {
                            ViewBag.imgUploadedAfter = "data:image;base64," + thePictureDataAsStringAfter;
                            ViewBag.width =int.Parse(Request.Form["w"]);
                            ViewBag.height = int.Parse(Request.Form["h"]);
                        }
                    }
                    return View();
                }
                else
                {
                    ViewBag.NoResult = true;
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        


        public static byte[] ImageFillerMaxWH(byte[] byteImageIn, int Width, int Height)
        {
            byte[] currentByteImageArray = byteImageIn;
            try
            {
                double scale = 1f;
                MemoryStream inputMemoryStream = new MemoryStream(byteImageIn);
                Image fullsizeImage = null;
                using (fullsizeImage = Image.FromStream(inputMemoryStream))
                {
                    var ratioX = (double)Width / fullsizeImage.Width;
                    var ratioY = (double)Height / fullsizeImage.Height;
                    var ratio = Math.Min(ratioX, ratioY);
                    scale = ratio;
                    Bitmap fullSizeBitmap = null;
                    MemoryStream resultStream = new MemoryStream();
                    //will reduce the image size only if some of the sizes (W/H) is bigger than some of the setting parameters
                    if (fullsizeImage.Width > Width || fullsizeImage.Height > Height)
                    {
                        using (fullSizeBitmap = new Bitmap(fullsizeImage, new Size((int)(fullsizeImage.Width * scale), (int)(fullsizeImage.Height * scale))))
                        {
                            Image imageOverlay = fullsizeImage;
                            Image img = new Bitmap(Width, Height);
                            using (Graphics gr = Graphics.FromImage(img))
                            {
                                gr.DrawImage(fullSizeBitmap, new Point(0, 0));
                            }
                            img.Save(resultStream, ImageFormat.Png);
                            currentByteImageArray = resultStream.ToArray();
                        };

                        resultStream.Dispose();
                        resultStream.Close();
                    }
                    else if (fullsizeImage.Width <= Width && fullsizeImage.Height <= Height)
                    {
                        Image imageOverlay = fullsizeImage;
                        Image img = new Bitmap(Width, Height);
                        using (Graphics gr = Graphics.FromImage(img))
                        {
                            gr.DrawImage(imageOverlay, new Point(0, 0));
                        }
                        img.Save(resultStream, ImageFormat.Png);
                        currentByteImageArray = resultStream.ToArray();
                        resultStream.Dispose();
                        resultStream.Close();
                    }
                };
                inputMemoryStream.Dispose();
                inputMemoryStream.Close();
            }
            catch (System.Exception ex)
            {
                return null;
            }
            return currentByteImageArray;
        }

    }
}
