using static CertificatesHelper;
using static XmlSigner;
using static FileHelper;
using System.Security.Cryptography.X509Certificates;

var fullInputPath = GetPathFromUser("Podaj plik do podpisania i naciśnij Enter");

//creating output  file path
var fullOutputPath = BuildOutputPath(fullInputPath);

//wybor rodzaju wczytania certyfikatu
CertLoadMethod loadMethod = ChooseCertLoadMethod();

//wczytanie certyfikatu
X509Certificate2? certificate = loadMethod switch
{
    CertLoadMethod.FromFile => GetCertFromFile(),
    CertLoadMethod.FromStore => GetCertFromStore(),
    _ => throw new InvalidOperationException($"Nieobsłużona wartość: {loadMethod}")
};

if (certificate == null)
    return;

    //podpisanie Xmla
await SignXmlAsync(fullInputPath, fullOutputPath, certificate);
if (!VerifySignedXml(fullOutputPath))
    return;

Console.WriteLine($"OK {fullOutputPath}");
return;