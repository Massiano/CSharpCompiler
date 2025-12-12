
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickPaste
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    class MainForm : Form
    {
        private readonly TextBox _delayBox;
        private readonly TextBox _textBox;
        private readonly Button _startBtn;
        private CancellationTokenSource? _cts;

        public MainForm()
        {
            Text = "QuickPaste";
            Size = new Size(400, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var lblDelay = new Label
            {
                Text = "Delay (seconds):",
                Location = new Point(10, 10),
                Width = 100
            };

            _delayBox = new TextBox
            {
                Text = "60",
                Location = new Point(120, 10),
                Width = 50
            };

            var lblText = new Label
            {
                Text = "Lines to paste:",
                Location = new Point(10, 40),
                Width = 100
            };

            _textBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 70),
                Size = new Size(360, 140)
            };

            _startBtn = new Button
            {
                Text = "Start",
                Location = new Point(10, 220),
                Width = 80
            };
            _startBtn.Click += StartClick;

            Controls.Add(lblDelay);
            Controls.Add(_delayBox);
            Controls.Add(lblText);
            Controls.Add(_textBox);
            Controls.Add(_startBtn);
        }

        private void StartClick(object? sender, EventArgs e)
        {
            if (!int.TryParse(_delayBox.Text, out var delaySec) || delaySec < 1)
            {
                MessageBox.Show("Enter a valid delay (≥1 s).");
                return;
            }

            var lines = _textBox.Lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            if (lines.Length == 0) return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _startBtn.Enabled = false;

            Task.Run(() => Worker(lines, delaySec, _cts.Token), _cts.Token)
                .ContinueWith(_ => BeginInvoke(() => _startBtn.Enabled = true), TaskScheduler.Current);
        }

        private static void Worker(string[] lines, int delaySec, CancellationToken token)
        {
            foreach (var line in lines)
            {
                if (token.IsCancellationRequested) break;

                SendKeys.SendWait(line);
                SendKeys.SendWait("{ENTER}");

                Thread.Sleep(delaySec * 1000);
            }
        }
    }
}
