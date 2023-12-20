using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TesteClientBeta
{
    public partial class Form1 : Form
    {
        private FileSystemWatcher fileWatcher;

        public Form1()
        {
            InitializeComponent();
            lblCurrentVersion.Text = $"Versão Atual: {AppConfig.AppVersion}";

            // Configurar o FileSystemWatcher para monitorar alterações no arquivo de versão
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = @"D:\xampp\htdocs\versao\"; // Substitua pelo caminho real do seu arquivo
            fileWatcher.Filter = "versao.txt";
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.EnableRaisingEvents = true;

            // Inicializar a verificação de atualizações
            CheckForUpdates();
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // O arquivo de versão foi alterado, verificar atualizações
            this.Invoke((MethodInvoker)delegate
            {
                CheckForUpdates();
            });
        }

        private void CheckForUpdates()
        {
            string latestVersion = UpdateChecker.GetLatestVersion();

            if (latestVersion != null && latestVersion != AppConfig.AppVersion)
            {
                lblUpdateAvailable.Text = "Nova versão disponível!";
                lblLatestVersion.Text = $"Versão mais recente: {latestVersion}";
            }
            else
            {
                lblUpdateAvailable.Text = "Você está usando a versão mais recente.";
                lblLatestVersion.Text = "";
            }
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            // Força a verificação de atualizações quando o botão é clicado
            CheckForUpdates();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Substitua "SeuRepositorio" pelo seu nome de usuário / nome do repositório no GitHub
            string owner = "cauanlbp";
            string repo = "TesteGameClient";
            string releaseUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

            // Obtém as informações da última release
            string downloadUrl = GetLatestReleaseDownloadUrl(releaseUrl);

            if (!string.IsNullOrEmpty(downloadUrl))
            {
                // Inicia o processo de download e instalação da nova versão
                DownloadAndInstallUpdate(downloadUrl);
            }
            else
            {
                MessageBox.Show("Não foi possível obter o link de download da última versão.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLatestReleaseDownloadUrl(string releaseUrl)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    // Define o cabeçalho para a requisição à API do GitHub
                    client.Headers.Add("User-Agent", "request");

                    // Faz a requisição à API do GitHub para obter as informações da última release
                    string releaseJson = client.DownloadString(releaseUrl);

                    // Analisa o JSON para obter o link de download do ativo (asset) do release
                    dynamic releaseInfo = JsonConvert.DeserializeObject(releaseJson);
                    string downloadUrl = releaseInfo.assets[0].browser_download_url;

                    return downloadUrl;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao obter informações da última release: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void DownloadAndInstallUpdate(string downloadUrl)
        {
            try
            {
                string downloadPath = Path.Combine(Path.GetTempPath(), "TesteClientBeta.exe"); // Nome do arquivo temporário

                using (WebClient client = new WebClient())
                {
                    // Faz o download do arquivo da última release
                    client.DownloadFile(downloadUrl, downloadPath);

                    // Inicia o processo de instalação
                    Process.Start(downloadPath);

                    // Fecha o aplicativo atual, se desejado
                    // Close();
                }
            }
            catch (Exception ex)
            {
                // Lida com erros durante o download ou instalação
                MessageBox.Show($"Erro ao fazer o download ou instalar a atualização: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}