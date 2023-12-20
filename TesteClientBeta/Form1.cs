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
            // Inicia o processo de download e instalação da nova versão
            DownloadAndInstallUpdate();
        }

        private void DownloadAndInstallUpdate()
        {
            // Substitua "SeuLinkDeDownload" pelo link real do seu instalador
            string downloadUrl = "SeuLinkDeDownload";
            string downloadPath = Path.Combine(Path.GetTempPath(), "SeuAplicativoSetup.exe"); // Nome do arquivo temporário

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(downloadUrl, downloadPath);

                    // Inicia o processo de instalação
                    Process.Start(downloadPath);

                    // Fecha o aplicativo atual, se desejado
                    // Close(); 
                }
                catch (Exception ex)
                {
                    // Lida com erros durante o download ou instalação
                    MessageBox.Show($"Erro ao fazer o download ou instalar a atualização: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}