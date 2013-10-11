using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Server.MirDatabase;

namespace Server
{
    public partial class Main : Form
    {
        private static readonly Queue<string> MessageLog = new Queue<string>();
        private static readonly Queue<string> ExceptionLog = new Queue<string>();

        public static DateTime StartTime = DateTime.Now;
        public static DateTime Now
        {
            get { return StartTime.AddMilliseconds(Time); }
        }
        public static long Time;
        public static Stopwatch Timer = Stopwatch.StartNew();
        
        public Main()
        {
            InitializeComponent();
            MirDB.Start();
            MirEnvir.Envir.Start();
            MirNetwork.Network.Start();
        }



        public static void EnqueueMessage(string S)
        {
            MessageLog.Enqueue(string.Format("[{0}]: {1}" + Environment.NewLine, Now, S));
        }
        public static void EnqueueException(Exception E)
        {
            ExceptionLog.Enqueue(string.Format("[{0}]:{1} - {2}" + Environment.NewLine, Now, E.TargetSite, E));
        }
        
        private void InterfaceTimer_Tick(object sender, EventArgs e)
        {
            UserCountLabel.Text = string.Format("{0}/{1}", MirNetwork.Network.ActiveConnections.Count,
                MirNetwork.Network.ActiveConnections.Count + MirNetwork.Network.ExpiredConnections.Count);


            while (MessageLog.Count > 0)
            {
                string S = MessageLog.Dequeue();
                if (S == null)
                    continue;

                SystemLogTextBox.AppendText(S);
            }

            while (ExceptionLog.Count > 0)
                ExceptionLogTextBox.AppendText(ExceptionLog.Dequeue());
        }


        public static void UpdateTime()
        {
            Time = Timer.ElapsedMilliseconds;
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mapsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
        }




    }
}
