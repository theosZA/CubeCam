using AForge.Video.DirectShow;
using CubeCam.Cube;
using CubeCam.Video;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CubeCam
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyPress += OnKeyPress;
            videoStreaming.OnNewFrame += OnNewFrame;
            cubeStateMachine.OnNewTime += OnNewTime;
            videoStreaming.VideoInjector = cubeStateMachine;
            RequestVideoSource();
        }
        
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                cubeStateMachine.AdvanceState();
            }
        }

        private void OnNewFrame(Bitmap newFrame)
        {
            videoDisplay.Image = newFrame;
        }
        
        private void OnNewTime(TimeSpan newTime)
        {
            tbResults.Text = cubeStateMachine.GetTextSummary();
        }

        private void RequestVideoSource()
        {
            VideoCaptureDeviceForm form = new VideoCaptureDeviceForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                videoStreaming.StartVideoSource(form.VideoDevice,
                                                form.VideoDevice.VideoResolution.FrameSize.Width,
                                                form.VideoDevice.VideoResolution.FrameSize.Height);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoStreaming.StopVideoSource();
        }

        private void importScramblesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Import Scrambles",
                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt|All files|*.*"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                cubeStateMachine.LoadScrambles(dialog.FileName);
            }
        }

        private void saveVideoStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save Video Stream",
                FileName = "cube.avi",
                DefaultExt = "avi",
                AddExtension = true,
                Filter = "Video files (*.avi)|*.avi|All files|*.*"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                videoStreaming.VideoOutputFileName = dialog.FileName;
            }
        }

        private void btnWriteResults_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                handledSpace = true;
            }
        }

        private void btnWriteResults_Click(object sender, EventArgs e)
        {
            if (handledSpace)
            {
                handledSpace = false;
                return;
            }

            cubeStateMachine.StartWriteResults();
        }

        private bool handledSpace = false;

        private VideoStreaming videoStreaming = new VideoStreaming();
        private CubeStateMachine cubeStateMachine = new CubeStateMachine();
    }
}
