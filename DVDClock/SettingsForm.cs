using System;
using System.Drawing;
using System.Windows.Forms;

namespace DVDClock
{
    public class SettingsForm : Form
    {
        private Settings _settings;
        private ComboBox _cmbSpeed;
        private Button _btnColor;
        private Label _lblPreview;
        private CheckBox _chkFlicker;
        private Button _btnSave;
        private Button _btnCancel;

        public SettingsForm()
        {
            _settings = Settings.Load();
            InitializeComponent();
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.Text = "Configurações do DVDClock";
            this.Size = new Size(350, 295);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblSpeed = new Label { Text = "Velocidade:", Location = new Point(20, 20), AutoSize = true };
            _cmbSpeed = new ComboBox
            {
                Location = new Point(120, 17),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 150
            };
            _cmbSpeed.Items.AddRange(new string[] { "Muito lenta", "Lenta", "Normal", "Rápida", "Muito rápida" });
            _cmbSpeed.SelectedIndex = _settings.SpeedIndex >= 0 && _settings.SpeedIndex < 5 ? _settings.SpeedIndex : 2;

            var lblColor = new Label { Text = "Cor do relógio:", Location = new Point(20, 60), AutoSize = true };
            _btnColor = new Button
            {
                Text = "Escolher Cor...",
                Location = new Point(120, 55),
                Width = 150
            };
            _btnColor.Click += BtnColor_Click;

            _lblPreview = new Label
            {
                Text = "23:48",
                Location = new Point(20, 100),
                Size = new Size(250, 50),
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Light", 24)
            };

            _chkFlicker = new CheckBox
            {
                Text = "Ativar flicker (piscar)",
                Location = new Point(20, 165),
                AutoSize = true,
                Checked = _settings.FlickerEnabled
            };

            _btnSave = new Button { Text = "Salvar", Location = new Point(140, 215), Width = 75 };
            _btnSave.Click += BtnSave_Click;
            
            _btnCancel = new Button { Text = "Cancelar", Location = new Point(225, 215), Width = 75 };
            _btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblSpeed);
            this.Controls.Add(_cmbSpeed);
            this.Controls.Add(lblColor);
            this.Controls.Add(_btnColor);
            this.Controls.Add(_lblPreview);
            this.Controls.Add(_chkFlicker);
            this.Controls.Add(_btnSave);
            this.Controls.Add(_btnCancel);
        }

        private void BtnColor_Click(object? sender, EventArgs e)
        {
            using var cd = new ColorDialog();
            cd.Color = _settings.GetClockColor();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                _settings.ClockColorHex = ColorTranslator.ToHtml(cd.Color);
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            _lblPreview.ForeColor = _settings.GetClockColor();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            _settings.SpeedIndex = _cmbSpeed.SelectedIndex;
            _settings.FlickerEnabled = _chkFlicker.Checked;
            _settings.Save();
            this.Close();
        }
    }
}
