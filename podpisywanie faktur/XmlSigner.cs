using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Xml;
using KSeF.Client.Api.Services;

internal static class XmlSigner
{
    internal static async Task SignXmlAsync(string inputPath, string outputPath, X509Certificate2 certificate)
    {
        //wczytanie xmla ktory ma byc podpisany
        var xml = await File.ReadAllTextAsync(inputPath);
        if (string.IsNullOrWhiteSpace(xml))
            throw new InvalidDataException("Podpisywany plik XML jest pusty");

        //zaladowanie serwisu do podpisywania z klienta KSEF
        var signatureService = new SignatureService();
        //podpisanie xmla
        string signedXml = await signatureService.SignAsync(xml, certificate);

        //sprawdzenie czy podpisanie sie udalo
        if (signedXml != null)
            await File.WriteAllTextAsync(outputPath, signedXml);
        else
            throw new Exception("Wystąpił błąd podczas podpisywania pliku");
        return;
    }

    internal static bool VerifySignedXml(string filePath)
    {
        try
        {
            //wczytanie xmla
            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.Load(filePath);
            if (xmlDocument.DocumentElement == null) throw new InvalidOperationException("Wczytywanie XML z pliku nieudane");

            //znalezienie danych podpisu we wczytanym xmlu
            var signedXml = new SignedXml(xmlDocument);
            var nodeList = xmlDocument.GetElementsByTagName("Signature");
            
            //sprawdzenie czy istnieje element 'Signature' i czy nie jest pusty
            if (nodeList.Count == 0) throw new CryptographicException("Nie znaleziono elementu 'Signature' w pliku XML");
            if (nodeList[0] is not XmlElement signature) throw new CryptographicException("Elemnt 'Signature' pliku XML jest pusty");

            //wczytanie podpisu do obiektu
            signedXml.LoadXml(signature);
            //wlasciwa kryptograficzna weryfikacja podpisu
            return signedXml.CheckSignature();
        }
        catch (Exception exception)
        {
            throw new Exception($"Podpis pliku {filePath} jest nieprawidłowy", exception);
        }
    }
}