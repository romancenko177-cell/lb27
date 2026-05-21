using System;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace Lab27_FileSystemExplorer;

public class Form1 : Form
{
    private ComboBox cmbDrives = new ComboBox();
    private ListBox lstFolders = new ListBox();
    private ListBox lstFiles = new ListBox();
    private TextBox txtPath = new TextBox();
    private TextBox txtFileFilter = new TextBox();
    private TextBox txtFolderFilter = new TextBox();
    private TextBox txtInfo = new TextBox();
    private TextBox txtTextPreview = new TextBox();
    private TextBox txtSecurity = new TextBox();
    private PictureBox picturePreview = new PictureBox();
    private Button btnBack = new Button();
    private Button btnOpen = new Button();

    private string currentPath = "";

    public Form1()
    {
        InitializeComponent();
        LoadDrives();
    }

    private void InitializeComponent()
    {
        Text = "ЛР27. Робота з файловою системою";
        Width = 1150;
        Height = 720;
        StartPosition = FormStartPosition.CenterScreen;

        Label lblDrive = new Label { Text = "Диск:", Left = 10, Top = 15, Width = 50 };
        cmbDrives.Left = 65; cmbDrives.Top = 10; cmbDrives.Width = 120;
        cmbDrives.SelectedIndexChanged += CmbDrives_SelectedIndexChanged;

        btnBack.Text = "Назад";
        btnBack.Left = 195; btnBack.Top = 9; btnBack.Width = 80;
        btnBack.Click += BtnBack_Click;

        Label lblPath = new Label { Text = "Поточний шлях:", Left = 290, Top = 15, Width = 100 };
        txtPath.Left = 395; txtPath.Top = 10; txtPath.Width = 560;
        txtPath.ReadOnly = true;

        btnOpen.Text = "Оновити";
        btnOpen.Left = 970; btnOpen.Top = 9; btnOpen.Width = 90;
        btnOpen.Click += (s, e) => LoadDirectory(currentPath);

        Label lblFolderFilter = new Label { Text = "Фільтр каталогів:", Left = 10, Top = 50, Width = 120 };
        txtFolderFilter.Left = 130; txtFolderFilter.Top = 47; txtFolderFilter.Width = 180;
        txtFolderFilter.Text = "*";
        txtFolderFilter.TextChanged += (s, e) => LoadDirectory(currentPath);

        Label lblFileFilter = new Label { Text = "Фільтр файлів:", Left = 330, Top = 50, Width = 100 };
        txtFileFilter.Left = 430; txtFileFilter.Top = 47; txtFileFilter.Width = 180;
        txtFileFilter.Text = "*.*";
        txtFileFilter.TextChanged += (s, e) => LoadDirectory(currentPath);

        Label lblFolders = new Label { Text = "Каталоги", Left = 10, Top = 85, Width = 150 };
        lstFolders.Left = 10; lstFolders.Top = 110; lstFolders.Width = 330; lstFolders.Height = 250;
        lstFolders.DoubleClick += LstFolders_DoubleClick;
        lstFolders.SelectedIndexChanged += LstFolders_SelectedIndexChanged;

        Label lblFiles = new Label { Text = "Файли", Left = 355, Top = 85, Width = 150 };
        lstFiles.Left = 355; lstFiles.Top = 110; lstFiles.Width = 330; lstFiles.Height = 250;
        lstFiles.SelectedIndexChanged += LstFiles_SelectedIndexChanged;

        Label lblInfo = new Label { Text = "Властивості", Left = 700, Top = 85, Width = 150 };
        txtInfo.Left = 700; txtInfo.Top = 110; txtInfo.Width = 410; txtInfo.Height = 250;
        txtInfo.Multiline = true; txtInfo.ScrollBars = ScrollBars.Vertical; txtInfo.ReadOnly = true;

        Label lblText = new Label { Text = "Перегляд текстових файлів", Left = 10, Top = 375, Width = 200 };
        txtTextPreview.Left = 10; txtTextPreview.Top = 400; txtTextPreview.Width = 520; txtTextPreview.Height = 250;
        txtTextPreview.Multiline = true; txtTextPreview.ScrollBars = ScrollBars.Both; txtTextPreview.ReadOnly = true;

        Label lblImage = new Label { Text = "Перегляд графічних файлів", Left = 545, Top = 375, Width = 200 };
        picturePreview.Left = 545; picturePreview.Top = 400; picturePreview.Width = 270; picturePreview.Height = 250;
        picturePreview.BorderStyle = BorderStyle.FixedSingle;
        picturePreview.SizeMode = PictureBoxSizeMode.Zoom;

        Label lblSecurity = new Label { Text = "Атрибути безпеки", Left = 830, Top = 375, Width = 200 };
        txtSecurity.Left = 830; txtSecurity.Top = 400; txtSecurity.Width = 280; txtSecurity.Height = 250;
        txtSecurity.Multiline = true; txtSecurity.ScrollBars = ScrollBars.Both; txtSecurity.ReadOnly = true;

        Controls.AddRange(new Control[] {
            lblDrive, cmbDrives, btnBack, lblPath, txtPath, btnOpen,
            lblFolderFilter, txtFolderFilter, lblFileFilter, txtFileFilter,
            lblFolders, lstFolders, lblFiles, lstFiles, lblInfo, txtInfo,
            lblText, txtTextPreview, lblImage, picturePreview, lblSecurity, txtSecurity
        });
    }

