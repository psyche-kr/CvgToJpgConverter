namespace CsvToJpgConverter
{
    public partial class Form1 : Form
    {
        private Button selectInputButton;
        private Button selectOutputButton;
        private Button convertButton;
        private Label statusLabel;

        private string inputFolder = "";
        private string outputFolder = "";

        public Form1()
        {
            InitializeComponent();

            selectInputButton = new Button();
            selectInputButton.Text = "CSV 폴더 선택";
            selectInputButton.Width = 180;
            selectInputButton.Left = 30;
            selectInputButton.Top = 30;
            selectInputButton.Click += SelectInputButton_Click;

            selectOutputButton = new Button();
            selectOutputButton.Text = "저장 폴더 선택";
            selectOutputButton.Width = 180;
            selectOutputButton.Left = 30;
            selectOutputButton.Top = 80;
            selectOutputButton.Click += SelectOutputButton_Click;

            convertButton = new Button();
            convertButton.Text = "변환 시작";
            convertButton.Width = 180;
            convertButton.Left = 30;
            convertButton.Top = 130;
            convertButton.Click += ConvertButton_Click;

            statusLabel = new Label();
            statusLabel.Text = "폴더를 선택하세요.";
            statusLabel.AutoSize = true;
            statusLabel.Left = 240;
            statusLabel.Top = 35;

            Controls.Add(selectInputButton);
            Controls.Add(selectOutputButton);
            Controls.Add(convertButton);
            Controls.Add(statusLabel);
        }

        private void SelectInputButton_Click(
            object? sender,
            EventArgs e)
        {
            using var dialog =
                new FolderBrowserDialog();

            dialog.Description =
                "CSV 파일이 있는 폴더를 선택하세요.";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                inputFolder = dialog.SelectedPath;
                statusLabel.Text =
                    $"입력 폴더: {inputFolder}";
            }
        }

        private void SelectOutputButton_Click(
            object? sender,
            EventArgs e)
        {
            using var dialog =
                new FolderBrowserDialog();

            dialog.Description =
                "JPG를 저장할 폴더를 선택하세요.";

            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                outputFolder = dialog.SelectedPath;
                statusLabel.Text =
                    $"저장 폴더: {outputFolder}";
            }
        }

        private void ConvertButton_Click(
            object? sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputFolder) ||
                string.IsNullOrWhiteSpace(outputFolder))
            {
                MessageBox.Show(
                    "입력 폴더와 저장 폴더를 모두 선택하세요.");

                return;
            }

            try
            {
                var converter =
                    new ImageConverter();

                int count =
                    converter.ConvertFolder(
                        inputFolder,
                        outputFolder);

                statusLabel.Text =
                    $"{count}개 이미지 생성 완료";

                MessageBox.Show(
                    $"{count}개의 JPG 이미지가 생성되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"오류가 발생했습니다.\n{ex.Message}");
            }
        }
    }
}