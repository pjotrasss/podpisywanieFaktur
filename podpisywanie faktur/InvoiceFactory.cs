using static FileHelper;

namespace podpisywanie_faktur;

internal class InvoiceFactory
{
    internal static IEnumerable<Invoice> FromDirectory(string directory)
    {
        var invoicePaths = Directory.EnumerateFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);

        var outputDirectory = PrepareOutputDirectory(directory);

        foreach (var path in invoicePaths)
        {
            if (Path.GetFileNameWithoutExtension(path).EndsWith("_signed"))
            {
                Console.WriteLine($"Pomijanie pliku {path} - oznaczony jako podpisany");
                continue;
            }

            yield return new Invoice
            {
                inputPath = path,
                outputPath = AutoOutputPath(path, false, outputDirectory),
            };
        }
    }
}