using CsvHelper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Globalization;
using System.Text;

namespace CsvToJpgConverter
{
    public class ImageConverter
    {
        public int ConvertFolder(
            string inputFolder,
            string outputFolder)
        {
            Directory.CreateDirectory(outputFolder);

            int totalImages = 0;

            string[] csvFiles = Directory.GetFiles(
                inputFolder,
                "*.csv");

            foreach (string csvFile in csvFiles)
            {
                string csvName =
                    Path.GetFileNameWithoutExtension(csvFile);

                string csvOutputFolder =
                    Path.Combine(outputFolder, csvName);

                Directory.CreateDirectory(csvOutputFolder);

                Encoding.RegisterProvider(
                    CodePagesEncodingProvider.Instance);

                using var reader = new StreamReader(
                    csvFile,
                    Encoding.GetEncoding(949),
                    detectEncodingFromByteOrderMarks: false);

                using var csv = new CsvReader(
                    reader,
                    CultureInfo.InvariantCulture);

                var records = csv.GetRecords<dynamic>().ToList();

                int imageIndex = 1;

                foreach (var record in records)
                {
                    var values =
                        (IDictionary<string, object>)record;

                    using var image =
                        new Image<Rgba32>(4, 4);

                    for (int pixel = 1; pixel <= 16; pixel++)
                    {
                        byte r = ToByte(values[$"{pixel}_R"]);
                        byte g = ToByte(values[$"{pixel}_G"]);
                        byte b = ToByte(values[$"{pixel}_B"]);

                        int x = (pixel - 1) % 4;
                        int y = (pixel - 1) / 4;

                        image[x, y] =
                            new Rgba32(r, g, b);
                    }

                    string name =
                        GetValue(values, "Name");

                    string time =
                        GetValue(values, "Time");

                    string fileName =
                        $"{imageIndex:000}_{time}_{name}";

                    fileName =
                        MakeSafeFileName(fileName);

                    string outputFile =
                        Path.Combine(
                            csvOutputFolder,
                            $"{fileName}.jpg");

                    image.SaveAsJpeg(outputFile);

                    imageIndex++;
                    totalImages++;
                }
            }

            return totalImages;
        }

        private static byte ToByte(object value)
        {
            double number = double.Parse(
                value?.ToString() ?? "0",
                CultureInfo.InvariantCulture);

            number = Math.Clamp(number, 0, 255);

            return (byte)Math.Round(number);
        }

        private static string GetValue(
            IDictionary<string, object> values,
            string columnName)
        {
            if (values.ContainsKey(columnName))
            {
                return values[columnName]?.ToString() ?? "";
            }

            return "";
        }

        private static string MakeSafeFileName(
            string fileName)
        {
            foreach (char invalidChar in
                Path.GetInvalidFileNameChars())
            {
                fileName =
                    fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }
    }
}