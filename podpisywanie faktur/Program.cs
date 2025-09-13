using podpisywanie_faktur;
using System.Security.Cryptography.X509Certificates;
using static CertificatesHelper;
using static ChoiceHelper;
using static FileHelper;
using static XmlSigner;
using static podpisywanie_faktur.InvoiceFactory;


//wybor sposobu wczytania certyfikatu
CertLoadMethod loadMethod = ChoiceMenu<CertLoadMethod>("Wybierz sposób załadowania certyfikatu i naciśnij Enter");

//wczytanie certyfikatu
X509Certificate2? certificate = loadMethod switch
{
    CertLoadMethod.FromFile => GetCertFromFile(),
    CertLoadMethod.FromStore => GetCertFromStore(),
    _ => throw new InvalidOperationException($"Nieobsłużona wartość: {loadMethod}")
};

//weryfikacja certyfikatu - po tym momencie certificate =/= null
if (!ValidateCertificate(certificate))
    return;



//wybor formy podpisywania faktur
InputMethod inputMethod = ChoiceMenu<InputMethod>("Wybierz sposób podpisania faktur i naciśnij Enter");

//wybor faktury/folderu faktur do podpisania
var fullInputPath = inputMethod switch
{
    InputMethod.SingleFile => GetPathFromUser("Podaj plik do podpisania i naciśnij Enter", true),
    InputMethod.FullDir => GetDirFromUser("Podaj folder zawierający faktury do podpisania i naciśnij Enter"),
    _ => throw new InvalidOperationException($"Nieobsłużona wartość: {inputMethod}")
};


if(Directory.Exists(fullInputPath))
{
    //lista nazw faktur w folderze wejsciowym
    IEnumerable<Invoice> invoices = FromDirectory(fullInputPath);
    //liczniki udanych i nieudanych operacji
    int signed = 0;
    int failed = 0;

    foreach (var invoice in invoices)
    {
        try
        {
            //podpisanie pliku
            await SignXmlAsync(invoice.inputPath, invoice.outputPath, certificate!);

            //nieudana weryfikacja podpisu
            if (!VerifySignedXml(invoice.outputPath))
            {
                Console.WriteLine($"Błąd weryfikacji podpisu: {invoice.outputPath}");
                failed++;
            }

            //udana weryfikacja podpisu
            Console.WriteLine($"OK {invoice.outputPath}");
            signed++;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Błąd: {invoice.outputPath} -> {exception.Message}");
            failed++;
        }
    }

    //podsumowanie, zakonczenie dzialania aplikacji
    Console.WriteLine($"Zakończono podpisywanie z {failed} błędami i {signed} udanymi operacjami");
    return;
}
else
{
    //wybor rodzaju utworzenia nazwy podpisanej faktury
    OutputPathMethod outputMethod = ChoiceMenu<OutputPathMethod>("Wybierz, jak nazwać podpisaną fakturę i naciśnij Enter");

    //stworzenie sciezki podpisanej faktury
    var fullOutputPath = outputMethod switch
    {
        OutputPathMethod.Auto => AutoOutputPath(fullInputPath, false, null),
        OutputPathMethod.Manual => GetPathFromUser("Podaj nazwę lub pełną ścieżkę zapisu podpisanej faktury i naciśnij Enter", false),
        OutputPathMethod.AutoToFolder => AutoOutputPath(fullInputPath, true, null),
        _ => throw new InvalidOperationException($"Nieobsłużona wartość: {outputMethod}")
    };


    //podpisanie Xmla
    try
    {
        //podpisanie
        await SignXmlAsync(fullInputPath, fullOutputPath, certificate);

        //nieudana weryfikacja podpisu
        if (!VerifySignedXml(fullOutputPath))
        {
            Console.WriteLine("Błąd weryfikacji podpisu");
            return;
        }

        //udana weryfikacja podpisu
        Console.WriteLine($"OK {fullOutputPath}");
        return;
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Błąd: {exception.Message}");
    }
}