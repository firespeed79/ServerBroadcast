using System;
using System.Net;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

using KellRemoting.Common;

namespace KellRemoting.Client
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class ClientForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label1;
		private IBroadCast watch = null;
		private System.Windows.Forms.TextBox txtMessage;
        private CheckBox checkBox1;
		private EventWrapper wrapper = null;

		public ClientForm()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.btnClear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(224, 248);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "&Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "收到服务端的广播:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(32, 40);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(272, 184);
            this.txtMessage.TabIndex = 5;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(34, 248);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "订阅广播事件";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ClientForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.ClientSize = new System.Drawing.Size(336, 285);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClear);
            this.Name = "ClientForm";
            this.Text = "ClientWatcher";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ClientForm_Closing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new ClientForm());
		}

		private void ClientForm_Load(object sender, System.EventArgs e)
		{
			BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

			IDictionary props = new Hashtable();
			props["port"] = 0;
			HttpChannel channel = new HttpChannel(props,clientProvider,serverProvider);
			ChannelServices.RegisterChannel(channel, false);

			watch = (IBroadCast)Activator.GetObject(
				typeof(IBroadCast),"http://localhost:8080/BroadCastMessage.soap");

			wrapper = new EventWrapper();	
			wrapper.LocalBroadCastEvent += new BroadCastEventHandler(BroadCastingMessage);
			watch.BroadCastEvent += wrapper.BroadCasting;
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			txtMessage.Text = "";
		}

		public void BroadCastingMessage(string message)
		{
			txtMessage.Text += "I got it:" + message;				
			txtMessage.Text += System.Environment.NewLine;
		}

		private void ClientForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			watch.BroadCastEvent -= wrapper.BroadCasting;
		}

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                watch.BroadCastEvent += wrapper.BroadCasting;
                //MessageBox.Show("订阅广播成功!");
            }
            else
            {
                watch.BroadCastEvent -= wrapper.BroadCasting;
                //MessageBox.Show("取消订阅广播成功!");
            }
        }		
	}
}
