Console.WriteLine("Podaj nazwę pliku do podpisania");
var userInput = Console.ReadLine();
if (string.IsNullOrWhiteSpace(userInput))
    throw new ArgumentException("Nazwa pliku nie może być pusta");

var currentDirectory = Environment.CurrentDirectory;
//creating input file path
var inputPath = Path.Combine(currentDirectory, userInput);

//creating output  file path
var outputFileName = $"{Path.GetFileNameWithoutExtension(userInput)}_signed.xml";
var outputPath = Path.Combine(currentDirectory, outputFileName);

//wybor certyfikatu
var certificate = CertificatesHelper.CurrentUser();
if (certificate == null)
    return;

await XmlSigner.SignXmlAsync(inputPath, outputPath, certificate);
if (!XmlSigner.VerifySignedXml(outputPath))
    return;

Console.WriteLine($"OK {outputPath}");
return;