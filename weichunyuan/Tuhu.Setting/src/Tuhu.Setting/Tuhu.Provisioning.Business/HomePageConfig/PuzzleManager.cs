using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Net.Http;
using System.Text.RegularExpressions;
using Tuhu.Service.Utility.Request;
using System.Web.Configuration;
using System.Drawing.Imaging;

namespace Tuhu.Provisioning.Business
{
    public class PuzzleManager
    {

        public IEnumerable<SE_HomePageContentPuzzle> Entities { get; set; }

        public PuzzleManager(IEnumerable<SE_HomePageContentPuzzle> entities,int type)
        {
           
            Type = type;
            this.Entities = entities;
            this.Extend = GetExtend();
            GetPuzzleProperty();
        }

        private void GetPuzzleProperty()
        {
            int width = 0;
            int height = 0;
            if (this.Type == 1 || this.Type == 2 || this.Type == 3)
            {
                foreach (var item in Entities)
                {
                    var property = GetImageProperty(item.ImageUrl);
                    if (this.Type == 1 || this.Type == 2 || this.Type == 3)
                    {
                        width += property.Item1;
                        height = property.Item2;
                    }
                }
            }
            else if (this.Type == 4 || this.Type == 5)
            {
                var list = this.Entities.ToList();
                var property1 = GetImageProperty(list[0].ImageUrl);
                var property2 = GetImageProperty(list[1].ImageUrl);
                width = property1.Item1 + property2.Item1;
                height = property1.Item2;
            }
            //else if (this.Type == 3)
            //{
            //    var list = Entities.ToList();
            //    for (int i = 0; i <list.Count / 2; i++)
            //    {
            //        var item = list[i];
            //        var property = GetImageProperty(item.ImageUrl);
            //        width += property.Item1;
            //        height = property.Item2 * 2;
            //    }
            //}

            this.Width = width;
            this.Height = height;
           
        }


        public string CreatePuzzleUri()
        {
            if (Type == 1 || Type == 2 || Type == 3)
                return CreatePuzzleSequence(this.Entities);
            //if (this.Type == 3)
            //{
            //    return CreatePuzzleBigSix();
            //}
            if (this.Type == 4)
                return CreatePuzzleFour();
            if (this.Type == 5)
                return CreatePuzzleThird();
            return null;
        }


        public int Width { get; set; }

        public int Height { get; set; }

        public int Type { get; set; }

        public ImageFormat Extend { get; set; }

        /// <summary>
        /// 创建连续四个 三个的合成图片
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private string CreatePuzzleSequence(IEnumerable<SE_HomePageContentPuzzle> entities)
        {
            Bitmap bmp = new Bitmap(Width, Height);
           
            var first = GetImage(entities.Where(o => o.PriorityLevel == 1).FirstOrDefault().ImageUrl);
            var second = GetImage(entities.Where(o => o.PriorityLevel == 2).FirstOrDefault().ImageUrl);
            var third = GetImage(entities.Where(o => o.PriorityLevel == 3).FirstOrDefault().ImageUrl);
            if (entities.Count() == 3)
            {
                Task.WaitAll(first, second, third);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    Image firstImage = new Bitmap(first.Result);
                    g.DrawImage(firstImage, 0, 0, firstImage.Width, firstImage.Height);
                    Image secondImage = new Bitmap(second.Result);
                    g.DrawImage(secondImage, firstImage.Width, 0, secondImage.Width, secondImage.Height);
                    Image thirdImage = new Bitmap(third.Result);
                    g.DrawImage(thirdImage, firstImage.Width + secondImage.Width, 0, thirdImage.Width, thirdImage.Height);
                }
            }
            else
            {
                var fourth = GetImage(entities.Where(o => o.PriorityLevel == 4).FirstOrDefault().ImageUrl);
                Task.WaitAll(first, second, third,fourth);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    Image firstImage = new Bitmap(first.Result);
                    g.DrawImage(firstImage, 0, 0, firstImage.Width, firstImage.Height);
                    Image secondImage = new Bitmap(second.Result);
                    g.DrawImage(secondImage, firstImage.Width, 0, secondImage.Width, secondImage.Height);
                    Image thirdImage = new Bitmap(third.Result);
                    g.DrawImage(thirdImage, firstImage.Width + secondImage.Width, 0, thirdImage.Width, thirdImage.Height);
                    Image fourthImage = new Bitmap(fourth.Result);
                    g.DrawImage(fourthImage,( firstImage.Width + secondImage.Width + thirdImage.Width),0, fourthImage.Width, fourthImage.Height);
                }
            }

