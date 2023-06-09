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

            string dir = @"C:\1";
            GetAllDirectories(null, dir);
        }

        private string GetDefaultWORDProgramPath()
        {
            // �������� ���� � ������������ ����� Microsoft Word
            // ���� �������, ���������� ���������� � ������������� ������� Word
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
            // �������� ���� � ������������ ����� Microsoft PDF
            // ���� �������, ���������� ���������� � ������������� ������� PDF
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
                    MessageBox.Show("�� ������� ������� ���� .txt");
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
                    MessageBox.Show("�� ������� ������� �������� .docx");
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
                    MessageBox.Show("�� ������� ������� �������� .pdf");
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
                dirs = Directory.GetDirectories(directory);  // ���������� ��� ����� � ����������
                files = Directory.GetFiles(directory);  // ���������� ��� ����� � ����������
            }
            catch (UnauthorizedAccessException) { return; }

            Icon icon;
            string imageKey = "";  // ���� ������

            // ������� ����������
            for (int i = 0; i < dirs.Length; i++)
            {
                if (node == null)  // ���� ��� ����� �������� ������
                {
                    // ������� ����(������ ����), ��������(��� �����), ���� ������(�����), ���� ��������� ������(�����)
                    treeView.Nodes.Add(dirs[i], Path.GetFileName(dirs[i]), "dir", "dir");
                    GetAllDirectories(treeView.Nodes[i], dirs[i]);  // ��������, ������� � �������� ������� Node � ���� � �����
                }
                else
                {
                    node.Nodes.Add(dirs[i], Path.GetFileName(dirs[i]), "dir", "dir");
                    GetAllDirectories(node.Nodes[i], dirs[i]);  // ��������, ������� � �������� ������� Node � ���� � �����
                }
            }

            // ������� ������
            for (int i = 0; i < files.Length; i++)
            {
                if (!imageList.Images.ContainsKey(Path.GetFileName(files[i])) || !imageList.Images.ContainsKey("file"))
                {
                    icon = GetFileOrFolderIcon(files[i], ref imageKey);  // �������� ������ �����
                    imageList.Images.Add(imageKey, icon.ToBitmap());  // ��������� ������
                }

                if (node == null)  // ���� ��� ����� �������� ������
                    treeView.Nodes.Add(files[i], Path.GetFileName(files[i]), imageKey, imageKey);
                else
                    node.Nodes.Add(files[i], Path.GetFileName(files[i]), imageKey, imageKey);
            }
        }

        private Icon GetFileOrFolderIcon(string path, ref string key)
        {
            if (File.Exists(path))  // ���� ��� ����
            {
                FileSystemInfo fileInfo = new FileInfo(path);
                try
                {
                    // ��������� ������
                    Icon? icon = Icon.ExtractAssociatedIcon(fileInfo.FullName);
                    key = Path.GetExtension(path);  // ����� (��� ImageList), ����������� ���������� �����

                    if (key == "") key = "file";  // ���� ��� ������ ���� (��� ����������)
                    return icon;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"��������� ������ ��� ��������� ������: {path} - {ex.Message}");
                }
            }
            else if (Directory.Exists(path))  // ���� ��� �����
            {
                key = "dir";  // ����� (��� ImageList), ����������� dir
                return dirIcon;
            }

            return null;
        }
    }
}