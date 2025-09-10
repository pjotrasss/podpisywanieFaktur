using static CertificatesHelper;
using static XmlSigner;
using static FileHelper;
using static ChoiceHelper;
using System.Security.Cryptography.X509Certificates;

//wybor faktury do podpisania
var fullInputPath = GetPathFromUser("Podaj plik do podpisania i naciśnij Enter", true);

//wybor rodzaju utworzenia nazwy podpisanej faktury
OutputPathMethod outputMethod = ChoiceMenu<OutputPathMethod>("Wybierz, jak nazwać podpisaną fakturę i naciśnij Enter");

//stworzenie nazwy podpisanej faktury
var fullOutputPath = outputMethod switch
{
    OutputPathMethod.Auto => AutoOutputPath(fullInputPath),
    OutputPathMethod.Manual => GetPathFromUser("Podaj nazwę lub pełną ścieżkę zapisu podpisanej faktury i naciśnij Enter", false),
    _ => throw new InvalidOperationException($"Nieobsłużona wartość: {outputMethod}")
};

//wybor rodzaju wczytania certyfikatu
CertLoadMethod loadMethod = ChoiceMenu<CertLoadMethod>("Wybierz sposób załadowania certyfikatu i naciśnij Enter");

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