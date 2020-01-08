using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;
using NAudioDemo.Utils;

namespace NAudioDemo
{
    public sealed partial class MainForm : Form
    {
        private INAudioDemoPlugin currentPlugin;

        public MainForm()
        {
            COMHelper.SerialPortInit();
            COMHelper cOMHelper = new COMHelper();

            for (int i = 0; i < 200; i++)
            {

                string audio;
                if(i %2 == 0)
                {
                    audio = @"D:\Github\BrainFunctionMonitor\Debug\ERP\MMN\sin800.wav";
                }
                else
                {
                    audio = @"D:\Github\BrainFunctionMonitor\Debug\ERP\MMN\s3.wav";
                }
                using (var audioFile = new AudioFileReader(audio))
                using (var outputDevice = new WasapiOut())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    cOMHelper.SendToCOM(i%2+1);
                    //Console.WriteLine("After Play:" + sw.ElapsedMilliseconds);
                    int interval = 0;
                    
                    switch(i%3)
                    {
                        case 0:
                            interval = 150;
                            break;
                        case 1:
                            interval = 125;
                            break;
                        case 2:
                            interval = 137;
                            break;
                    }
                    for (int j = 0; j < interval; j++)
                    {
                        Thread.Sleep(1);
                    }
                    //while (outputDevice.PlaybackState == PlaybackState.Playing)
                    //{
                    //    Thread.Sleep(1);
                    //}
                }
                for(int j = 0;j<100 ;j++ )
                {
                    Thread.Sleep(1);
                }
            }

            COMHelper.SerialPortClose();
            //// use reflection to find all the demos
            //var demos = ReflectionHelper.CreateAllInstancesOf<INAudioDemoPlugin>().OrderBy(d => d.Name);

            //InitializeComponent();
            //listBoxDemos.DisplayMember = "Name";
            //foreach (var demo in demos)
            //{
            //    listBoxDemos.Items.Add(demo);
            //}

            //Text += ((System.Runtime.InteropServices.Marshal.SizeOf(IntPtr.Zero) == 8) ? " (x64)" : " (x86)");
        }


        private void OnLoadDemoClick(object sender, EventArgs e)
        {
            var plugin = (INAudioDemoPlugin)listBoxDemos.SelectedItem;
            if (plugin == currentPlugin) return;
            currentPlugin = plugin;
            DisposeCurrentDemo();
            var control = plugin.CreatePanel();
            control.Dock = DockStyle.Fill;
            panelDemo.Controls.Add(control);
        }

        private void DisposeCurrentDemo()
        {
            if (panelDemo.Controls.Count <= 0) return;
            panelDemo.Controls[0].Dispose();
            panelDemo.Controls.Clear();
            GC.Collect();
        }

        private void OnListBoxDemosDoubleClick(object sender, EventArgs e)
        {
            OnLoadDemoClick(sender, e);
        }

        private void OnMainFormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeCurrentDemo();
        }
    }
}