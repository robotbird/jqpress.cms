using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography;
using Jqpress.Framework.Configuration;



public partial class verifycode : System.Web.UI.Page
{

    private static byte[] randb = new byte[4];
    private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
    private Matrix m = new Matrix();
    private Bitmap charbmp = new Bitmap(30, 30);

    private Font[] fonts = {
                                        new Font(new FontFamily("Times New Roman"), 16 + Next(3), FontStyle.Regular),
                                        new Font(new FontFamily("Georgia"), 16 + Next(3), FontStyle.Regular),
                                        new Font(new FontFamily("Arial"), 16 + Next(3), FontStyle.Regular),
                                        new Font(new FontFamily("Comic Sans MS"), 16 + Next(3), FontStyle.Regular)
                                     };

    protected void Page_Load(object sender, System.EventArgs e)
    {
        this.CreateCheckCodeImage(GenerateCheckCode());
    }

    /// <summary>
    /// 生成随机数
    /// </summary>
    /// <returns></returns>
    private string GenerateCheckCode()
    {
      //  PageUtils.VerifyCode = checkCode.ToLower();


       

        int number;
        char code;
        object objcode = HttpContext.Current.Session[ConfigHelper.SitePrefix + "VerifyCode"];
        string checkCode = objcode!=null?objcode.ToString():"";

        if (!string.IsNullOrEmpty(checkCode))
        {
            return checkCode;
        }
        System.Random random = new Random();
        for (int i = 0; i < 4; i++)
        {
            number = random.Next();
            if (number % 2 == 0)
            {
                code = (char)('0' + (char)(number % 10));
            }
            else
            {
                //	code = (char)('a' + (char)(number % 26));
                code = (char)('0' + (char)(number % 10));
            }
            checkCode += code.ToString();
        }
        //     Response.Cookies.Add(new HttpCookie("VerifyCode", checkCode.ToLower()));
      //  Session[VerifyCode] = checkCode.ToLower();
        HttpContext.Current.Session[ConfigHelper.SitePrefix + "VerifyCode"] = checkCode.ToLower();
        return checkCode;
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <param name="checkCode"></param>
    private void CreateCheckCodeImage(string checkCode)
    {
        if (checkCode == null || checkCode.Trim() == String.Empty) return;

        System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 22.5)), 30);
        Graphics g = Graphics.FromImage(image);

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        g.Clear(Color.White);
        try
        {
            Random random = new Random();		//生成随机生成器
            g.Clear(Color.White);				//清空图片背景色
            for (int i = 0; i < 2; i++)				//画图片的背景噪音线
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);
                g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
            }

            //输出文字
            //Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
            //System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Gray, Color.Blue, 1.2f, true);
            //g.DrawString(checkCode, font, brush, 2, 2);

            Graphics charg = Graphics.FromImage(charbmp);

            SolidBrush drawBrush = new SolidBrush(Color.FromArgb(Next(100), Next(100), Next(100)));
            float charx = -18;
            for (int i = 0; i < checkCode.Length; i++)
            {
                m.Reset();
                m.RotateAt(Next(30) - 25, new PointF(Next(3) + 7, Next(3) + 7));

                charg.Clear(Color.Transparent);
                charg.Transform = m;
                //定义前景色为黑色
                drawBrush.Color = Color.Black;

                charx = charx + 20 + Next(2);
                PointF drawPoint = new PointF(charx, 0.1F);
                charg.DrawString(checkCode[i].ToString(), fonts[Next(fonts.Length - 1)], drawBrush, new PointF(0, 0));

                charg.ResetTransform();

                g.DrawImage(charbmp, drawPoint);
            }


            //画图片的前景噪音点
            for (int i = 0; i < 25; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);

                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            //画图片的边框线
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

            //输出
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            Response.ClearContent();
            Response.ContentType = "image/Gif";
            Response.BinaryWrite(ms.ToArray());
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
    }

    /// <summary>
    /// 获得下一个随机数
    /// </summary>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private static int Next(int max)
    {
        rand.GetBytes(randb);
        int value = BitConverter.ToInt32(randb, 0);
        value = value % (max + 1);
        if (value < 0)
            value = -value;
        return value;
    }

    /// <summary>
    /// 获得下一个随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private static int Next(int min, int max)
    {
        int value = Next(max - min) + min;
        return value;
    }
}
 