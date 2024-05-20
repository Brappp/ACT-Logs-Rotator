using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace LogsRotate
{
    public class UpdateChecker
    {
        private const string GitHubRepoUrl = "https://api.github.com/repos/Brappp/ACT-Logs-Rotator/releases/latest";
        private const string CurrentVersion = "3.8";

        public static async Task CheckForUpdates()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
                    HttpResponseMessage response = await client.GetAsync(GitHubRepoUrl);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var latestRelease = JObject.Parse(responseBody);
                    string latestVersion = latestRelease["tag_name"].ToString();
                    var assets = latestRelease["assets"] as JArray;

                    if (assets == null || assets.Count == 0)
                    {
                        MessageBox.Show("No assets found in the latest release.", "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string downloadUrl = string.Empty;

                    foreach (var asset in assets)
                    {
                        string assetName = asset["name"].ToString();
                        string assetUrl = asset["browser_download_url"].ToString();

                        if (assetName.EndsWith(".zip"))
                        {
                            downloadUrl = assetUrl;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        MessageBox.Show("No valid ZIP asset found in the latest release.", "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (IsNewVersionAvailable(CurrentVersion, latestVersion))
                    {
                        var result = MessageBox.Show($"A new version ({latestVersion}) of the LogRotatorPlugin is available. Do you want to download it?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(downloadUrl);
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Error checking for updates: {ex.Message}\nCurrent Version: {CurrentVersion}", "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for updates: {ex.Message}", "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool IsNewVersionAvailable(string currentVersion, string latestVersion)
        {
            try
            {
                Version current = new Version(currentVersion);
                Version latest = new Version(latestVersion);
                return latest > current;
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Error parsing version strings: {ex.Message}\nCurrent Version: {currentVersion}\nGitHub Version: {latestVersion}", "Version Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
