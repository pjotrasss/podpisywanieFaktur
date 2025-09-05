using System.Security.Cryptography.X509Certificates;
using System.Text;
using static InputValidationHelper;
using static FileHelper;

internal static class CertificatesHelper
{
    internal enum CertLoadMethod
    {
        FromFile = 1,
        FromStore = 2
    }



    internal static CertLoadMethod ChooseCertLoadMethod()
    {
        while (true)
        {
            Console.WriteLine("Wybierz w jaki sposob zaladowania certyfikatu i kliknij Enter");
            Console.WriteLine("1: Zaladowanie certyfikatu z pliku");
            Console.WriteLine("2: Zaladowanie certyfikatu z magazynu systemowego");

            var userInput = Console.ReadLine();
            int? validatedChoice = ValidateIntInput(userInput, 1, 2);
            if (validatedChoice != null)
                return (CertLoadMethod)validatedChoice.Value;
        }
    }



    internal static X509Certificate2 GetCertFromFile()
    {
        while (true)
        {
            var fullPath = GetPathFromUser("Podaj ścieżkę do certyfikatu i naciśnij Enter");
            
            Console.WriteLine("Podaj hasło certyfikatu");

            //maskowanie hasla
            var passwordBuilder = new StringBuilder();
            while (true)
            {
                //'przechwycenie' wcisnietego klawisza i blokada wyswietlenia go w terminalu
                var key = Console.ReadKey(intercept: true);

                //przeskok do nowej lini i zakonczenie petli po wcisnieciu Enter
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                
                //usuniecie znaku po wcisnieciu backspace
                if (key.Key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
                {
                    passwordBuilder.Length--;
                    Console.Write("\b \b"); // usuniecie gwiazdki odpowiadajacej usuwanemu znakowi
                    continue;
                }

                //dodanie znaku do hasla jesli nie jest klawiszem sterujacym
                if (!char.IsControl(key.KeyChar))
                {
                    passwordBuilder.Append(key.KeyChar);
                    Console.Write('*');
                }
            }
            var password = passwordBuilder.ToString();

            try
            {
                return X509CertificateLoader.LoadPkcs12FromFile(fullPath, password);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Błąd podczas wczytywania certyfikatu: {exception.Message}");
                Console.WriteLine($"Spróbować ponownie? (t/n)");
                var input = Console.ReadLine();
                if (input?.ToLower() != "t")
                    throw;
            }
        }
    }



    internal static X509Certificate2? GetCertFromStore()
    {
        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        //docelowo valid only TRUE
        var certificates = store.Certificates.Find(
        X509FindType.FindByTimeValid, DateTime.Now, validOnly: false);

        //sprawdzenie czy do systemu dodano jakiekolwiek certyfikaty
        if (certificates.Count == 0)
        {
            Console.WriteLine("Brak certyfikatów w magazynie systemowym");
            return null;
        }

        while (true)
        {
            //wypisanie mozliwych do uzycia certyfikatow
            for (var i = 0; i < certificates.Count; i++)
            {
                var certificate = certificates[i];
                Console.WriteLine($"{i}: {certificate.Subject}");
            }
            Console.WriteLine("Wybierz certyfikat i kliknij Enter");

            var userInput = Console.ReadLine();
            var certInt = ValidateIntInput(userInput, 1, certificates.Count);
            if (certInt != null)
            {
                Console.WriteLine($"Wybrano certyfikat nr {certInt}");
                return certificates[certInt.Value];
            }
        }
    }
}