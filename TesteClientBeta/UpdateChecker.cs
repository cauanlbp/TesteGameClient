using System;
using System.Net;

public class UpdateChecker
{
    private const string UpdateUrl = "http://localhost/versao/versao.txt";

    public static string GetLatestVersion()
    {
        using (WebClient client = new WebClient())
        {
            try
            {
                return client.DownloadString(UpdateUrl).Trim();
            }
            catch (Exception ex)
            {
                // Lida com erros (pode não haver conexão com a internet, servidor indisponível, etc.)
                return null;
            }
        }
    }
}
