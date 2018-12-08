using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptureDesktop.Model
{
	class TopForm : Form
	{
		bool inProgress = false;
		Panel p = new Panel();

		public Rectangle AreaBounds
		{
			get {
				return p.Bounds;
			}
		}

		public int t, l, w, h = 0;

		public TopForm()
		{
			WindowState = FormWindowState.Maximized;
			FormBorderStyle = FormBorderStyle.None;
			Bounds = Screen.PrimaryScreen.Bounds;
			BackColor = Color.White;
			TransparencyKey = Color.Gray;
			Opacity = 0.4;
			p.BackColor = Color.Gray;
			p.BorderStyle = BorderStyle.FixedSingle;
			Cursor = Cursors.Cross;
			FormClosing += TopForm_FormClosing;
			MouseDown += TopForm_MouseDown;
			MouseUp += TopForm_MouseUp;
		}

		private void TopForm_MouseUp(object sender, MouseEventArgs e)
		{
			if (inProgress)
			{
				w = p.Width;
				h = p.Height;
				MouseMove -= TopForm_MouseMove;
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void TopForm_MouseDown(object sender, MouseEventArgs e)
		{
			if (!inProgress)
			{
				inProgress = true;
				Controls.Add(p);
				p.Left = l = e.X;
				p.Top = t = e.Y;
				MouseMove += TopForm_MouseMove;
            }
		}

		private void TopForm_MouseMove(object sender, MouseEventArgs e)
		{
			SuspendLayout();
			p.SuspendLayout();
			p.Width = e.X - p.Left;
			p.Height = e.Y - p.Top;
			p.ResumeLayout();
			ResumeLayout();

		}

		private void TopForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Cursor = Cursors.Arrow;
		}
	}
}
