using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kyrsova
{
    public enum Tools
    {
        TEXT = 1, LINE, ELLIPSE, RECTANGLE, ZOOM, NULL = 0
    }

    public partial class ChildForm : Form
    {
        public Image Image;
        public Rectangle ZoomedRectangle;
        public float ZoomValue = 3F;
        public bool ZoomedImagePainted = false;
        public bool IsImageChanged = false;
        public string FileName;
        public string FullPath;

        public Tools curentTool = Tools.NULL;
        public SolidBrush Brh = new SolidBrush(Color.Black);
        public Pen Pen = new Pen(Color.Black, 10);
        public string FontFamily = "Times New Roman";
        public string TextDraw = "";
        public Point PreviousPoint;
        public bool FirstClick = false;

        public ChildForm(MainForm mainForm)
        {
            InitializeComponent();
        }

        private void ChildForm_Paint(object sender, PaintEventArgs e)
        {
            if (ZoomedImagePainted)
                pictureBox.Image = GetZoomImage(ZoomedRectangle);
            else
                pictureBox.Image = Image;
        }

        public void SetImage(Image image) 
        {
            Image = image;
            pictureBox.Image = image;
        }

        private Point GetOffsetPoint(Point point) 
        {
            float scaleFactorX = pictureBox.ClientSize.Width / pictureBox.Image.Size.Width;
            float scaleFactorY = pictureBox.ClientSize.Height / pictureBox.Image.Size.Height;

            Point unscaled_p = new Point();

            // image and container dimensions
            int w_i = pictureBox.Image.Width;
            int h_i = pictureBox.Image.Height;
            int w_c = pictureBox.Width;
            int h_c = pictureBox.Height;

            float imageRatio = w_i / (float)h_i; // image W:H ratio
            float containerRatio = w_c / (float)h_c; // container W:H ratio

            if (imageRatio >= containerRatio)
            {
                // horizontal image
                float scaleFactor = w_c / (float)w_i;
                float scaledHeight = h_i * scaleFactor;
                // calculate gap between top of container and top of image
                float filler = Math.Abs(h_c - scaledHeight) / 2;
                unscaled_p.X = (int)(point.X / scaleFactor);
                unscaled_p.Y = (int)((point.Y - filler) / scaleFactor);
            }
            else
            {
                // vertical image
                float scaleFactor = h_c / (float)h_i;
                float scaledWidth = w_i * scaleFactor;
                float filler = Math.Abs(w_c - scaledWidth) / 2;
                unscaled_p.X = (int)((point.X - filler) / scaleFactor);
                unscaled_p.Y = (int)(point.Y / scaleFactor);
            }

            return unscaled_p;
        }
        void DrawLine(Point point)
        {
            if (Image == null)
                return;
            if (FirstClick == true)
            {
                Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
                Graphics gfx = Graphics.FromImage(bmp);    
                gfx.DrawImage(Image, new PointF(0, 0));
                gfx.DrawLine(Pen, GetOffsetPoint(PreviousPoint), GetOffsetPoint(point));
                gfx.Dispose();
                SetImage(bmp);

                FirstClick = false;
                IsImageChanged = true;
            }
            else
                FirstClick = true;
        }
        void DrawEllipse(Point point)
        {
            if (Image == null)
                return;
            if (FirstClick == true)
            {
                Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
                Graphics gfx = Graphics.FromImage(bmp);
                gfx.DrawImage(Image, new PointF(0, 0));
                gfx.DrawEllipse(Pen, GetOffsetPoint(PreviousPoint).X, GetOffsetPoint(PreviousPoint).Y,
                                (GetOffsetPoint(point).X - GetOffsetPoint(PreviousPoint).X),
                                (GetOffsetPoint(point).Y - GetOffsetPoint(PreviousPoint).Y));
                gfx.Dispose();
                SetImage(bmp);
                
                FirstClick = false;
                IsImageChanged = true;
            }
            else
                FirstClick = true;
        }
        void DrawRectangle(Point point)
        {
            if (Image == null)
                return;
            if (FirstClick == true)
            {
                Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
                Graphics gfx = Graphics.FromImage(bmp);
                gfx.DrawImage(Image, new PointF(0, 0));
                gfx.DrawRectangle(Pen, GetRectangle(GetOffsetPoint(point), GetOffsetPoint(PreviousPoint)));
                gfx.Dispose();
                SetImage(bmp);

                FirstClick = false;
                IsImageChanged = true;
            }
            else
                FirstClick = true;
        }
        private Rectangle GetRectangle(Point p1, Point p2)
        {
            int top = Math.Min(p1.Y, p2.Y);
            int bottom = Math.Max(p1.Y, p2.Y);
            int left = Math.Min(p1.X, p2.X);
            int right = Math.Max(p1.X, p2.X);

            Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
            return rect;
        }
        void DrawText(Point point)
        {
            if (Image == null)
                return;
            string msg = "Введіть текст:";
            string title = "Інформація про зображення";
            TextDraw = Microsoft.VisualBasic.Interaction.InputBox(msg, title);
            if (TextDraw == "")
                return;

            Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
            bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
            Graphics gfx = Graphics.FromImage(bmp);
            Font TextFont = new Font(FontFamily, pictureBox.Image.Width / 10);
            gfx.DrawImage(Image, new PointF(0, 0));
            gfx.DrawString(TextDraw, TextFont, Brh, GetOffsetPoint(point).X, GetOffsetPoint(point).Y);
            gfx.Dispose();
            SetImage(bmp);
            IsImageChanged = true;
        }
        void ZoomImage(Point point)
        {
            if (Image == null || ZoomedImagePainted)
                return;
            if (FirstClick == true)
            {
                ZoomedRectangle = GetRectangle(GetOffsetPoint(point), GetOffsetPoint(PreviousPoint));
                Pen RedPen = new Pen(Color.Red, 10);
                Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
                bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
                Graphics gfx = Graphics.FromImage(bmp);
                gfx.DrawImage(Image, new PointF(0, 0));

                Bitmap temp = new Bitmap(CropImage(bmp, ZoomRectangle(ZoomedRectangle, ZoomValue)));
                SolidBrush tmpBrh = new SolidBrush(Color.Transparent);
                gfx.FillRectangle(tmpBrh, ZoomedRectangle);
                gfx.DrawImage(temp, ZoomedRectangle);
                gfx.DrawRectangle(RedPen, ZoomedRectangle);
                pictureBox.Image = bmp;
                gfx.Dispose();

                FirstClick = false;
                ZoomedImagePainted = true;
            }
            else
                FirstClick = true;
        }

        Rectangle ZoomRectangle(Rectangle rectangle, float Zoom) 
        {
            int tempx = Convert.ToInt32(rectangle.Width / Zoom / 2);
            int tempy = Convert.ToInt32(rectangle.Height / Zoom / 2);

            rectangle.X += tempx;
            rectangle.Y += tempy;
            rectangle.Width -= tempx;
            rectangle.Height -= tempy;

            return rectangle;
        }

        Bitmap GetZoomImage(Rectangle rectangle)
        {
            Pen RedPen = new Pen(Color.Red, 10);
            Bitmap bmp = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
            bmp.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.DrawImage(Image, new PointF(0, 0));

            Bitmap temp = new Bitmap(CropImage(bmp, ZoomRectangle(ZoomedRectangle, ZoomValue)));
            SolidBrush tmpBrh = new SolidBrush(Color.Transparent);
            gfx.FillRectangle(tmpBrh, ZoomedRectangle);
            gfx.DrawImage(temp, ZoomedRectangle);
            gfx.DrawRectangle(RedPen, ZoomedRectangle);
            pictureBox.Image = bmp;
            gfx.Dispose();
            return bmp;
        }

        public Bitmap CropImage(Image source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public Image GetImageFromPictureBox() 
        {
            return pictureBox.Image;
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (curentTool)
            {
                case Tools.LINE:
                    DrawLine(new Point(e.X, e.Y));
                    break;
                case Tools.ELLIPSE:
                    DrawEllipse(new Point(e.X, e.Y));
                    break;
                case Tools.TEXT:
                    DrawText(new Point(e.X, e.Y));
                    break;
                case Tools.RECTANGLE:
                    DrawRectangle(new Point(e.X, e.Y));
                    break;
                case Tools.ZOOM:
                    ZoomImage(new Point(e.X, e.Y));
                    break;
            }
            PreviousPoint.X = e.X;
            PreviousPoint.Y = e.Y;
        }
        private void ChildForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (curentTool == Tools.ZOOM && ZoomedImagePainted)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                    e.IsInputKey = true;
                if (e.KeyCode == Keys.Up && ZoomValue > 1)
                {
                    ZoomValue -= 0.5F;
                    ChildForm_Paint(null, null);
                }
                else if (e.KeyCode == Keys.Down && ZoomValue < 3)
                {
                    ZoomValue += 0.5F;
                    ChildForm_Paint(null, null);
                }
            }
        }
    }
}
