using BananaHackV2.OCR;
using BananaHackV2.UI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BananaHackV2.UI
{
    public partial class WndMain : Form
    {
        //private int _currentPaintIndex = 0;
        //private List<Graphics> _graphics = new List<Graphics>();
        //private List<OcrRegionPainter> _painter = new List<OcrRegionPainter>();

        private void AddPainter(ShiftProcessor processor, Bitmap bitmap)
        {
            this.Invoke(new MethodInvoker(() => {
                //var copy = new Bitmap(bitmap);
                //_currentPaintIndex++;
                //tableLayout_ImageDisplay.RowCount = _currentPaintIndex;
                //var pb = new PictureBox();
                //pb.Size = copy.Size;
                ////pb.Dock = DockStyle.Fill;
                //pb.Image = copy;
                //Graphics gr = Graphics.FromImage(copy);
                //var painter = new OcrRegionPainter(processor, ref gr);
                //_graphics.Add(gr);
                //_painter.Add(painter);
                //tableLayout_ImageDisplay.Controls.Add(pb, 1, _currentPaintIndex);
            }));            
        }


        private void StartProcessing()
        {
            Stopwatch sp = Stopwatch.StartNew();

            string folder = @"C:\Users\max\Bilder\shift_ocr";
            //using (var ofd = new System.Windows.Forms.FolderBrowserDialog()) {
            //    ofd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            //    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
            //        folder = ofd.SelectedPath;
            //    }
            //}

            if (string.IsNullOrEmpty(folder)) {
                return;
            }

            ParallelOptions parallelOpts = new ParallelOptions();
            parallelOpts.MaxDegreeOfParallelism = Environment.ProcessorCount / 2;

            string[] files = Directory.GetFiles(folder);
            Parallel.ForEach(files, parallelOpts, file => {
                using (var source = (Bitmap)Image.FromFile(file))
                using (var processor = new ShiftProcessor(source)) {                    
                    //AddPainter(processor, source);                    
                    var results = processor.StartProcessing();
                    Application.DoEvents();
                }
            });

            //string[] files = Directory.GetFiles(folder);
            //foreach (var file in files)
            //{
            //    Bitmap source = (Bitmap)Image.FromFile(file);
            //    using (var processor = new ShiftProcessor(source)) {
            //        Application.DoEvents();
            //        AddPainter(processor, source);
            //        var results = processor.StartProcessing();
            //    }
            //    source?.Dispose();
            //}

            sp.Stop();
            Debug.WriteLine(sp.ElapsedMilliseconds);
        }


        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            SetUploadButtonCentered();
            //Task t = new Task(() => StartProcessing());
            //t.Start();
        }


        private void SetUploadButtonCentered()
        {
            if (InvokeRequired) {
                this.Invoke(new MethodInvoker(() => {
                    SetUploadButtonCentered();                    
                }));
                return;
            }
            else {
                int w = splitContainer1.Panel1.Width;
                int h = splitContainer1.Panel1.Height;
                int bw = uploadButton_OcrImage.Width;
                int bh = uploadButton_OcrImage.Height;

                uploadButton_OcrImage.Location = new Point(w / 2 - bw / 2, h / 2 -  bh / 2);
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetUploadButtonCentered();
        }


        public WndMain()
        {
            InitializeComponent();

            this.uploadButton_OcrImage.ScreenshotRequested += UploadButtonScreenshotRequested;
        }


        private Stack<WndOverlay> _overlays = new Stack<WndOverlay>();
        private static object _overlayLock = new object();


        private void OnOverlayClosed(object sender, EventArgs e)
        {
            lock (_overlayLock) {
                while (_overlays.Count > 0) {
                    var window = _overlays.Pop();
                    window.Close();
                    window.Dispose();
                }
            }
        }


        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            //foreach (FlowImageContainer flowImage in flowLayout_Images.Controls)
            //{
            //    flowImage.Height = flowLayout_Images.Height / 4;
            //}
        }


        private void FlowContainerCloseRequested(object sender, EventArgs e)
        {
            FlowImageContainer flowImage = (FlowImageContainer)sender;
            if (flowImage == null) {
                return;
            }
            flowLayout_Images.Controls.Remove(flowImage);
        }


        private void OverlayScreenshotReady(object sender, ScreenshotEventArgs e)
        {
            OnOverlayClosed(this, null);
            FlowImageContainer flowImage = new FlowImageContainer();
            flowImage.Image = e.Screenshot;
            flowImage.Size = e.Bounds.Size;
            flowImage.Dock = DockStyle.Top;
            flowImage.CloseClicked += FlowContainerCloseRequested;
            flowLayout_Images.Controls.Add(flowImage);
        }


        private void UploadButtonScreenshotRequested(object sender, EventArgs e)
        {
            lock (_overlayLock) {
                foreach (Screen screen in Screen.AllScreens) 
                {
                    var overlay = new WndOverlay(
                        screen, 
                        Color.FromArgb(0, 0, 0), 
                        0.65);

                    overlay.FormClosed      += OnOverlayClosed;
                    overlay.ScreenshotReady += OverlayScreenshotReady;

                    _overlays.Push(overlay);
                    
                    overlay.Show();
                }
            }
        }


        private void splitContainer1_Panel1_SizeChanged(object sender, EventArgs e)
        {
            SetUploadButtonCentered();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            foreach (FlowImageContainer flowImage in flowLayout_Images.Controls) {
                Bitmap target = (Bitmap)flowImage.Image;
                var processor = new OCR.ShiftProcessor(target);
                var result = processor.StartProcessing();
            }            
        }
    }
}
