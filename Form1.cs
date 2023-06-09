using Microsoft.Win32;
using System.Diagnostics;

namespace DirectoriesTree
{
    public partial class Form1 : Form
    {
        TreeView treeView = new TreeView();
        ImageList imageList = new ImageList();
        Icon dirIcon = new Icon("dir3.ico");
        string pathTXT, pathWORD, pathPDF;

        public Form1()
        {
            InitializeComponent();

            treeView.BackColor = Color.FromArgb(255, 48, 48, 48);
            treeView.ForeColor = Color.White;

            pathTXT = "notepad.exe";
            pathWORD = GetDefaultWORDProgramPath();
            pathPDF = GetDefaultPDFProgramPath();

            SettingsTreeView();
            GetAllDirectories(null, @"C:\1");
        }

        private string GetDefaultWORDProgramPath()
        {
            // Получает путь к исполняемому файлу Microsoft Word
            // ключ реестра, содержащий информацию о установленных версиях Word
            RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Winword.exe");
            if (registryKey != null)
            {
                object? pathValue = registryKey.GetValue(string.Empty);
                if (pathValue != null)
                {
                    return pathValue + "";
                }
                registryKey.Close();
            }
            return "";
        }

        private string GetDefaultPDFProgramPath()
        {
            // Получает путь к исполняемому файлу Microsoft PDF
            // ключ реестра, содержащий информацию о установленных версиях PDF
            RegistryKey? registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\PDFXEdit.exe");
            if (registryKey != null)
            {
                object? pathValue = registryKey.GetValue(string.Empty);
                if (pathValue != null)
                {
                    return pathValue + "";
                }
                registryKey.Close();
            }
            return "";
        }

        private void SettingsTreeView()
        {
            treeView.Location = new Point(0, 0);
            treeView.Size = new Size(ClientSize.Width, ClientSize.Height);
            treeView.NodeMouseDoubleClick += TreeViewOpenFile_NodeMouseDoubleClick;
            Controls.Add(treeView);

            imageList.Images.Add("dir", dirIcon.ToBitmap());
            imageList.ImageSize = new Size(18, 18);
            treeView.ImageList = imageList;
        }

        private void TreeViewOpenFile_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            string extension = Path.GetExtension(e.Node.Text);
            if (extension == ".txt")
            {
                try
                {
                    Process.Start(pathTXT, e.Node.Name);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось открыть файл .txt");
                }
            }
            else if (extension == ".docx")
            {
                try
                {
                    Process.Start(pathWORD, e.Node.Name);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось открыть документ .docx");
                }
            }
            else if (extension == ".pdf")
            {
                try
                {
                    Process.Start(pathPDF, e.Node.Name);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось открыть документ .pdf");
                }
            }
        }

        private void GetAllDirectories(TreeNode node, string directory)
        {
            var dirs = Directory.GetDirectories(directory);  // показывает все папки в директории
            var files = Directory.GetFiles(directory);  // показывает все файлы в директории
            Icon icon;
            string imageKey = "";  // ключ иконки

            // перебор директории
            for (int i = 0; i < dirs.Length; i++)
            {
                if (node == null)  // если это корни верхнего уровня
                {
                    // передаём ключ(полный путь), название(имя файла), ключ иконки(текст), ключ выбранной иконки(текст)
                    treeView.Nodes.Add(dirs[i], Path.GetFileName(dirs[i]), "dir", "dir");
                    GetAllDirectories(treeView.Nodes[i], dirs[i]);  // рекурсия, передаём в параметр элемент Node и путь к папке
                }
                else
                {
                    node.Nodes.Add(dirs[i], Path.GetFileName(dirs[i]), "dir", "dir");
                    GetAllDirectories(node.Nodes[i], dirs[i]);  // рекурсия, передаём в параметр элемент Node и путь к папке
                }
            }

            // перебор файлов
            for (int i = 0; i < files.Length; i++)
            {
                icon = GetFileOrFolderIcon(files[i], ref imageKey);  // получаем иконку файла
                if (!imageList.Images.ContainsKey(imageKey))  // если такой иконки ещё нет в списке ImageList
                {
                    imageList.Images.Add(imageKey, icon.ToBitmap());  // добавляем иконку
                }

                if (node == null)  // если это корни верхнего уровня
                    treeView.Nodes.Add(files[i], Path.GetFileName(files[i]), imageKey, imageKey);
                else
                    node.Nodes.Add(files[i], Path.GetFileName(files[i]), imageKey, imageKey);
            }
        }

        private Icon GetFileOrFolderIcon(string path, ref string key)
        {
            if (File.Exists(path))  // если это файл
            {
                FileSystemInfo fileInfo = new FileInfo(path);
                try
                {
                    // считываем иконку
                    Icon? icon = Icon.ExtractAssociatedIcon(fileInfo.FullName);
                    key = Path.GetExtension(path);  // ключу (для ImageList), присваиваем расширение файла
                    if (key == "") key = "file";  // если это пустой файл (без расширения)
                    return icon;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при получении иконки: {path} - {ex.Message}");
                }
            }
            else if (Directory.Exists(path))  // если это папка
            {
                key = "dir";  // ключу (для ImageList), присваиваем dir
                return dirIcon;
            }
            return null;
        }
    }
}