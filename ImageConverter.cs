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
            Encoding.RegisterProvider(
                CodePagesEncodingProvider.Instance);

            Directory.CreateDirectory(outputFolder);

            int totalImages = 0;

            string[] csvFiles =
                Directory.GetFiles(inputFolder, "*.csv");

            foreach (string csvFile in csvFiles)
            {
                string csvName =
                    Path.GetFileNameWithoutExtension(csvFile);

                string csvOutputFolder =
                    Path.Combine(outputFolder, csvName);

                Directory.CreateDirectory(csvOutputFolder);

                using var reader = new StreamReader(
                    csvFile,
                    Encoding.GetEncoding(949),
                    detectEncodingFromByteOrderMarks: false);

                using var csv = new CsvReader(
                    reader,
                    CultureInfo.InvariantCulture);

                var records =
                    csv.GetRecords<dynamic>().ToList();

                int imageIndex = 1;

                foreach (var record in records)
                {
                    var values =
                        (IDictionary<string, object>)record;

                    CreateImage(
                        values,
                        csvOutputFolder,
                        imageIndex);

                    imageIndex++;
                    totalImages++;
                }
            }

            return totalImages;
        }

        private static void CreateImage(
            IDictionary<string, object> values,
            string outputFolder,
            int imageIndex)
        {
            const int circleSize = 50;
            const int gap = 20;
            const int margin = 10;

            int canvasSize =
                margin * 2 +
                circleSize * 4 +
                gap * 3;

            using var image =
                new Image<Rgba32>(
                    canvasSize,
                    canvasSize);

            // 배경을 검은색으로 초기화
            for (int y = 0; y < canvasSize; y++)
            {
                for (int x = 0; x < canvasSize; x++)
                {
                    image[x, y] =
                        new Rgba32(0, 0, 0);
                }
            }

            // 16개 RGB 값을 4×4 원형으로 배치
            for (int pixel = 1; pixel <= 16; pixel++)
            {
                byte r =
                    ToByte(values[$"{pixel}_R"]);

                byte g =
                    ToByte(values[$"{pixel}_G"]);

                byte b =
                    ToByte(values[$"{pixel}_B"]);

                int column = (pixel - 1) % 4;
                int row = (pixel - 1) / 4;

                int left =
                    margin +
                    column * (circleSize + gap);

                int top =
                    margin +
                    row * (circleSize + gap);

                DrawCircle(
                    image,
                    left,
                    top,
                    circleSize,
                    new Rgba32(r, g, b));
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
                    outputFolder,
                    $"{fileName}.jpg");

            image.SaveAsJpeg(outputFile);
        }

        private static void DrawCircle(
            Image<Rgba32> image,
            int left,
            int top,
            int size,
            Rgba32 color)
        {
            double radius = size / 2.0;
            double centerX = left + radius;
            double centerY = top + radius;

            for (int y = top; y < top + size; y++)
            {
                for (int x = left; x < left + size; x++)
                {
                    double distanceX =
                        x + 0.5 - centerX;

                    double distanceY =
                        y + 0.5 - centerY;

                    double distance =
                        distanceX * distanceX +
                        distanceY * distanceY;

                    if (distance <= radius * radius)
                    {
                        image[x, y] = color;
                    }
                }
            }
        }

        private static byte ToByte(object value)
        {
            double number = double.Parse(
                value?.ToString() ?? "0",
                CultureInfo.InvariantCulture);

            number = Math.Clamp(number, 0, 255);

            // .5일 때 무조건 올림하는 일반적인 반올림
            return (byte)Math.Round(
                number,
                MidpointRounding.AwayFromZero);
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