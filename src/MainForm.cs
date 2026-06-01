using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiegoStrap
{
    internal sealed class MainForm : Form
    {
        private readonly GroupBox versionModeGroupBox;
        private readonly RadioButton versionSourceRadioButton;
        private readonly RadioButton versionHashRadioButton;
        private readonly ComboBox versionSourceComboBox;
        private readonly ComboBox binaryTypeComboBox;
        private readonly TextBox versionHashTextBox;
        private readonly Button cleanLogsButton;
        private readonly Button cleanLocalStorageButton;
        private readonly Button cleanRobloxFolderButton;
        private readonly Button downloadButton;
        private readonly ProgressBar progressBar;
        private readonly Label statusLabel;
        private readonly TextBox outputTextBox;

        public MainForm()
        {
            Text = "DiegoStrap";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            ClientSize = new Size(560, 395);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            versionModeGroupBox = new GroupBox
            {
                Text = "Version selection",
                Location = new Point(16, 12),
                Size = new Size(528, 110)
            };

            versionSourceRadioButton = new RadioButton
            {
                Text = "Use version source",
                AutoSize = true,
                Location = new Point(14, 26),
                Checked = true
            };
            versionSourceRadioButton.CheckedChanged += VersionMode_CheckedChanged;

            versionHashRadioButton = new RadioButton
            {
                Text = "Use version hash",
                AutoSize = true,
                Location = new Point(14, 58)
            };
            versionHashRadioButton.CheckedChanged += VersionMode_CheckedChanged;

            Label versionSourceLabel = new Label
            {
                Text = "Version source",
                AutoSize = true,
                Location = new Point(190, 27)
            };

            versionSourceComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(290, 23),
                Width = 140
            };
            versionSourceComboBox.Items.AddRange(new object[] { "LIVE", "FUTURE", "PAST" });
            versionSourceComboBox.SelectedIndex = 0;

            Label versionHashLabel = new Label
            {
                Text = "Version hash",
                AutoSize = true,
                Location = new Point(190, 61)
            };

            versionHashTextBox = new TextBox
            {
                Location = new Point(290, 57),
                Width = 220,
                Enabled = false
            };

            versionModeGroupBox.Controls.Add(versionSourceRadioButton);
            versionModeGroupBox.Controls.Add(versionHashRadioButton);
            versionModeGroupBox.Controls.Add(versionSourceLabel);
            versionModeGroupBox.Controls.Add(versionSourceComboBox);
            versionModeGroupBox.Controls.Add(versionHashLabel);
            versionModeGroupBox.Controls.Add(versionHashTextBox);

            Label binaryTypeLabel = new Label
            {
                Text = "Binary type",
                AutoSize = true,
                Location = new Point(16, 138)
            };

            binaryTypeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(140, 134),
                Width = 140
            };
            binaryTypeComboBox.Items.AddRange(new object[] { "WindowsPlayer", "WindowsStudio64" });
            binaryTypeComboBox.SelectedIndex = 0;

            cleanLogsButton = new Button
            {
                Text = "Delete Logs",
                Location = new Point(16, 176),
                Size = new Size(100, 30)
            };
            cleanLogsButton.Click += CleanLogsButton_Click;

            cleanLocalStorageButton = new Button
            {
                Text = "Delete LocalStorage",
                Location = new Point(128, 176),
                Size = new Size(140, 30)
            };
            cleanLocalStorageButton.Click += CleanLocalStorageButton_Click;

            cleanRobloxFolderButton = new Button
            {
                Text = "Delete Roblox",
                Location = new Point(280, 176),
                Size = new Size(110, 30)
            };
            cleanRobloxFolderButton.Click += CleanRobloxFolderButton_Click;

            downloadButton = new Button
            {
                Text = "Download",
                Width = 100,
                Height = 32,
                Location = new Point(16, 222)
            };
            downloadButton.Click += DownloadButton_Click;

            progressBar = new ProgressBar
            {
                Location = new Point(16, 280),
                Size = new Size(484, 20),
                Minimum = 0,
                Maximum = 100
            };

            statusLabel = new Label
            {
                Text = "Ready.",
                AutoSize = false,
                Location = new Point(16, 308),
                Size = new Size(484, 20)
            };

            outputTextBox = new TextBox
            {
                Location = new Point(16, 314),
                Size = new Size(484, 20),
                ReadOnly = true
            };

            Controls.Add(versionModeGroupBox);
            Controls.Add(binaryTypeLabel);
            Controls.Add(binaryTypeComboBox);
            Controls.Add(cleanLogsButton);
            Controls.Add(cleanLocalStorageButton);
            Controls.Add(cleanRobloxFolderButton);
            Controls.Add(downloadButton);
            Controls.Add(progressBar);
            Controls.Add(statusLabel);
            Controls.Add(outputTextBox);

            UpdateVersionModeControls();
        }

        private async void DownloadButton_Click(object? sender, EventArgs e)
        {
            try
            {
                downloadButton.Enabled = false;
                progressBar.Value = 0;
                statusLabel.Text = "Starting...";
                outputTextBox.Text = string.Empty;

                if (versionHashRadioButton.Checked && string.IsNullOrWhiteSpace(versionHashTextBox.Text))
                {
                    throw new InvalidOperationException("Enter a version hash or choose a version source.");
                }

                DownloadRequest request = new DownloadRequest
                {
                    Channel = "LIVE",
                    BinaryType = binaryTypeComboBox.SelectedItem?.ToString() ?? "WindowsPlayer",
                    VersionSource = ParseVersionSource(versionSourceComboBox.SelectedItem?.ToString()),
                    UseVersionHash = versionHashRadioButton.Checked,
                    VersionHash = NormalizeVersionHash(versionHashTextBox.Text),
                };

                string outputDirectory = AppContext.BaseDirectory;
                string tempOutputPath = System.IO.Path.Combine(outputDirectory, "download.zip");

                if (System.IO.File.Exists(tempOutputPath))
                {
                    System.IO.File.Delete(tempOutputPath);
                }

                WindowsDownloader downloader = new WindowsDownloader();
                IProgress<DownloadProgressInfo> reporter = new Progress<DownloadProgressInfo>(UpdateProgress);
                string resolvedVersion = await downloader.DownloadAsync(request, tempOutputPath, CancellationToken.None, reporter).ConfigureAwait(true);
                string finalFileName = resolvedVersion + ".zip";
                string finalOutputPath = System.IO.Path.Combine(outputDirectory, finalFileName);

                if (System.IO.File.Exists(finalOutputPath))
                {
                    System.IO.File.Delete(finalOutputPath);
                }

                System.IO.File.Move(tempOutputPath, finalOutputPath);
                statusLabel.Text = "Done.";
                outputTextBox.Text = finalOutputPath;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Error.";
                MessageBox.Show(this, ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                downloadButton.Enabled = true;
            }
        }

        private void CleanLogsButton_Click(object? sender, EventArgs e)
        {
            if (ConfirmCleanup("Logs"))
            {
                bool deleted = CleanupRobloxData.DeleteLogs();
                MessageBox.Show(this, deleted ? "Logs deleted." : "Logs folder not found.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CleanLocalStorageButton_Click(object? sender, EventArgs e)
        {
            if (ConfirmCleanup("LocalStorage"))
            {
                bool deleted = CleanupRobloxData.DeleteLocalStorage();
                MessageBox.Show(this, deleted ? "LocalStorage deleted." : "LocalStorage folder not found.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CleanRobloxFolderButton_Click(object? sender, EventArgs e)
        {
            if (ConfirmCleanup("Roblox folder"))
            {
                bool deleted = CleanupRobloxData.DeleteRobloxFolder();
                MessageBox.Show(this, deleted ? "Roblox folder deleted." : "Roblox folder not found.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateProgress(DownloadProgressInfo info)
        {
            int percent = info.Percent;
            if (percent < 0)
            {
                percent = 0;
            }

            if (percent > 100)
            {
                percent = 100;
            }

            progressBar.Value = percent;
            string status = info.StatusText;
            if (!string.IsNullOrWhiteSpace(info.SpeedText))
            {
                status += " | " + info.SpeedText;
            }

            statusLabel.Text = status;
        }

        private bool ConfirmCleanup(string targetName)
        {
            DialogResult result = MessageBox.Show(this, "Delete " + targetName + "?", "Confirm cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return result == DialogResult.Yes;
        }

        private void VersionMode_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateVersionModeControls();
        }

        private void UpdateVersionModeControls()
        {
            bool useHash = versionHashRadioButton.Checked;
            versionSourceComboBox.Enabled = !useHash;
            versionHashTextBox.Enabled = useHash;
        }

        private static VersionSource ParseVersionSource(string? value)
        {
            string normalized = (value ?? string.Empty).Trim().ToLowerInvariant();

            if (normalized == "future")
            {
                return VersionSource.Future;
            }

            if (normalized == "past")
            {
                return VersionSource.Past;
            }

            return VersionSource.Live;
        }

        private static string NormalizeVersionHash(string? value)
        {
            string normalized = (value ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            if (!normalized.StartsWith("version-", StringComparison.OrdinalIgnoreCase))
            {
                normalized = "version-" + normalized;
            }

            return normalized.ToLowerInvariant();
        }
    }
}