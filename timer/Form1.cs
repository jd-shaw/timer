using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace timer
{
    public partial class Form1 : Form
    {

        System.Timers.Timer timer;
        double hours = 0;
        DateTime startTime;

        public Form1()
        {
            InitializeComponent();
        }

        private void selectFile_Click(object sender, EventArgs e)
        {
            String filePath = ImportExcelToDataTable(this);

            if (!String.IsNullOrWhiteSpace(filePath))
            {
                filePathText.Text = filePath;

            }
        }
        private void start_Click(object sender, EventArgs e)
        {


            if (numericUpDown.Value > 0 && !String.IsNullOrWhiteSpace(filePathText.Text))
            {
                selectFile.Enabled = false;
                numericUpDown.Enabled = false;
                start.Enabled = false;
                filePathText.Enabled = false;
                hours = (double)this.numericUpDown.Value;
                startTime = DateTime.Now;

                timer = new System.Timers.Timer();
                timer.Interval = 1000;  //设置计时器事件间隔执行时间
                timer.Elapsed += new System.Timers.ElapsedEventHandler(doExcuteTimer);
                timer.Enabled = true;
            }
            else
            {
                MessageBox.Show("文件路径与执行间隔为必填项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
            }
        }


        private void setTaskAtFixedTime()
        {
            try
            {
                DateTime oneOClock = DateTime.Now;
                if (oneOClock > startTime)
                {

                    addMessage("执行文件：" + filePathText.Text);

                    // 这里是要调用的可执行文件的文件夹目录
                    string targetPath = string.Format(@"" + filePathText.Text + "");
                    Process.Start(targetPath);

                    startTime = DateTime.Now.AddMinutes(hours);
                }
            }
            catch (Exception e)
            {
                addMessage(e.Message);
            }

        }

        //要执行的任务
        private void doExcuteTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            MethodInvoker showMsgLabelInvoke = new MethodInvoker(() =>
            {
                int intHour = e.SignalTime.Hour;
                int intMinute = e.SignalTime.Minute;
                int intSecond = e.SignalTime.Second;

                //再次设定
                setTaskAtFixedTime();
            });
            this.BeginInvoke(showMsgLabelInvoke);
        }

        private void stop_Click(object sender, EventArgs e)
        {
            addMessage("定时执行停止！");

            selectFile.Enabled = true;
            numericUpDown.Enabled = true;
            start.Enabled = true;
            filePathText.Enabled = true;
            if (timer != null)
            {
                timer.Stop();
                this.timer.Enabled = false;
            }
        }
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="form"></param>
        /// <param name="callback"></param>
        public String ImportExcelToDataTable(Form form)
        {
            string Filter = "bat文件|*.bat|exe文件|*.exe";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "可执行文件/程序";
            openFileDialog1.Filter = Filter;
            openFileDialog1.ValidateNames = true;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return String.Empty;

            string localFilePath = openFileDialog1.FileName;
            return localFilePath;
        }

        private void clearMessage_Click(object sender, EventArgs e)
        {
            message.Items.Clear();
        }


        private void addMessage(String messageValue)
        {
            message.Items.Add(DateTime.Now + "—>" + messageValue);
            message.TopIndex = message.Items.Count - 1;
        }

    }
}
