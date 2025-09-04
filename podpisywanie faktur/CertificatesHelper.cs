using System.Security.Cryptography.X509Certificates;

internal static class CertificatesHelper
{
    internal static X509Certificate2? CurrentUser()
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        //docelowo valid only TRUE
        var certificates = store.Certificates.Find(
        X509FindType.FindByTimeValid, DateTime.Now, validOnly: false);

        //sprawdzenie czy do systemu dodano jakiekolwiek certyfikaty
        if (certificates.Count == 0)
        {
            Console.WriteLine("Brak Certyfikatów");
            return null;
        }

        //wypisanie mozliwych do uzycia certyfikatow
        for (var i = 0; i <  certificates.Count; i++)
        {
            var certificate = certificates[i];
            Console.WriteLine($"{i}: {certificate.Subject}");
        }
        Console.WriteLine("Wybierz certyfikat i kliknij Enter");

        var userInput = Console.ReadLine();
        if (userInput == null)
        {
            Console.WriteLine("Nie wybrano certyfikatu");
            return null;
        }

        if (!int.TryParse(userInput, out int certInt))
        {
            Console.WriteLine("Niepoprawny typ danych - oczekiwano liczby");
            return null;
        }

        if (!(certInt >= 0 && certInt < certificates.Count))
        {
            Console.WriteLine("Wybrano ceretyfikat spoza zakresu");
            return null;
        }
        Console.WriteLine($"Wybrano certyfikat nr {certInt}");
        return certificates[certInt];
        }
}