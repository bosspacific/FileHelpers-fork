using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace FileHelpersSamples
{
	/// <summary>
	/// Summary description for frmSamples.
	/// </summary>
	public class frmSamples : frmFather
	{
		private Button cmdEasy;
		private Button cmdDataLink;
		private Button button1;
		private Button cmdEasy2;
		private Button cmdLibrary;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public frmSamples()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cmdEasy = new System.Windows.Forms.Button();
			this.cmdDataLink = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.cmdEasy2 = new System.Windows.Forms.Button();
			this.cmdLibrary = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pictureBox2
			// 
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(64, 51);
			// 
			// cmdEasy
			// 
			this.cmdEasy.BackColor = System.Drawing.Color.FromArgb(((System.Byte) (0)), ((System.Byte) (0)), ((System.Byte) (192)));
			this.cmdEasy.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.cmdEasy.ForeColor = System.Drawing.Color.White;
			this.cmdEasy.Location = new System.Drawing.Point(80, 72);
			this.cmdEasy.Name = "cmdEasy";
			this.cmdEasy.Size = new System.Drawing.Size(240, 43);
			this.cmdEasy.TabIndex = 0;
			this.cmdEasy.Text = "Easy Delimited sample ->";
			this.cmdEasy.Click += new System.EventHandler(this.cmdEasy_Click);
			// 
			// cmdDataLink
			// 
			this.cmdDataLink.BackColor = System.Drawing.Color.FromArgb(((System.Byte) (0)), ((System.Byte) (0)), ((System.Byte) (192)));
			this.cmdDataLink.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.cmdDataLink.ForeColor = System.Drawing.Color.White;
			this.cmdDataLink.Location = new System.Drawing.Point(80, 184);
			this.cmdDataLink.Name = "cmdDataLink";
			this.cmdDataLink.Size = new System.Drawing.Size(240, 43);
			this.cmdDataLink.TabIndex = 2;
			this.cmdDataLink.Text = "DataLink Example ->";
			this.cmdDataLink.Click += new System.EventHandler(this.cmdDataLink_Click);
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.Navy;
			this.button1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.button1.ForeColor = System.Drawing.Color.White;
			this.button1.Location = new System.Drawing.Point(80, 320);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(240, 43);
			this.button1.TabIndex = 4;
			this.button1.Text = "Exit";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// cmdEasy2
			// 
			this.cmdEasy2.BackColor = System.Drawing.Color.FromArgb(((System.Byte) (0)), ((System.Byte) (0)), ((System.Byte) (192)));
			this.cmdEasy2.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.cmdEasy2.ForeColor = System.Drawing.Color.White;
			this.cmdEasy2.Location = new System.Drawing.Point(80, 120);
			this.cmdEasy2.Name = "cmdEasy2";
			this.cmdEasy2.Size = new System.Drawing.Size(240, 43);
			this.cmdEasy2.TabIndex = 1;
			this.cmdEasy2.Text = "Easy Fixed sample ->";
			this.cmdEasy2.Click += new System.EventHandler(this.cmdEasy2_Click);
			// 
			// cmdLibrary
			// 
			this.cmdLibrary.BackColor = System.Drawing.Color.FromArgb(((System.Byte) (0)), ((System.Byte) (0)), ((System.Byte) (192)));
			this.cmdLibrary.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.cmdLibrary.ForeColor = System.Drawing.Color.White;
			this.cmdLibrary.Location = new System.Drawing.Point(80, 248);
			this.cmdLibrary.Name = "cmdLibrary";
			this.cmdLibrary.Size = new System.Drawing.Size(240, 43);
			this.cmdLibrary.TabIndex = 3;
			this.cmdLibrary.Text = "Time And Stress Tests ->";
			this.cmdLibrary.Click += new System.EventHandler(this.cmdLibrary_Click);
			// 
			// frmSamples
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(394, 408);
			this.Controls.Add(this.cmdLibrary);
			this.Controls.Add(this.cmdEasy2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.cmdDataLink);
			this.Controls.Add(this.cmdEasy);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmSamples";
			this.Text = "FileHelpers Library - Samples ";
			this.Controls.SetChildIndex(this.pictureBox2, 0);
			this.Controls.SetChildIndex(this.cmdEasy, 0);
			this.Controls.SetChildIndex(this.cmdDataLink, 0);
			this.Controls.SetChildIndex(this.button1, 0);
			this.Controls.SetChildIndex(this.cmdEasy2, 0);
			this.Controls.SetChildIndex(this.cmdLibrary, 0);
			this.ResumeLayout(false);

		}

		#endregion

		private void cmdEasy_Click(object sender, EventArgs e)
		{
			frmEasySampleDelimited frm = new frmEasySampleDelimited();
			frm.ShowDialog();
			frm.Dispose();
		}

		private void cmdDataLink_Click(object sender, EventArgs e)
		{
			frmDataLinkSample frm = new frmDataLinkSample();
			frm.ShowDialog();
			frm.Dispose();

		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cmdEasy2_Click(object sender, EventArgs e)
		{
			frmEasySampleFixed frm = new frmEasySampleFixed();
			frm.ShowDialog();
			frm.Dispose();
		}

		private void cmdLibrary_Click(object sender, EventArgs e)
		{
			frmTimming frm = new frmTimming();
			frm.ShowDialog();
			frm.Dispose();
		}

	}
}