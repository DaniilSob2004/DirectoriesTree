using Microsoft.Win32;
using System.Diagnostics;

namespace DirectoriesTree
{
    public partial class Form1 : Form
    {
        TreeView treeView = new TreeView();
        ImageList imageList = new ImageList();
        Icon dirIcon = new Icon("dir3.ico");

        public Form1()
        {
            InitializeComponent();
            SettingsTreeView();
            GetAllDirectories(null, @"C:\1");
        }

        private void SettingsTreeView()
        {
            treeView.Location = new Point(0, 0);
            treeView.Size = new Size(ClientSize.Width, ClientSize.Height);
            treeView.BackColor = Color.FromArgb(255, 48, 48, 48);
            treeView.ForeColor = Color.White;
            treeView.NodeMouseDoubleClick += TreeViewOpenFile_NodeMouseDoubleClick;
            treeView.BeforeExpand += TreeView_BeforeExpand;
            Controls.Add(treeView);

            imageList.Images.Add("dir", dirIcon.ToBitmap());
            imageList.ImageSize = new Size(18, 18);
            treeView.ImageList = imageList;
        }

        private void TreeView_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            // при открытии какого то элемента, вызывается событие
        }

        private void TreeViewOpenFile_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (File.Exists(e.Node.Name))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = e.Node.Name,
                        UseShellExecute = true
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось открыть файл!");
                }
            }
        }

        private void GetAllDirectories(TreeNode node, string directory)
        {
            if (!Directory.Exists(directory)) return;

            string[] dirs = {};
            string[] files = {};
            try
            {
                dirs = Directory.GetDirectories(directory);  // показывает все папки в директории
                files = Directory.GetFiles(directory);  // показывает все файлы в директории
            }
            catch (UnauthorizedAccessException) { return; }

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
            foreach (string f in files)
            {
                if (!imageList.Images.ContainsKey(Path.GetFileName(f)) || !imageList.Images.ContainsKey("file"))
                {
                    icon = GetFileOrFolderIcon(f, ref imageKey);  // получаем иконку файла
                    imageList.Images.Add(imageKey, icon.ToBitmap());  // добавляем иконку
                }

                if (node == null)  // если это корни верхнего уровня
                    treeView.Nodes.Add(f, Path.GetFileName(f), imageKey, imageKey);
                else
                    node.Nodes.Add(f, Path.GetFileName(f), imageKey, imageKey);
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