    private void LoadDrives()
    {
        cmbDrives.Items.Clear();
        foreach (DriveInfo drive in DriveInfo.GetDrives())
            cmbDrives.Items.Add(drive.Name);

        if (cmbDrives.Items.Count > 0)
            cmbDrives.SelectedIndex = 0;
    }

    private void CmbDrives_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbDrives.SelectedItem == null) return;
        currentPath = cmbDrives.SelectedItem.ToString()!;
        ShowDriveInfo(currentPath);
        LoadDirectory(currentPath);
    }

    private void LoadDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return;

        try
        {
            currentPath = path;
            txtPath.Text = currentPath;
            lstFolders.Items.Clear();
            lstFiles.Items.Clear();
            txtTextPreview.Clear();
            txtSecurity.Clear();
            picturePreview.Image = null;

            string folderFilter = string.IsNullOrWhiteSpace(txtFolderFilter.Text) ? "*" : txtFolderFilter.Text;
            string fileFilter = string.IsNullOrWhiteSpace(txtFileFilter.Text) ? "*.*" : txtFileFilter.Text;

            foreach (string dir in Directory.GetDirectories(path, folderFilter))
                lstFolders.Items.Add(dir);

            foreach (string file in Directory.GetFiles(path, fileFilter))
                lstFiles.Items.Add(file);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка відкриття каталогу: " + ex.Message);
        }
    }

    private void BtnBack_Click(object? sender, EventArgs e)
    {
        try
        {
            DirectoryInfo? parent = Directory.GetParent(currentPath);
            if (parent != null)
                LoadDirectory(parent.FullName);
        }
        catch { }
    }

    private void LstFolders_DoubleClick(object? sender, EventArgs e)
    {
        if (lstFolders.SelectedItem != null)
            LoadDirectory(lstFolders.SelectedItem.ToString()!);
    }

    private void LstFolders_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstFolders.SelectedItem == null) return;
        string path = lstFolders.SelectedItem.ToString()!;
        ShowDirectoryInfo(path);
        ShowSecurityInfo(path);
    }

    private void LstFiles_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstFiles.SelectedItem == null) return;
        string path = lstFiles.SelectedItem.ToString()!;
        ShowFileInfo(path);
        ShowTextPreview(path);
        ShowImagePreview(path);
        ShowSecurityInfo(path);
    }

    private void ShowDriveInfo(string path)
    {
        try
        {
            DriveInfo drive = new DriveInfo(path);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ВЛАСТИВОСТІ ДИСКА");
            sb.AppendLine("Назва: " + drive.Name);
            sb.AppendLine("Тип: " + drive.DriveType);
            sb.AppendLine("Готовий: " + drive.IsReady);

            if (drive.IsReady)
            {
                sb.AppendLine("Файлова система: " + drive.DriveFormat);
                sb.AppendLine("Мітка тому: " + drive.VolumeLabel);
                sb.AppendLine("Загальний розмір: " + FormatBytes(drive.TotalSize));
                sb.AppendLine("Вільне місце: " + FormatBytes(drive.TotalFreeSpace));
                sb.AppendLine("Кореневий каталог: " + drive.RootDirectory.FullName);
            }

            txtInfo.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            txtInfo.Text = ex.Message;
        }
    }

    private void ShowDirectoryInfo(string path)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            txtInfo.Text =
                "ВЛАСТИВОСТІ КАТАЛОГУ" + Environment.NewLine +
                "Назва: " + dir.Name + Environment.NewLine +
                "Повний шлях: " + dir.FullName + Environment.NewLine +
                "Батьківський каталог: " + dir.Parent + Environment.NewLine +
                "Кореневий каталог: " + dir.Root + Environment.NewLine +
                "Дата створення: " + dir.CreationTime + Environment.NewLine +
                "Останній доступ: " + dir.LastAccessTime + Environment.NewLine +
                "Остання зміна: " + dir.LastWriteTime + Environment.NewLine +
                "Атрибути: " + dir.Attributes;
        }
        catch (Exception ex)
        {
            txtInfo.Text = ex.Message;
        }
    }

    private void ShowFileInfo(string path)
    {
        try
        {
            FileInfo file = new FileInfo(path);
            txtInfo.Text =
                "ВЛАСТИВОСТІ ФАЙЛУ" + Environment.NewLine +
                "Назва: " + file.Name + Environment.NewLine +
                "Повний шлях: " + file.FullName + Environment.NewLine +
                "Розширення: " + file.Extension + Environment.NewLine +
                "Розмір: " + FormatBytes(file.Length) + Environment.NewLine +
                "Каталог: " + file.DirectoryName + Environment.NewLine +
                "Дата створення: " + file.CreationTime + Environment.NewLine +
                "Останній доступ: " + file.LastAccessTime + Environment.NewLine +
                "Остання зміна: " + file.LastWriteTime + Environment.NewLine +
                "Тільки читання: " + file.IsReadOnly + Environment.NewLine +
                "Атрибути: " + file.Attributes;
        }
        catch (Exception ex)
        {
            txtInfo.Text = ex.Message;
        }
    }

    private void ShowTextPreview(string path)
    {
        txtTextPreview.Clear();
        try
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".txt" || ext == ".cs" || ext == ".html" || ext == ".css" || ext == ".json" || ext == ".xml" || ext == ".md")
                txtTextPreview.Text = File.ReadAllText(path, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            txtTextPreview.Text = "Помилка читання тексту: " + ex.Message;
        }
    }

    private void ShowImagePreview(string path)
    {
        picturePreview.Image = null;
        try
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
            {
                using FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                picturePreview.Image = Image.FromStream(fs);
            }
        }
        catch { }
    }

    private void ShowSecurityInfo(string path)
    {
        txtSecurity.Clear();

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("АТРИБУТИ БЕЗПЕКИ");

            AuthorizationRuleCollection rules;

            if (File.Exists(path))
            {
                FileSecurity security = new FileSecurity(path, AccessControlSections.Access);
                rules = security.GetAccessRules(true, true, typeof(NTAccount));
            }
            else if (Directory.Exists(path))
            {
                DirectorySecurity security = new DirectorySecurity(path, AccessControlSections.Access);
                rules = security.GetAccessRules(true, true, typeof(NTAccount));
            }
            else
            {
                txtSecurity.Text = "Об'єкт не знайдено.";
                return;
            }

            foreach (FileSystemAccessRule rule in rules)
            {
                sb.AppendLine(rule.IdentityReference.Value);
                sb.AppendLine("Тип: " + rule.AccessControlType);
                sb.AppendLine("Права: " + rule.FileSystemRights);
                sb.AppendLine();
            }

            txtSecurity.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            txtSecurity.Text = "Немає доступу або помилка: " + ex.Message;
        }
    }

    private string FormatBytes(long bytes)
    {
        double size = bytes;
        string[] units = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return Math.Round(size, 2) + " " + units[unit];
    }
}