             var buffer = ImageToBytes(bmp);
          
            if (buffer == null || buffer.Count() == 0)
                return null;
            var result = ImageUploadToAli(buffer);
            return result;
        }

        /// <summary>
        /// 创建六大块图片
        /// </summary>
        /// <returns></returns>
        private string CreatePuzzleBigSix()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            var entities = this.Entities;
            var first = GetImage(entities.Where(o => o.PriorityLevel == 1).FirstOrDefault().ImageUrl);
            var second = GetImage(entities.Where(o => o.PriorityLevel == 2).FirstOrDefault().ImageUrl);
            var third = GetImage(entities.Where(o => o.PriorityLevel == 3).FirstOrDefault().ImageUrl);
            var fourth = GetImage(entities.Where(o => o.PriorityLevel == 4).FirstOrDefault().ImageUrl);
            var fifth = GetImage(entities.Where(o => o.PriorityLevel == 5).FirstOrDefault().ImageUrl);
            var sixth = GetImage(entities.Where(o => o.PriorityLevel == 6).FirstOrDefault().ImageUrl);
            Task.WaitAll(first, second, third, fourth, fifth, sixth);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Image firstImage = new Bitmap(first.Result);
                g.DrawImage(firstImage, 0, 0, firstImage.Width, firstImage.Height);
                Image secondImage = new Bitmap(second.Result);
                g.DrawImage(secondImage, firstImage.Width, 0, secondImage.Width, secondImage.Height);
                Image thirdImage = new Bitmap(third.Result);
                g.DrawImage(thirdImage, firstImage.Width + secondImage.Width, 0, thirdImage.Width, thirdImage.Height);

                Image fourthImage = new Bitmap(fourth.Result);
                g.DrawImage(fourthImage, 0, firstImage.Height, fourthImage.Width, fourthImage.Height);
                Image fifthImage = new Bitmap(fifth.Result);
                g.DrawImage(fifthImage, fourthImage.Width, firstImage.Height, fifthImage.Width, fifthImage.Height);
                Image sixthImage = new Bitmap(sixth.Result);
                g.DrawImage(sixthImage, fourthImage.Width + fifthImage.Width, firstImage.Height, sixthImage.Width, sixthImage.Height);

            }
            var buffer = ImageToBytes(bmp);

            if (buffer == null || buffer.Count() == 0)
                return null;
            var result = ImageUploadToAli(buffer);
            return result;
        }

        /// <summary>
        /// 大小组合四块
        /// </summary>
        /// <returns></returns>

        private string CreatePuzzleFour()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            var entities = this.Entities;
            var first = GetImage(entities.Where(o => o.PriorityLevel == 1).FirstOrDefault().ImageUrl);
            var second = GetImage(entities.Where(o => o.PriorityLevel == 2).FirstOrDefault().ImageUrl);
            var third = GetImage(entities.Where(o => o.PriorityLevel == 3).FirstOrDefault().ImageUrl);
            var fourth = GetImage(entities.Where(o => o.PriorityLevel == 4).FirstOrDefault().ImageUrl);
          
            Task.WaitAll(first, second, third, fourth);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Image firstImage = new Bitmap(first.Result);
                g.DrawImage(firstImage, 0, 0, firstImage.Width, firstImage.Height);

                Image secondImage = new Bitmap(second.Result);
                g.DrawImage(secondImage, firstImage.Width, 0, secondImage.Width, secondImage.Height);

                Image thirdImage = new Bitmap(third.Result);
                g.DrawImage(thirdImage, firstImage.Width , secondImage.Height, thirdImage.Width, thirdImage.Height);

                Image fourthImage = new Bitmap(fourth.Result);
                g.DrawImage(fourthImage, firstImage.Width+thirdImage.Width, secondImage.Height, fourthImage.Width, fourthImage.Height);
               

            }
            var buffer = ImageToBytes(bmp);

            if (buffer == null || buffer.Count() == 0)
                return null;
            var result = ImageUploadToAli(buffer);
            return result;
        }

        private string CreatePuzzleThird()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            var entities = this.Entities;
            var first = GetImage(entities.Where(o => o.PriorityLevel == 1).FirstOrDefault().ImageUrl);
            var second = GetImage(entities.Where(o => o.PriorityLevel == 2).FirstOrDefault().ImageUrl);
            var third = GetImage(entities.Where(o => o.PriorityLevel == 3).FirstOrDefault().ImageUrl);
          

            Task.WaitAll(first, second, third);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                Image firstImage = new Bitmap(first.Result);
                g.DrawImage(firstImage, 0, 0, firstImage.Width, firstImage.Height);

                Image secondImage = new Bitmap(second.Result);
                g.DrawImage(secondImage, firstImage.Width, 0, secondImage.Width, secondImage.Height);

                Image thirdImage = new Bitmap(third.Result);
                g.DrawImage(thirdImage, firstImage.Width, secondImage.Height, thirdImage.Width, thirdImage.Height);

            }
            var buffer = ImageToBytes(bmp);

            if (buffer == null || buffer.Count() == 0)
                return null;
            var result = ImageUploadToAli(buffer);
            return result;
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<Stream> GetImage(string uri)
        {
            int i = 1;
            Stream image = null;
            do
            {
                image = DownLoadFielToMemoryStream(uri);
            } while (i <= 3 && image == null);
            return image;
        }

        private  MemoryStream DownLoadFielToMemoryStream(string url)
        {
            var wreq = HttpWebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = wreq.GetResponse() as HttpWebResponse;
            MemoryStream ms = null;
            using (var stream = response.GetResponseStream())
            {
                Byte[] buffer = new Byte[response.ContentLength];
                int offset = 0, actuallyRead = 0;
                do
                {
                    actuallyRead = stream.Read(buffer, offset, buffer.Length - offset);
                    offset += actuallyRead;
                }
                while (actuallyRead > 0);
                ms = new MemoryStream(buffer);
            }
            response.Close();
            return ms;
        }


        private Tuple<int, int> GetImageProperty(string uri)
        {
            Regex regex = new Regex(@"_w\d+_");
            var match = regex.Match(uri);
            int width = Convert.ToInt32(match.Value.Substring(2, match.Value.Length - 3));
            regex = new Regex(@"_h\d+.");
            match = regex.Match(uri);
            int height = Convert.ToInt32(match.Value.Substring(2, match.Value.Length - 3));
            return Tuple.Create<int, int>(width, height);
        }

        private void CreatePuzzle(Graphics g, Bitmap source, int x, int y, int width, int height)
        {
            g.DrawImage(source, x, y, width, height);
        }

        private string ImageUploadToAli(byte[] buffer)
        {
            string _BImage = string.Empty;
            string _SImage = string.Empty;
            string _ImgGuid = Guid.NewGuid().ToString();
            Exception ex = null;
            if (buffer.Count() > 0)
            {
                try
                {
                    using (var client = new Tuhu.Service.Utility.FileUploadClient())
                    {
                        string dirName = System.Web.Configuration.WebConfigurationManager.AppSettings["UploadDoMain_image"];
                        var result = client.UploadImage(new ImageUploadRequest(dirName, buffer));
                        result.ThrowIfException(true);
                        if (result.Success)
                        {
                            _BImage = result.Result;
                            //_SImage= ImageHelper.GetImageUrl(result.Result, 100);
                        }
                    }
                }
                catch (Exception error)
                {
                    ex = error;
                }
            }
            string imgUrl = WebConfigurationManager.AppSettings["DoMain_image"] + _BImage;

            return imgUrl;
        }


        private  byte[] ImageToBytes(Image image)
        {
            ImageFormat format = this.Extend;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public Tuple<int, int, int, int> GetPostion(string uri,int priorityLevel)
        {
            #region 连续的
            if (this.Type == 1 || this.Type == 2)
            {
                var list = this.Entities.ToList();
                int width = 0;
                int indexWidth = 0;
                int height = 0;
                for (int i = priorityLevel-1; i > 0; i--)
                {
                    var item = GetImageProperty(list[i-1].ImageUrl);
                    indexWidth += item.Item1;
                }

                var self = GetImageProperty(list[priorityLevel - 1].ImageUrl);
                width = self.Item1;
                height = self.Item2;

                int upperleftX = Convert.ToInt32(( indexWidth*1.0 / this.Width * 1.0) * 100);
                int lowerRightX = Convert.ToInt32(((indexWidth+width)*1.00 / this.Width*1.0)*100);
                int lowerRightY = Convert.ToInt32((height*1.0 / this.Height*1.0)*100);
                return Tuple.Create<int, int, int, int>(upperleftX, 0,lowerRightX, lowerRightY);
                
            }
            #endregion
            #region 六大块
            if (this.Type == 3)
            {
                if (priorityLevel <= 3)
                {
                    var list = this.Entities.ToList();
                    int width = 0;
                    int indexWidth = 0;
                    int height = 0;
                    for (int i = priorityLevel - 1; i > 0; i--)
                    {
                        var item = GetImageProperty(list[i-1].ImageUrl);
                        indexWidth += item.Item1;
                    }

                    var self = GetImageProperty(list[priorityLevel - 1].ImageUrl);
                    width = self.Item1;
                    height = self.Item2;

                    int upperleftX = Convert.ToInt32((indexWidth * 1.0 / this.Width * 1.0) * 100);
                    int lowerRightX = Convert.ToInt32(((indexWidth + width) * 1.00 / this.Width * 1.0) * 100);
                    int lowerRightY = Convert.ToInt32((height * 1.0 / this.Height * 1.0) * 100);
                    return Tuple.Create<int, int, int, int>(upperleftX, 0, lowerRightX, lowerRightY);
                }
                else
                {
                    var list = this.Entities.ToList();
                    int width = 0;
                    int indexWidth = 0;
                    int indexHeight = GetImageProperty(list[0].ImageUrl).Item2;
                    int height = 0;
                    for (int i = priorityLevel - 4; i > 0; i--)
                    {
                        var item = GetImageProperty(list[i-1].ImageUrl);
                        indexWidth += item.Item1;
                        indexHeight = item.Item2;
                    }

                    var self = GetImageProperty(list[priorityLevel - 1].ImageUrl);
                    width = self.Item1;
                    height = self.Item2;

                    int upperleftX = Convert.ToInt32((indexWidth * 1.0 / (this.Width * 1.0)) * 100);
                    int upperleftY = Convert.ToInt32((indexHeight /( this.Height * 1.0)) * 100);
                    int lowerRightX = Convert.ToInt32(((indexWidth + width) * 1.00 / (this.Width * 1.0)) * 100);
                    int lowerRightY = Convert.ToInt32((( indexHeight+ height) * 1.0 / (this.Height * 1.0)) * 100);
                    return Tuple.Create<int, int, int, int>(upperleftX, upperleftY, lowerRightX, lowerRightY);
                }
            }
            #endregion

            #region 一大两小
            if (this.Type == 5)
            {
                if(priorityLevel == 1)
                    return Tuple.Create<int, int, int, int>(0, 0, 40, 100);
                if(priorityLevel == 2)
                    return Tuple.Create<int, int, int, int>(40, 0, 100, 50);
                if(priorityLevel == 3)
                    return Tuple.Create<int, int, int, int>(40, 50, 100, 100);
            }
            #endregion
            #region  两大两小
            if (this.Type == 4)
            {
                if (priorityLevel == 1)
                    return Tuple.Create<int, int, int, int>(0, 0, 40, 100);
                if(priorityLevel == 2)
                    return Tuple.Create<int, int, int, int>(40, 0, 100, 50);
                if(priorityLevel == 3)
                    return Tuple.Create<int, int, int, int>(40, 50, 70, 100);
                if(priorityLevel == 4)
                    return Tuple.Create<int, int, int, int>(70, 50, 100, 100);
            }
            #endregion
            return Tuple.Create<int, int, int, int>(-1, -1, -1, -1);
        }

        private ImageFormat GetExtend()
        {
            Regex regex = new Regex(@"_h\d+.*");
            var match = regex.Match(this.Entities.FirstOrDefault().ImageUrl);
            var extend= match.Value.Split('.')[1];
            if (extend.ToLower().Contains("jpg") || extend.ToLower().Contains("jpeg"))
                return ImageFormat.Jpeg;
            if (extend.ToLower().Contains("png"))
                return ImageFormat.Png;
            return ImageFormat.Jpeg;
        }

    }
}
