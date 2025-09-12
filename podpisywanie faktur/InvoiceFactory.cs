using static FileHelper;

namespace podpisywanie_faktur;

internal class InvoiceFactory
{
    internal static IEnumerable<Invoice> FromDirectory(string directory)
    {
        var invoicePaths = Directory.EnumerateFiles(directory, "*.xml", SearchOption.TopDirectoryOnly);

        foreach(var path in invoicePaths)
        {
            if (Path.GetFileNameWithoutExtension(path).EndsWith("_signed"))
                continue;

            yield return new Invoice
            {
                inputPath = path,
                outputPath = AutoOutputPath(path, false),
            };
        }
    }
}