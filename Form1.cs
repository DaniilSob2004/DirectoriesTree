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
            // ��� �������� ������ �� ��������, ���������� �������
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
                    MessageBox.Show("�� ������� ������� ����!");
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
            foreach (string f in files)
            {
                if (!imageList.Images.ContainsKey(Path.GetFileName(f)) || !imageList.Images.ContainsKey("file"))
                {
                    icon = GetFileOrFolderIcon(f, ref imageKey);  // �������� ������ �����
                    imageList.Images.Add(imageKey, icon.ToBitmap());  // ��������� ������
                }

                if (node == null)  // ���� ��� ����� �������� ������
                    treeView.Nodes.Add(f, Path.GetFileName(f), imageKey, imageKey);
                else
                    node.Nodes.Add(f, Path.GetFileName(f), imageKey, imageKey);
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