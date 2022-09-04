using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Kyrsova
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LayoutMdi(MdiLayout.Cascade);
        }

        private void CreateMenuItem_Click(object sender, EventArgs e)
        {
            ushort FormID = 0;
            bool wasID = false;
            for (ushort i = 0; i < MdiChildren.Length; i++)
            {
                wasID = false;
                for (ushort a = 0; a < MdiChildren.Length; a++)
                {
                    if (i == Convert.ToUInt16(MdiChildren[a].Text.Substring(5))) 
                    {
                        wasID = true;
                        break; 
                    }
                }
                if (!wasID)
                {
                    FormID = i;
                    break;
                }
                else if (wasID && i == MdiChildren.Length - 1) 
                    FormID = ++i;
            }
        
            ChildForm child = new ChildForm(this);
            child.MdiParent = this;
            child.Icon = this.Icon;
            child.Text = "Form " + FormID;
            child.Show();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
            {
                object msg = "Немає активного вікна!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
                return;
            }
            openFileDialog.Filter = "Файл зображення|*.BMP;*.JPG;*PNG;*.GIF;*TIFF";
            openFileDialog.Multiselect = false;
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                try
                {
                    Image img = Image.FromFile(openFileDialog.FileName);
                    ChildForm activeform = (ChildForm)this.ActiveMdiChild;
                    activeform.FileName = openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf('\\')+1, openFileDialog.FileName.Length - openFileDialog.FileName.LastIndexOf('\\')-1);
                    activeform.FullPath = openFileDialog.FileName;
                    activeform.SetImage(img);
                }
                catch (Exception)
                {
                    object msg = "Непередбачувана помилка!";
                    object title = "Error!";
                    Microsoft.VisualBasic.Interaction.MsgBox(msg,Microsoft.VisualBasic.MsgBoxStyle.OkOnly,title);
                }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            ChildForm activeform = (ChildForm)this.ActiveMdiChild;
            if (activeform == null)
            {
                object msg = "Немає активного вікна!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
                return;
            }
            if (activeform.Image == null)
            {
                object msg = "Зображення відсутнє!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);

                return;
            }
                activeform = (ChildForm)this.ActiveMdiChild;
                Image image = activeform.Image;
                saveFileDialog.Filter = "Файл зображення|*"+ImageProps.GetImageFormat(activeform.FileName);
                saveFileDialog.FileName = activeform.FileName;
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK) 
                {
                    image.Save(saveFileDialog.FileName, image.RawFormat);
                }
        }

        private void UncheckAllItems()
        {
            textToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            rectangleToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            zoomToolStripMenuItem.Checked = false;
        }

        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            ChildForm activeform = (ChildForm)this.ActiveMdiChild;
            if (activeform == null)
            {
                object msg = "Немає активного вікна!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
                return;
            }
            if (activeform.Image == null)
            {
                object msg = "Зображення відсутнє!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);

                return;
            }
            try
            {
                activeform = (ChildForm)this.ActiveMdiChild;
                Image image = activeform.Image;
                saveFileDialog.Filter = "Файл зображення|*.BMP;*.JPG;*.PNG;*.GIF;*.TIFF";
                saveFileDialog.FileName = activeform.FileName;
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    image.Save(saveFileDialog.FileName, image.RawFormat);
                }
            }
            catch (Exception)
            {
                object msg = "Непередбачувана помилка!";
                object title = "Error!";
                Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null) 
                return;
            ChildForm activeform = (ChildForm)this.ActiveMdiChild;
            if (activeform.Image == null || activeform.IsImageChanged == false)
            {
                activeform.Close();
                if (this.ActiveMdiChild != null)
                    MainForm_MdiChildActivate(sender, e);
                return;
            }
            try
                {
                    object msg = "Зберегти зображення?";
                    object title = "Збереження";
                    MsgBoxResult msgBoxResult = Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.YesNo, title);
                    if (msgBoxResult == MsgBoxResult.Yes)
                    {
                        SaveMenuItem_Click(sender, e);
                        activeform.Close();
                        if (this.ActiveMdiChild != null)
                            MainForm_MdiChildActivate(sender, e);
                    }
                    else
                    {
                        activeform.Close();
                        if (this.ActiveMdiChild != null)
                            MainForm_MdiChildActivate(sender, e);
                    }
                }
                catch (Exception)
                {
                    object msg = "Непередбачувана помилка!";
                    object title = "Error!";
                    Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
                }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                Application.Exit();
            Form[] ChildForms = MdiChildren;
            foreach (ChildForm childForm in ChildForms) 
            {
                if (childForm.Image == null || childForm.IsImageChanged == false) 
                {
                    childForm.Close();
                }
                else 
                {
                    ActivateMdiChild(childForm);
                    object msg = "Зберегти зображення з "+childForm.Text+" ?";
                    object title = "Збереження";
                    MsgBoxResult msgBoxResult = Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.YesNo, title);
                    if (msgBoxResult == MsgBoxResult.Yes)
                    {
                        SaveMenuItem_Click(sender, e);
                        childForm.Close();
                    }
                    else
                        childForm.Close();
                }
            }
            Application.Exit();

        }

        private void InfoMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)this.ActiveMdiChild;
            if(activeform.Image == null)
                return;

            int BppImage = ImageProps.GetBpp(activeform.Image);
            bool hasAlpha = ImageProps.GetAlpha(activeform.Image);
            object msg = "І'мя файлу: " + activeform.FileName +
                "\nПовний шлях: " + activeform.FullPath + 
                "\nФормат файлу: " + ImageProps.GetImageFormat(activeform.FileName) + 
                "\nРозміри у пікселях: " + activeform.Image.Height + " висота, " + activeform.Image.Width + " ширина" +
                "\nРоздільна здатність(пікселі на см): \n"+ Math.Round(activeform.Image.VerticalResolution/2.54, 2) + " вертикальна, "+ Math.Round(activeform.Image.HorizontalResolution / 2.54, 2) + " горизонтальна"+
                "\nФізичні розміри (см): "+ Math.Round(activeform.Image.Height/(activeform.Image.VerticalResolution / 2.54), 2) + " висота, "+ Math.Round(activeform.Image.Width / (activeform.Image.HorizontalResolution / 2.54), 2) + " ширина"+
                "\nФормат пікселів: " + activeform.Image.PixelFormat +
                "\nБіт/байт прозорості: " + hasAlpha.ToString() +
                "\nЧисло біт на піксель: " + BppImage;
            object title = "Інформація про зображення";
            Microsoft.VisualBasic.Interaction.MsgBox(msg, Microsoft.VisualBasic.MsgBoxStyle.OkOnly, title);
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            activeform.ZoomedImagePainted = false;
            textToolStripMenuItem.Checked = true;
            activeform.FirstClick = false;
            activeform.curentTool = Tools.TEXT;
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            activeform.ZoomedImagePainted = false;
            lineToolStripMenuItem.Checked = true;
            activeform.FirstClick = false;
            activeform.curentTool = Tools.LINE;
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            activeform.ZoomedImagePainted = false;
            ellipseToolStripMenuItem.Checked = true;
            activeform.FirstClick = false;
            activeform.curentTool = Tools.ELLIPSE;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            activeform.ZoomedImagePainted = false;
            rectangleToolStripMenuItem.Checked = true;
            activeform.FirstClick = false;
            activeform.curentTool = Tools.RECTANGLE;
        }

        private void styleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            string fontfamily = Interaction.InputBox("Введіть назву шрифту:", "Вибір шрифту");
            activeform.FontFamily = fontfamily;
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild == null)
                return;

            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            zoomToolStripMenuItem.Checked = true;
            activeform.FirstClick = false;
            activeform.curentTool = Tools.ZOOM;
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            ChildForm activeform = (ChildForm)ActiveMdiChild;
            UncheckAllItems();
            if (ActiveMdiChild == null)
                return;
            if (activeform.curentTool != Tools.NULL)
            {
                switch (activeform.curentTool) 
                {
                    case Tools.ELLIPSE:
                        ellipseToolStripMenuItem_Click(sender, e);
                        break;
                    case Tools.LINE:
                        lineToolStripMenuItem_Click(sender, e);
                        break;
                    case Tools.RECTANGLE:
                        rectangleToolStripMenuItem_Click(sender, e);
                        break;
                    case Tools.TEXT:
                        textToolStripMenuItem_Click(sender, e);
                        break;
                    case Tools.ZOOM:
                        zoomToolStripMenuItem_Click(sender, e);
                        break;
                }
                   
            }
        }

        private void cancelZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChildForm activeform = (ChildForm)ActiveMdiChild;
            if (activeform.ZoomedImagePainted)
            {
                activeform.ZoomedImagePainted = false;
                zoomToolStripMenuItem_Click(null, null);
                activeform.SetImage(activeform.Image);
            }
        }
    }
}
