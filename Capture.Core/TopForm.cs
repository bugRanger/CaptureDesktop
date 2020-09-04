namespace Capture.Core
{
    using System.Drawing;
    using System.Windows.Forms;

    class TopForm : Form
    {
        private bool _inProgress;
        private readonly Panel _panel = new Panel();

        public Rectangle AreaBounds => _panel.Bounds;

        public int t, l, w, h = 0;

        public TopForm()
        {
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
            BackColor = Color.White;
            TransparencyKey = Color.Gray;
            Opacity = 0.4;
            _panel.BackColor = Color.Gray;
            _panel.BorderStyle = BorderStyle.FixedSingle;
            Cursor = Cursors.Cross;

            FormClosing += TopForm_FormClosing;
            MouseDown += TopForm_MouseDown;
            MouseMove += TopForm_MouseMove;
        }

        private void TopForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (!_inProgress)
            {
                Controls.Add(_panel);
                _panel.Left = l = e.X;
                _panel.Top = t = e.Y;

                _inProgress = true;
            }
            else
            {
                w = _panel.Width;
                h = _panel.Height;

                _inProgress = false;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void TopForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_inProgress)
                return;

            SuspendLayout();
            _panel.SuspendLayout();
            _panel.Width = e.X - _panel.Left;
            _panel.Height = e.Y - _panel.Top;
            _panel.ResumeLayout();
            ResumeLayout();
        }

        private void TopForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }
    }
}
