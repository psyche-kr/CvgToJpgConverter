namespace CsvToJpgConverter
{
    public partial class Form1 : Form
    {
        private Button selectInputButton;
        private Button selectOutputButton;
        private Button convertButton;

        private Label inputLabel;
        private Label outputLabel;

        private TextBox inputPathTextBox;
        private TextBox outputPathTextBox;

        private string inputFolder = "";
        private string outputFolder = "";

        public Form1()
        {
            InitializeComponent();

            Text = "CSV to PNG";

            // 폼 크기 고정
            ClientSize = new Size(600, 430);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;

            // CSV 폴더 선택 버튼
            selectInputButton = new Button
            {
                Text = "CSV 폴더 선택",
                Location = new Point(32, 30),
                Size = new Size(180, 42)
            };
            selectInputButton.Click += SelectInputButton_Click;

            // 입력 폴더 라벨
            inputLabel = new Label
            {
                Text = "입력 폴더:",
                Location = new Point(36, 88),
                AutoSize = true
            };

            // 입력 폴더 경로 표시 영역
            inputPathTextBox = new TextBox
            {
                Location = new Point(112, 82),
                Size = new Size(450, 70),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true
            };

            // 저장 폴더 선택 버튼
            selectOutputButton = new Button
            {
                Text = "저장 폴더 선택",
                Location = new Point(32, 180),
                Size = new Size(180, 42)
            };
            selectOutputButton.Click += SelectOutputButton_Click;

            // 출력 폴더 라벨
            outputLabel = new Label
            {
                Text = "출력 폴더:",
                Location = new Point(36, 238),
                AutoSize = true
            };

            // 출력 폴더 경로 표시 영역
            outputPathTextBox = new TextBox
            {
                Location = new Point(112, 232),
                Size = new Size(450, 70),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true
            };

            // 변환 시작 버튼
            convertButton = new Button
            {
                Text = "변환",
                Location = new Point(32, 340),
                Size = new Size(180, 42)
            };
            convertButton.Click += ConvertButton_Click;

            Controls.Add(selectInputButton);
            Controls.Add(inputLabel);
            Controls.Add(inputPathTextBox);

            Controls.Add(selectOutputButton);
            Controls.Add(outputLabel);
            Controls.Add(outputPathTextBox);

            Controls.Add(convertButton);
        }

        private void SelectInputButton_Click(
            object? sender,
            EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "CSV 파일이 있는 폴더를 선택하세요."
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                inputFolder = dialog.SelectedPath;
                inputPathTextBox.Text = inputFolder;
            }
        }

        private void SelectOutputButton_Click(
            object? sender,
            EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "PNG를 저장할 폴더를 선택하세요."
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputFolder = dialog.SelectedPath;
                outputPathTextBox.Text = outputFolder;
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
                    "입력 폴더와 출력 폴더를 모두 선택하세요.",
                    "알림",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                var converter = new ImageConverter();

                int count = converter.ConvertFolder(
                    inputFolder,
                    outputFolder);

                MessageBox.Show(
                    $"{count}개의 PNG 이미지가 생성되었습니다.",
                    "변환 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"오류가 발생했습니다.\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}