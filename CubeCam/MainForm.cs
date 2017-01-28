using AForge.Video.DirectShow;
using CubeCam.Cube;
using CubeCam.Extensions;
using CubeCam.Video;
using System;
using System.Drawing;
using System.Text;
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
            timeAggregator.AddTime(newTime);
            var text = new StringBuilder();
            int count = 0;
            foreach (var time in timeAggregator.Times)
            {
                ++count;
                text.Append($"{count}. {time.ToSecondsString()}\n");
            }
            text.Append($"\nMean: {timeAggregator.Mean.ToSecondsString()}");
            text.Append($"\nAverage (drop high & low): {timeAggregator.AverageDropHighLow.ToSecondsString()}");
            text.Append($"\n\n{timeAggregator.TextListDropHighLow}");
            tbResults.Text = text.ToString();
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

        private VideoStreaming videoStreaming = new VideoStreaming();
        private CubeStateMachine cubeStateMachine = new CubeStateMachine();
        private TimeAggregator timeAggregator = new TimeAggregator();
    }
}
