using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BananaHackV2.UI.Components
{
    [DefaultEvent("ScreenshotRequested")]
    public partial class UploadButton : UserControl
    {
        private const string FILTER_IMAGEFILES = "Bilder|*.jpg;*.jpeg;*.png;*.tif;*.tiff";

        private const string LABEL_UPLOADFILE       = "Datei hochladen";
        private const string LABEL_TAKESCREENSHOT   = "Screenshot erstellen";

        private const int OPTION_FILE       = 0;
        private const int OPTION_SCREENSHOT = 1;

        private int _currentOption = OPTION_FILE;

        private ToolTip _internalTip = new ToolTip();

        private OpenFileDialog _imageFileDialog = new OpenFileDialog() {
            Filter = FILTER_IMAGEFILES,
            AddExtension = true,
            AutoUpgradeEnabled = true,
            CheckFileExists = true,
            CheckPathExists = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Multiselect = true,
            RestoreDirectory = true,
            SupportMultiDottedExtensions = true,
            Title = "Banana Screenshots auswählen"
        };

        private OpenFileDialog ImageFileDialog
        {
            get {
                return ImageFileDialog;
            }
        }


        private event EventHandler onScreenshotRequested;
        public event EventHandler ScreenshotRequested
        {
            add {
                onScreenshotRequested += value;
            }
            remove {
                onScreenshotRequested -= value;
            }
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            switch (_currentOption)
            {
                case OPTION_FILE:
                    SelectImages();
                    break;
                case OPTION_SCREENSHOT:
                    onScreenshotRequested?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    onScreenshotRequested?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }


        private string GetOptionText(int option)
        {
            switch (option) {
                case OPTION_FILE:
                    return LABEL_UPLOADFILE;
                case OPTION_SCREENSHOT:
                    return LABEL_TAKESCREENSHOT;
                default:
                    return LABEL_TAKESCREENSHOT;
            }
        }

        private void SetUploadOption(int option)
        {
            if (InvokeRequired) {
                this.Invoke(
                    new MethodInvoker(() => SetUploadOption(option)
                ));
                return;
            }
            lbl_CurrentOption.Text = GetOptionText(option);
            _currentOption = option;
        }


        private void ButtonOnEnterSetOption(object sender, EventArgs e) 
        {
            Button me = (Button)sender;
            if (me == null) {
                return;
            }

            string optionTag = me?.Tag?.ToString();
            int option;
            if (int.TryParse(optionTag, out option)) {
                SetUploadOption(option);
            }
            else {
                SetUploadOption(OPTION_SCREENSHOT);
            }
        }


        private void SelectImages()
        {
            if (_imageFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string[] files = _imageFileDialog.FileNames;
        }


        private void ButtonSelectFileClick(object sender, EventArgs e)
        {
            SelectImages();
        }


        private void ButtonScreenshotClick(object sender, EventArgs e)
        {
            onScreenshotRequested?.Invoke(this, EventArgs.Empty);
        }


        private void LabelOptionClick(object sender, EventArgs e)
        {
            switch (_currentOption)
            {
                case OPTION_FILE:
                    SelectImages();
                    break;
                case OPTION_SCREENSHOT:
                    onScreenshotRequested?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    onScreenshotRequested?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }


        public UploadButton()
        {
            InitializeComponent();

            SetUploadOption(OPTION_SCREENSHOT);

            lbl_CurrentOption.Click += LabelOptionClick;

            btn_Screenshot.MouseEnter += ButtonOnEnterSetOption;
            btn_SelectFIle.MouseEnter += ButtonOnEnterSetOption;

            btn_Screenshot.Click += ButtonScreenshotClick;
            btn_SelectFIle.Click += ButtonSelectFileClick;
        }

        private void Btn_SelectFIle_MouseEnter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tableLayout_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
