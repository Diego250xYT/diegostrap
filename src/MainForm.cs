using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiegoStrap
{
    internal sealed class MainForm : Form
    {
        private readonly string[] startupArgs;
        private readonly TabControl mainTabs;
        private readonly TabPage downloadTab;
        private readonly TabPage accountsTab;
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
        private readonly ListBox accountsListBox;
        private readonly TextBox accountUserTextBox;
        private readonly TextBox accountPasswordTextBox;
        private readonly Button addAccountButton;
        private readonly Button removeAccountButton;
        private readonly Label accountsNoteLabel;
        private readonly Button launchInstanceButton;
        private readonly Button multiInstanceButton;
        private readonly Label multiInstancePathLabel;
        private readonly GroupBox protocolGroupBox;
        private readonly Label protocolStatusLabel;
        private readonly Button registerProtocolButton;
        private readonly Button restoreOfficialButton;
        private readonly Button unregisterProtocolButton;
        private readonly Button verifyProtocolButton;

        private readonly ProtocolHandlerRegistrar protocolRegistrar;

        private readonly List<string> accountUsernames;

        public MainForm(string[] args)
        {
            startupArgs = args ?? Array.Empty<string>();
            Text = "DiegoStrap";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            ClientSize = new Size(580, 430);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

            accountUsernames = AccountStore.LoadUsernames();
            protocolRegistrar = new ProtocolHandlerRegistrar(Application.ExecutablePath);

            mainTabs = new TabControl
            {
                Dock = DockStyle.Fill
            };

            downloadTab = new TabPage("Download");
            accountsTab = new TabPage("Accounts");

            versionModeGroupBox = new GroupBox
            {
                Text = "Version selection",
                Location = new Point(10, 12),
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
                Location = new Point(10, 138)
            };

            binaryTypeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(134, 134),
                Width = 140
            };
            binaryTypeComboBox.Items.AddRange(new object[] { "WindowsPlayer", "WindowsStudio64" });
            binaryTypeComboBox.SelectedIndex = 0;

            cleanLogsButton = new Button
            {
                Text = "Delete Logs",
                Location = new Point(10, 176),
                Size = new Size(100, 30)
            };
            cleanLogsButton.Click += CleanLogsButton_Click;

            cleanLocalStorageButton = new Button
            {
                Text = "Delete LocalStorage",
                Location = new Point(122, 176),
                Size = new Size(140, 30)
            };
            cleanLocalStorageButton.Click += CleanLocalStorageButton_Click;

            cleanRobloxFolderButton = new Button
            {
                Text = "Delete Roblox",
                Location = new Point(274, 176),
                Size = new Size(110, 30)
            };
            cleanRobloxFolderButton.Click += CleanRobloxFolderButton_Click;

            downloadButton = new Button
            {
                Text = "Download",
                Width = 100,
                Height = 32,
                Location = new Point(10, 222)
            };
            downloadButton.Click += DownloadButton_Click;

            progressBar = new ProgressBar
            {
                Location = new Point(16, 280),
                Size = new Size(520, 20),
                Minimum = 0,
                Maximum = 100
            };

            statusLabel = new Label
            {
                Text = "Ready.",
                AutoSize = false,
                Location = new Point(10, 308),
                Size = new Size(520, 20)
            };

            outputTextBox = new TextBox
            {
                Location = new Point(10, 332),
                Size = new Size(520, 20),
                ReadOnly = true
            };

            downloadTab.Controls.Add(versionModeGroupBox);
            downloadTab.Controls.Add(binaryTypeLabel);
            downloadTab.Controls.Add(binaryTypeComboBox);
            downloadTab.Controls.Add(cleanLogsButton);
            downloadTab.Controls.Add(cleanLocalStorageButton);
            downloadTab.Controls.Add(cleanRobloxFolderButton);
            downloadTab.Controls.Add(downloadButton);
            downloadTab.Controls.Add(progressBar);
            downloadTab.Controls.Add(statusLabel);
            downloadTab.Controls.Add(outputTextBox);

            accountsListBox = new ListBox
            {
                Location = new Point(12, 18),
                Size = new Size(270, 260)
            };

            Label accountUserLabel = new Label
            {
                Text = "User",
                AutoSize = true,
                Location = new Point(300, 18)
            };

            accountUserTextBox = new TextBox
            {
                Location = new Point(300, 38),
                Size = new Size(240, 23)
            };

            Label accountPasswordLabel = new Label
            {
                Text = "Password",
                AutoSize = true,
                Location = new Point(300, 76)
            };

            accountPasswordTextBox = new TextBox
            {
                Location = new Point(300, 96),
                Size = new Size(240, 23),
                UseSystemPasswordChar = true
            };

            addAccountButton = new Button
            {
                Text = "Add account",
                Location = new Point(300, 134),
                Size = new Size(110, 30)
            };
            addAccountButton.Click += AddAccountButton_Click;

            removeAccountButton = new Button
            {
                Text = "Remove",
                Location = new Point(430, 134),
                Size = new Size(110, 30)
            };
            removeAccountButton.Click += RemoveAccountButton_Click;

            launchInstanceButton = new Button
            {
                Text = "Launch Roblox",
                Location = new Point(300, 176),
                Size = new Size(240, 32)
            };
            launchInstanceButton.Click += LaunchInstanceButton_Click;

            multiInstanceButton = new Button
            {
                Text = "Enable multi-instance",
                Location = new Point(300, 216),
                Size = new Size(240, 32)
            };
            multiInstanceButton.Click += MultiInstanceButton_Click;

            multiInstancePathLabel = new Label
            {
                Text = BuildRobloxPathLabel(),
                AutoSize = false,
                Location = new Point(300, 252),
                Size = new Size(240, 24)
            };

            accountsNoteLabel = new Label
            {
                Text = "Password is used only for this session and is not saved.",
                AutoSize = false,
                Location = new Point(300, 280),
                Size = new Size(240, 18)
            };

            protocolGroupBox = new GroupBox
            {
                Text = "Protocol handlers",
                Location = new Point(12, 298),
                Size = new Size(528, 130)
            };

            protocolStatusLabel = new Label
            {
                Text = protocolRegistrar.GetStatus(),
                AutoSize = false,
                Location = new Point(14, 24),
                Size = new Size(500, 20)
            };

            registerProtocolButton = new Button
            {
                Text = "Register this launcher",
                Location = new Point(14, 54),
                Size = new Size(130, 30)
            };
            registerProtocolButton.Click += RegisterProtocolButton_Click;

            restoreOfficialButton = new Button
            {
                Text = "Restore official Roblox",
                Location = new Point(152, 54),
                Size = new Size(130, 30)
            };
            restoreOfficialButton.Click += RestoreOfficialButton_Click;

            unregisterProtocolButton = new Button
            {
                Text = "Remove handler",
                Location = new Point(290, 54),
                Size = new Size(110, 30)
            };
            unregisterProtocolButton.Click += UnregisterProtocolButton_Click;

            verifyProtocolButton = new Button
            {
                Text = "Verify state",
                Location = new Point(408, 54),
                Size = new Size(100, 30)
            };
            verifyProtocolButton.Click += VerifyProtocolButton_Click;

            protocolGroupBox.Controls.Add(protocolStatusLabel);
            protocolGroupBox.Controls.Add(registerProtocolButton);
            protocolGroupBox.Controls.Add(restoreOfficialButton);
            protocolGroupBox.Controls.Add(unregisterProtocolButton);
            protocolGroupBox.Controls.Add(verifyProtocolButton);

            accountsTab.Controls.Add(accountsListBox);
            accountsTab.Controls.Add(accountUserLabel);
            accountsTab.Controls.Add(accountUserTextBox);
            accountsTab.Controls.Add(accountPasswordLabel);
            accountsTab.Controls.Add(accountPasswordTextBox);
            accountsTab.Controls.Add(addAccountButton);
            accountsTab.Controls.Add(removeAccountButton);
            accountsTab.Controls.Add(launchInstanceButton);
            accountsTab.Controls.Add(multiInstanceButton);
            accountsTab.Controls.Add(multiInstancePathLabel);
            accountsTab.Controls.Add(accountsNoteLabel);
            accountsTab.Controls.Add(protocolGroupBox);

            mainTabs.TabPages.Add(downloadTab);
            mainTabs.TabPages.Add(accountsTab);
            Controls.Add(mainTabs);

            Shown += MainForm_Shown;
            UpdateVersionModeControls();
            RefreshAccountsList();
            RefreshProtocolStatus();
        }

        private void MainForm_Shown(object? sender, EventArgs e)
        {
            if (TryGetStartupProtocolUri(out string protocolUri))
            {
                HandleProtocolLaunch(protocolUri);
                BeginInvoke(new Action(Close));
            }
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

        private void AddAccountButton_Click(object? sender, EventArgs e)
        {
            string user = (accountUserTextBox.Text ?? string.Empty).Trim();
            string password = accountPasswordTextBox.Text ?? string.Empty;

            if (user.Length == 0)
            {
                MessageBox.Show(this, "Enter a user name.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length == 0)
            {
                MessageBox.Show(this, "Enter a password for this session.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!HasUserName(user))
            {
                accountUsernames.Add(user);
                AccountStore.SaveUsernames(accountUsernames);
                RefreshAccountsList();
            }

            accountPasswordTextBox.Clear();
            accountsListBox.SelectedItem = user;
        }

        private void RemoveAccountButton_Click(object? sender, EventArgs e)
        {
            string selected = accountsListBox.SelectedItem?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            if (MessageBox.Show(this, "Remove " + selected + "?", "Confirm remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            accountUsernames.RemoveAll(value => string.Equals(value, selected, StringComparison.OrdinalIgnoreCase));
            AccountStore.SaveUsernames(accountUsernames);
            RefreshAccountsList();
        }

        private void LaunchInstanceButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (MultiInstanceManager.IsRobloxRunning())
                {
                    DialogResult confirm = MessageBox.Show(this, "Roblox is already running. Launch another instance anyway?", "Confirm launch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes)
                    {
                        return;
                    }
                }

                MultiInstanceManager.LaunchRoblox();
                MessageBox.Show(this, "Roblox launched.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MultiInstanceButton_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Select RobloxPlayerBeta.exe";
                dialog.Filter = "RobloxPlayerBeta.exe|RobloxPlayerBeta.exe|Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.FileName = "RobloxPlayerBeta.exe";

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                string fileName = Path.GetFileName(dialog.FileName);
                if (!string.Equals(fileName, "RobloxPlayerBeta.exe", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this, "Select RobloxPlayerBeta.exe.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                MultiInstanceManager.SaveRobloxPlayerPath(dialog.FileName);
                multiInstancePathLabel.Text = BuildRobloxPathLabel();
                MessageBox.Show(this, "RobloxPlayerBeta.exe saved.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RegisterProtocolButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (protocolRegistrar.IsOwnedByAnotherApp(out string? owner))
                {
                    DialogResult result = MessageBox.Show(this, "Another app owns the Roblox protocol: " + (owner ?? "Unknown") + ". Replace it?", "Confirm overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                protocolRegistrar.RegisterProtocols();
                protocolStatusLabel.Text = protocolRegistrar.GetStatus();
                MessageBox.Show(this, "Protocol handlers registered.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearProtocolButtonFocus();
            }
        }

        private void RestoreOfficialButton_Click(object? sender, EventArgs e)
        {
            try
            {
                protocolRegistrar.RestoreDefaultRobloxHandler();
                protocolStatusLabel.Text = protocolRegistrar.GetStatus();
                MessageBox.Show(this, "Roblox handler restored.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearProtocolButtonFocus();
            }
        }

        private void UnregisterProtocolButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(this, "Remove the protocol handlers registered by this app?", "Confirm removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                {
                    return;
                }

                protocolRegistrar.UnregisterProtocols();
                protocolStatusLabel.Text = protocolRegistrar.GetStatus();
                MessageBox.Show(this, "Protocol handlers removed.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ClearProtocolButtonFocus();
            }
        }

        private void VerifyProtocolButton_Click(object? sender, EventArgs e)
        {
            RefreshProtocolStatus();
            MessageBox.Show(this, protocolStatusLabel.Text, "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearProtocolButtonFocus();
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

        private void RefreshAccountsList()
        {
            accountsListBox.Items.Clear();
            for (int i = 0; i < accountUsernames.Count; i++)
            {
                accountsListBox.Items.Add(accountUsernames[i]);
            }
        }

        private void RefreshProtocolStatus()
        {
            protocolStatusLabel.Text = protocolRegistrar.GetStatus();
        }

        private void ClearProtocolButtonFocus()
        {
            BeginInvoke(new Action(() => ActiveControl = mainTabs));
        }

        private static string BuildRobloxPathLabel()
        {
            string path = MultiInstanceManager.LoadSavedRobloxPlayerPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                return "No RobloxPlayerBeta.exe saved yet.";
            }

            return "Saved path: " + path;
        }

        private bool TryGetStartupProtocolUri(out string protocolUri)
        {
            protocolUri = string.Empty;

            for (int i = 0; i < startupArgs.Length; i++)
            {
                string argument = startupArgs[i] ?? string.Empty;
                if (argument.StartsWith("roblox://", StringComparison.OrdinalIgnoreCase) || argument.StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
                {
                    protocolUri = argument;
                    return true;
                }
            }

            return false;
        }

        private void HandleProtocolLaunch(string protocolUri)
        {
            string robloxPlayerPath = RobloxLocator.ResolveExecutable();
            if (string.IsNullOrWhiteSpace(robloxPlayerPath))
            {
                MessageBox.Show(this, "RobloxPlayerBeta.exe was not found. Install Roblox before launching a protocol.", "DiegoStrap", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = robloxPlayerPath,
                Arguments = "\"" + protocolUri + "\"",
                WorkingDirectory = Path.GetDirectoryName(robloxPlayerPath) ?? string.Empty,
                UseShellExecute = false
            };

            Process.Start(startInfo);
        }

        private bool HasUserName(string userName)
        {
            for (int i = 0; i < accountUsernames.Count; i++)
            {
                if (string.Equals(accountUsernames[i], userName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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