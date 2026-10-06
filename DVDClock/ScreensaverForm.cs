using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DVDClock
{
    public class ScreensaverForm : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private Point _mouseLocation;
        private bool _isPreviewMode;
        private System.Windows.Forms.Timer _timer;
        private Settings _settings;
        
        private float _x, _y;
        private float _dx, _dy;
        
        private string _timeString = "00:00";
        private Font _clockFont;
        private SolidBrush _clockBrush;
        private SizeF _textSize;
        
        private Random _random = new Random();
        private bool _flickerVisible = true; // toggles every tick when flicker is enabled

        public ScreensaverForm(Rectangle bounds)
        {
            _settings = Settings.Load();
            InitializeScreensaver(bounds);
        }

        public ScreensaverForm(IntPtr previewWndHandle)
        {
            _settings = Settings.Load();
            _isPreviewMode = true;

            SetParent(this.Handle, previewWndHandle);
            SetWindowLong(this.Handle, -16, new IntPtr(GetWindowLong(this.Handle, -16) | 0x40000000).ToInt32());

            RECT parentRect;
            GetClientRect(previewWndHandle, out parentRect);
            Size parentSize = new Size(parentRect.Right - parentRect.Left, parentRect.Bottom - parentRect.Top);

            InitializeScreensaver(new Rectangle(Point.Empty, parentSize));
        }

        private void InitializeScreensaver(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;

            if (!_isPreviewMode)
            {
                Cursor.Hide();
                this.TopMost = true;
                
                this.MouseMove += ScreensaverForm_MouseMove;
                this.MouseDown += ScreensaverForm_MouseClick;
                this.KeyDown += ScreensaverForm_KeyDown;
            }

            int speed = _settings.GetSpeedMultiplier();
            _dx = speed * (_random.Next(2) == 0 ? 1 : -1);
            _dy = speed * (_random.Next(2) == 0 ? 1 : -1);

            int fontSize = Math.Max(12, (int)(bounds.Width * 0.08));
            if (_isPreviewMode) 
            {
                fontSize = Math.Max(12, (int)(bounds.Width * 0.15));
            }
            
            _clockFont = new Font("Segoe UI Light", fontSize, GraphicsUnit.Pixel);
            _clockBrush = new SolidBrush(_settings.GetClockColor());
            
            UpdateTimeString();

            _x = _random.Next(0, Math.Max(1, bounds.Width - (int)_textSize.Width));
            _y = _random.Next(0, Math.Max(1, bounds.Height - (int)_textSize.Height));

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 10; // ~60fps+ (WinForms timers usually snap to ~15ms system clock)
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void UpdateTimeString()
        {
            string newTime = DateTime.Now.ToString("HH:mm");
            if (newTime != _timeString || _textSize.Width == 0)
            {
                _timeString = newTime;
                using var g = this.CreateGraphics();
                _textSize = g.MeasureString(_timeString, _clockFont);
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_settings.FlickerEnabled)
                _flickerVisible = !_flickerVisible;

            UpdateTimeString();

            _x += _dx;
            _y += _dy;

            if (_x <= 0)
            {
                _x = 0;
                _dx = Math.Abs(_dx);
            }
            else if (_x + _textSize.Width >= this.Width)
            {
                _x = this.Width - _textSize.Width;
                _dx = -Math.Abs(_dx);
            }

            if (_y <= 0)
            {
                _y = 0;
                _dy = Math.Abs(_dy);
            }
            else if (_y + _textSize.Height >= this.Height)
            {
                _y = this.Height - _textSize.Height;
                _dy = -Math.Abs(_dy);
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_flickerVisible) return;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.DrawString(_timeString, _clockFont, _clockBrush, _x, _y);
        }

        private void ScreensaverForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_mouseLocation.IsEmpty)
            {
                if (Math.Abs(_mouseLocation.X - e.X) > 5 || Math.Abs(_mouseLocation.Y - e.Y) > 5)
                {
                    Application.Exit();
                }
            }
            _mouseLocation = e.Location;
        }

        private void ScreensaverForm_MouseClick(object? sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void ScreensaverForm_KeyDown(object? sender, KeyEventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isPreviewMode) Cursor.Show();
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }
                _clockFont?.Dispose();
                _clockBrush?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
