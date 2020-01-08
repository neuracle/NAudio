using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;
using NAudioDemo.Utils;

namespace NAudioDemo
{
    public sealed partial class MainForm : Form
    {
        private INAudioDemoPlugin currentPlugin;

        [DllImport("INPOUT32", EntryPoint = "Out32")]
        public static extern void Output(int address, int value);

        private int _parallelPortAddress = 888;
        bool _useParallelPort = true;

        public MainForm()
        {
            COMHelper.SerialPortInit();
            COMHelper cOMHelper = new COMHelper();

            for (int i = 0; i < 200; i++)
            {

                string audio;
                if(i %2 == 0)
                {
                    audio = @"sin800.wav";
                }
                else
                {
                    audio = @"sin1200.wav";
                }
                using (var audioFile = new AudioFileReader(audio))
                using (var outputDevice = new WasapiOut())
                {
                    ParallelPortSend(_parallelPortAddress, 0);
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    if (!_useParallelPort)
                    {
                        cOMHelper.SendToCOM(i % 2 + 1);
                    }
                    else Output(_parallelPortAddress, i % 2 + 1);

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
        private void ParallelPortSend(int address, int eventNumber)
        {
            Output(address, eventNumber);
        }
    }
}