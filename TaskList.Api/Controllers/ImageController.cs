using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;
using TaskList.Data.Interfaces;
using TaskList.Data;
using Microsoft.AspNet.Identity;
using E9.Common;

namespace RepsUK.Web.Controllers
{
    public class ImageController : ApiController
    {
        public IUnitOfWork Uow;
        public static Dictionary<string, byte[]> imageCache = new Dictionary<string, byte[]>();

        public ImageController()
            : this(UnitOfWork.Instantiate())
        {

        }

        public ImageController(IUnitOfWork uow)
        {
            Uow = uow;
        }

        public Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/Uploads/Temp");
            var provider = new MultipartFormDataStreamProvider(root);
            var CurrentContext = HttpContext.Current;

            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<IHttpActionResult>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    if (provider.FileData.Count > 0)
                    {
                        var file = provider.FileData[0];
                        var raw = file.LocalFileName;
                        var index = raw.IndexOf("Uploads");
                        var extension = file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf('.')).TrimEnd('\"');
                        var path = raw.Substring(index) + extension;
                        File.Move(raw, Path.ChangeExtension(raw, extension));
                        imageCache = new Dictionary<string, byte[]>();
                        return Ok(path);
                    }
                    else
                    {
                        return BadRequest();
                    }
                });

            return task;
        }

        // GET api/<controller>
        public async Task<IHttpActionResult> Post(string ownerType, int ownerId)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string temp = HttpContext.Current.Server.MapPath("~/Uploads/Temp");
            var provider = new MultipartFormDataStreamProvider(temp);
            var CurrentContext = HttpContext.Current;
            var task = await Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<IHttpActionResult>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    if (provider.FileData.Count > 0)
                    {
                        var file = provider.FileData[0];
                        var raw = file.LocalFileName;
                        var extension = file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf('.')).TrimEnd('\"');
                        var index = raw.IndexOf("Uploads");
                        var filename = raw.Substring(index) + extension;
                        Directory.CreateDirectory(CurrentContext.Server.MapPath("~/Uploads") + "/" + ownerType + "/" + ownerId);
                        string newLocation = CurrentContext.Server.MapPath("~/Uploads/" + ownerType + "/" + ownerId);
                        var guid = Guid.NewGuid();
                        string newFullPath = "Uploads/" + ownerType + "/" + ownerId + "/" + guid;
                        File.Move(raw, Path.ChangeExtension(newLocation + "/" + guid, extension));

                        switch (ownerType.ToUpper())
                        {
                            /*case "HOTEL":
                                var hotel = Uow.Hotels.Find(ownerId);
                                if (hotel == null)
                                    throw new ArgumentException("ownerId not a valid hotel", "ownerId");
                                hotel.LogoUrlRaw = newFullPath + extension;
                                break;
                            case "AIRLINE":
                                var airline = Uow.Airlines.Find(ownerId);
                                if (airline == null)
                                    throw new ArgumentException("ownerId not a valid airline", "ownerId");
                                airline.LogoUrlRaw = newFullPath + extension;
                                break;
                            case "HANDLER":
                                var handler = Uow.Handlers.Find(ownerId);
                                if (handler == null)
                                    throw new ArgumentException("ownerId not a valid handler", "ownerId");
                                handler.LogoUrlRaw = newFullPath + extension;
                                break;*/
                            default:
                                throw new ArgumentException("OwnerType not recognised", "ownerType");
                        }

                        Uow.Commit(User.Identity.GetUserId());

                        string cacheKey = "PROFILE" + ownerType + ownerId;
                        imageCache.Remove(cacheKey);

                        return Ok(newFullPath + extension);
                    }
                    else
                    {
                        return BadRequest();
                    }
                });

            return task;
        }

        public async Task<IHttpActionResult> Post(string ownerType, int ownerId, int cropx, int cropy, int cropx2, int cropy2, int height, int width)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string temp = HttpContext.Current.Server.MapPath("~/Uploads/Temp");
            var provider = new MultipartFormDataStreamProvider(temp);
            var CurrentContext = HttpContext.Current;
            var task = await Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<IHttpActionResult>(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    if (provider.FileData.Count > 0)
                    {
                        var file = provider.FileData[0];
                        var raw = file.LocalFileName;
                        var extension = file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf('.')).TrimEnd('\"');
                        var index = raw.IndexOf("Uploads");
                        var filename = raw.Substring(index) + extension;
                        Directory.CreateDirectory(CurrentContext.Server.MapPath("~/Uploads") + "/" + ownerType + "/" + ownerId);
                        string newLocation = CurrentContext.Server.MapPath("~/Uploads/" + ownerType + "/" + ownerId);
                        var guid = Guid.NewGuid();
                        string newFullPath = "Uploads/" + ownerType + "/" + ownerId + "/" + guid;
                        Bitmap img = new Bitmap(raw);
                        Rectangle srcRect = new Rectangle(cropx, cropy, cropx2 - cropx, cropy2 - cropy);
                        Bitmap cropped = (Bitmap)img.Clone(srcRect, img.PixelFormat);
                        cropped.Save(newLocation + "/" + guid + extension);
                        img.Dispose();
                        cropped.Dispose();

                        if (ownerId != 0)
                        {
                            switch (ownerType.ToUpper())
                            {
                                /*case "HOTEL":
                                    var hotel = Uow.Hotels.Find(ownerId);
                                    if (hotel == null)
                                        throw new ArgumentException("ownerId not a valid hotel", "ownerId");
                                    hotel.LogoUrlRaw = newFullPath + extension;
                                    break;
                                case "AIRLINE":
                                    var airline = Uow.Airlines.Find(ownerId);
                                    if (airline == null)
                                        throw new ArgumentException("ownerId not a valid airline", "ownerId");
                                    airline.LogoUrlRaw = newFullPath + extension;
                                    break;
                                case "HANDLER":
                                    var handler = Uow.Handlers.Find(ownerId);
                                    if (handler == null)
                                        throw new ArgumentException("ownerId not a valid handler", "ownerId");
                                    handler.LogoUrlRaw = newFullPath + extension;
                                    break;*/
                                default:
                                    throw new ArgumentException("OwnerType not recognised", "ownerType");
                            }

                            Uow.Commit(User.Identity.GetUserId());

                            string cacheKey = "PROFILE" + ownerType + ownerId;
                            imageCache.Remove(cacheKey);
                        }

                        return Ok(newFullPath + extension);
                    }
                    else
                    {
                        return BadRequest();
                    }
                });

            return task;
        }


        private static Font TextFont = new Font("Montserrat", 84f, FontStyle.Regular);
        private static Color TextColor = Color.White;

        public HttpResponseMessage Get(string imageType, string ownerType, int ownerId)
        {
            byte[] content = null;
            string cacheKey = imageType.ToUpper() + ownerType.ToUpper() + ownerId;
            if (imageCache.ContainsKey(cacheKey))
            {
                content = imageCache[cacheKey];
            }
            else
            {
                Image result = null;
                Color? background = null;
                String text = null;
                String imageUrl = null;

                switch (ownerType.ToUpper())
                {
                    /*case "HOTEL":
                        var hotel = Uow.Hotels.Find(ownerId);
                        if (hotel == null)
                            throw new ArgumentException("ownerId not a valid hotel", "ownerId");
                        text = hotel.Name;
                        imageUrl = hotel.LogoUrlRaw;
                        break;
                    case "AIRLINE":
                        var airline = Uow.Airlines.Find(ownerId);
                        if (airline == null)
                            throw new ArgumentException("ownerId not a valid airline", "ownerId");
                        text = airline.Name;
                        imageUrl = airline.LogoUrlRaw;
                        break;
                    case "HANDLER":
                        var handler = Uow.Handlers.Find(ownerId);
                        if (handler == null)
                            throw new ArgumentException("ownerId not a valid handler", "ownerId");
                        text = handler.Name;
                        imageUrl = handler.LogoUrlRaw;
                        break;
                    case "COMPANY":
                        var company = Uow.Companies.Find(ownerId);
                        if (company == null)
                            throw new ArgumentException("ownerId not a valid company", "ownerId");
                        text = company.Name;
                        imageUrl = company.LogoUrlRaw;
                        break;*/
                    default:
                        throw new ArgumentException("OwnerType not recognised", "ownerType");
                }

                switch (imageType.ToUpper())
                {
                    case "RAW":
                        if (imageUrl != null && File.Exists(HttpRuntime.AppDomainAppPath + imageUrl))
                        {
                            result = Image.FromFile(HttpRuntime.AppDomainAppPath + imageUrl);
                        }
                        break;
                    case "PROFILE":
                        background = GetColor(text);
                        text = text.GetInitials().ToUpper();
                        result = GenerateProfileImage(imageUrl, text, background);
                        break;
                }
                MemoryStream ms = new MemoryStream();
                result.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                content = ms.ToArray();
                //imageCache.Add(cacheKey, content);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(content);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            return response;
        }

        private Color GetColor(string text)
        {
            double sum = 1 + text.Sum(x => x);
            var col = (sum % 33);
            return new HSLColor(col * 7.5, 240 * 0.55, 240 * 0.55);
        }

        private Image GenerateProfileImage(string imageURL, string text, Color? backColor)
        {
            Bitmap img = new Bitmap(250, 250);
            Graphics drawing = Graphics.FromImage(img);
            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color InsideColor = (backColor.HasValue) ? backColor.Value : Color.Black;
            Color TextColor = Color.White;

            drawing.Clear(InsideColor);

            if (imageURL != null && File.Exists(HttpRuntime.AppDomainAppPath + imageURL))
            {
                Image picture = Image.FromFile(HttpRuntime.AppDomainAppPath + imageURL);
                drawing.DrawImage(picture, 0, 0, 250, 250);
            }
            else if (text != null)
            {
                SizeF textSize = drawing.MeasureString(text, TextFont);

                Brush TextBrush = new SolidBrush(TextColor);
                drawing.DrawString(text, TextFont, TextBrush, 125 - (textSize.Width / 2), 125 - (textSize.Height / 2));
            }

            drawing.Save();

            return img;
        }
    }
}