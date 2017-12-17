using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Jqpress.Framework.Utils
{
    /// <summary>
    ///水印处理程序
    /// </summary>
    public class Watermark
    {
        private Watermark()
        {

        }

        /// <summary>
        /// 添加图片水印
        /// </summary>
        /// <param name="oldFilePath">原始图片路径</param>
        /// <param name="newFilePath">将要添加水印图片路径</param>
        /// <param name="waterPosition">水印位置</param>
        /// <param name="waterImagePath">水印图片路径</param>
        /// <param name="transparency">透明度</param>
        /// <param name="quality">质量</param>
        public static void CreateWaterImage(string oldFilePath, string newFilePath, int waterPosition, string waterImagePath, int watermarkTransparency, int quality)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(oldFilePath);

            Bitmap bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            g.DrawImage(image, 0, 0, image.Width, image.Height);

            //设置透明度
            System.Drawing.Image watermark = new Bitmap(waterImagePath);
            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
            {
                transparency = (watermarkTransparency / 10.0F);
            }

            float[][] colorMatrixElements = {
													new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
													new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
													new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
													new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f}, //注意：倒数第二处为0.0f为完全透明，1.0f为完全不透明
													new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
												};
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int _width = image.Width;
            int _height = image.Height;
            int xpos = 0;
            int ypos = 0;
            int WatermarkWidth = 0;
            int WatermarkHeight = 0;
            double bl = 1d;
            //计算水印图片的比率
            //取背景的1/4宽度来比较
            if ((_width > watermark.Width * 2) && (_height > watermark.Height * 2))
            {
                bl = 1;
            }
            else if ((_width > watermark.Width * 2) && (_height < watermark.Height * 2))
            {
                bl = Convert.ToDouble(_height / 2) / Convert.ToDouble(watermark.Height);

            }
            else if ((_width < watermark.Width * 2) && (_height > watermark.Height * 2))
            {
                bl = Convert.ToDouble(_width / 2) / Convert.ToDouble(watermark.Width);
            }
            else
            {
                if ((_width * watermark.Height) > (_height * watermark.Width))
                {
                    bl = Convert.ToDouble(_height / 2) / Convert.ToDouble(watermark.Height);
                }
                else
                {
                    bl = Convert.ToDouble(_width / 2) / Convert.ToDouble(watermark.Width);
                }
            }
            WatermarkWidth = Convert.ToInt32(watermark.Width * bl);
            WatermarkHeight = Convert.ToInt32(watermark.Height * bl);
            switch (waterPosition)
            {
                case 3:
                    xpos = _width - WatermarkWidth - 10;
                    ypos = 10;
                    break;
                case 2:
                    xpos = 10;
                    ypos = _height - WatermarkHeight - 10;
                    break;
                case 5:
                    xpos = _width / 2 - WatermarkWidth / 2;
                    ypos = _height / 2 - WatermarkHeight / 2;
                    break;
                case 1:
                    xpos = 10;
                    ypos = 10;
                    break;
                case 4:
                default:
                    xpos = _width - WatermarkWidth - 10;
                    ypos = _height - WatermarkHeight - 10;
                    break;
            }
            g.DrawImage(watermark, new Rectangle(xpos, ypos, WatermarkWidth, WatermarkHeight), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
            try
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                    {
                        ici = codec;
                    }
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];

                if (quality < 0 || quality > 100)
                {
                    quality = 80;
                }

                qualityParam[0] = quality;

                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                if (ici != null)
                {
                    bmp.Save(newFilePath, ici, encoderParams);
                }
                else
                {
                    bmp.Save(newFilePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watermark.Dispose();
                imageAttributes.Dispose();
                image.Dispose();
                bmp.Dispose();
            }
        }

        /// <summary>
        /// 添加文字水印
        /// </summary>
        /// <param name="oldFilePath">原始图片路径</param>
        /// <param name="newFilePath">将要添加水印图片路径</param>
        /// <param name="waterPosition">水印位置</param>
        /// <param name="waterText">水印内容</param>
        public static void CreateWaterText(string oldFilePath, string newFilePath, int waterPosition, string waterText, int quality, string fontname, int fontsize)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(oldFilePath);
            Bitmap bmp = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            g.DrawImage(image, 0, 0, image.Width, image.Height);

            int _width = bmp.Width;
            int _height = bmp.Height;

            Font crFont = new Font(fontname, fontsize, FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF crSize = g.MeasureString(waterText, crFont);

            float xpos = 0;
            float ypos = 0;
            switch (waterPosition)
            {
                case 3:
                    xpos = ((float)_width * (float).99) - (crSize.Width / 2);
                    ypos = (float)_height * (float).01;
                    break;
                case 2:
                    xpos = ((float)_width * (float).01) + (crSize.Width / 2);
                    ypos = ((float)_height * (float).99) - crSize.Height;
                    break;
                case 5:
                    xpos = ((_width - crSize.Width) / 2) + crSize.Width / 2;    //奇怪的表达式
                    ypos = (_height - crSize.Height) / 2 + crSize.Height / 2;
                    break;
                case 1:

                    xpos = ((float)_width * (float).01) + (crSize.Width / 2);
                    ypos = (float)_height * (float).01;
                    break;

                case 4:
                default:
                    xpos = ((float)_width * (float).99) - (crSize.Width / 2);
                    ypos = ((float)_height * (float).99) - crSize.Height;
                    break;
            }

            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            //可设置透明度
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));
            g.DrawString(waterText, crFont, semiTransBrush, xpos, ypos, StrFormat);

            try
            {
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                    {
                        ici = codec;
                    }
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];

                if (quality < 0 || quality > 100)
                {
                    quality = 80;
                }

                qualityParam[0] = quality;


                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                if (ici != null)
                {
                    bmp.Save(newFilePath, ici, encoderParams);
                }
                else
                {
                    bmp.Save(newFilePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                semiTransBrush.Dispose();
                image.Dispose();
                bmp.Dispose();
            }
        }
    }
}
