using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace krkrFgiEditor
{
    public partial class BatchWin : Form
    {
        public BatchWin(Action<ComboBox> setGroups,
            Action<ComboBox,int> setLayerBox,
            Action<object, MeasureItemEventArgs> measureItem,
            Action<ComboBox,ComboBox, DrawItemEventArgs> drawItem,
            Func<int,int,Layer> getLayer,
            Func<List<Layer>,string> getName)
        {
            InitializeComponent();
            groups = new List<List<Layer>>();
            SetLayerBox = setLayerBox;
            layerBox.MeasureItem += new MeasureItemEventHandler(measureItem);
            DrawItem = drawItem;
            GetLayer = getLayer;
            GetName = getName;
            listBoxTip.SetToolTip(groupList, "按backspace删除当前项");
            listBoxTip.SetToolTip(layerList, "按backspace删除当前项");
            setGroups(groupBox);
        }

        private int count = 1;
        private readonly List<List<Layer>> groups;
        private readonly Action<ComboBox, int> SetLayerBox;
        private readonly Action<ComboBox, ComboBox, DrawItemEventArgs> DrawItem;
        private readonly Func<int,int,Layer> GetLayer;
        private readonly Func<List<Layer>, string> GetName;

        private void BatchWin_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("合成顺序：按照组的顺序从上往下依次覆盖\n" +
                "合成规则：合成时遍历组内的所有图层", "帮助",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
                savePath.Text = fbd.SelectedPath;
            fbd.Dispose();
        }

        private void SavePath_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                CreateFolder(savePath.Text);
        }

        private bool CreateFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("请指定保存路径！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Directory.Exists(path))
            {
                if (MessageBox.Show("该文件夹不存在，是否新建文件夹？", "",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information) == DialogResult.OK)
                {
                    string[] folders = path.Split('\\');
                    string fpath = "";
                    foreach (string folder in folders)
                    {
                        fpath += folder + "/";
                        if (Directory.Exists(fpath))
                            continue;
                        try
                        {
                            Directory.CreateDirectory(fpath);
                        }
                        catch
                        {
                            MessageBox.Show("路径名称不合法！", "",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
                if (!Directory.Exists(path))
                {
                    MessageBox.Show("新建文件夹失败！", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            return true;
        }

        private void BatchWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            GC.Collect();
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            if (!CreateFolder(savePath.Text))
                return;
            GenerateImage(new List<Layer>(),0);
        }

        private void GenerateImage(List<Layer> layers,int index)
        {
            if (index < groups.Count)
                foreach (Layer layer in groups[index])
                    GenerateImage(new List<Layer>(layers) { layer }, index + 1);
            else
            {
                Image result = Layer.GenerateImage(layers);
                if(result != null)
                {
                    result.Save(Path.Combine(savePath.Text,
                    GetName(layers) + ".png"));
                    result.Dispose();
                }
            }
        }

        private bool IsCompleted()
        {
            if (groups.Count == 0)
                return false;
            foreach (List<Layer> group in groups)
                if (group.Count - (Layer.HasNone(group) ? 1 : 0) == 0)
                    return false;
            return true;
        }

        private void ListItemsChanged(object sender, EventArgs e)
        {
            if (!((ListBoxEx)sender).Enabled)
                return;
            confirmButton.Enabled = IsCompleted();
            CheckAddGroupButton();
            CheckAddButton();
            CheckSelected();
        }

        private void CheckAddGroupButton()
        {
            if (groupList.Items.Contains(groupBox.SelectedItem))
                addGroupButton.Enabled = false;
            else
                addGroupButton.Enabled = true;
        }

        private void CheckSelected()
        {
            if (groupList.SelectedIndex == -1)
            {
                upButton.Enabled = false;
                downButton.Enabled = false;
                deleteButton.Enabled = false;
            }
            else
            {
                deleteButton.Enabled = true;
                if (groupList.SelectedIndex > 0)
                    upButton.Enabled = true;
                else
                    upButton.Enabled = false;
                if (groupList.SelectedIndex < groupList.Items.Count - 1)
                    downButton.Enabled = true;
                else
                    downButton.Enabled = false;
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if(layerList.SelectedIndex == -1||
                layerList.Items.Contains("(none)"))
            {
                groups.Add(new List<Layer>());
                groupList.Items.Add($"Group {count++}");
            }
            else
            {
                groups[groupList.SelectedIndex].Add(new Layer()
                {
                    name = "(none)",
                    layerId = -1
                });
                layerList.Items.Add("(none)");
            }
        }

        private void LayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelected();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if(layerList.SelectedIndex == -1)
            {
                if(groupList.SelectedIndex != -1)
                {
                    groups.RemoveAt(groupList.SelectedIndex);
                    groupList.Items.RemoveAt(groupList.SelectedIndex);
                }
            }
            else
            {
                groups[groupList.SelectedIndex].RemoveAt(layerList.SelectedIndex);
                layerList.Items.RemoveAt(layerList.SelectedIndex);
            }
        }

        private void GroupList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(groupList.SelectedIndex!=-1&&e.KeyChar == '\b')
            {
                groups.RemoveAt(groupList.SelectedIndex);
                groupList.Items.RemoveAt(groupList.SelectedIndex);
            }
        }

        private void LayerList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (layerList.SelectedIndex != -1 && e.KeyChar == '\b')
            {
                groups[groupList.SelectedIndex].RemoveAt(layerList.SelectedIndex);
                layerList.Items.RemoveAt(layerList.SelectedIndex);
            }
        }

        private void GroupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            layerList.Items.Clear();
            if (groupList.SelectedIndex != -1)
                foreach (Layer layer in groups[groupList.SelectedIndex])
                    layerList.Items.Add(layer.name);
            CheckAddButton();
            CheckSelected();
        }

        private void MoveSelected(int index, bool isUp)
        {
            List<Layer> group = groups[index];
            string item = groupList.Items[index].ToString();
            groups.RemoveAt(index);
            groupList.Enabled = false;
            groupList.Items.RemoveAt(index);
            if (isUp)
                index--;
            else
                index++;
            groupList.Enabled = true;
            groups.Insert(index, group);
            groupList.Items.Insert(index, item);
            groupList.SelectedIndex = index;
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            MoveSelected(groupList.SelectedIndex, true);
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            MoveSelected(groupList.SelectedIndex, false);
        }

        private void GroupBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLayerBox(layerBox,groupBox.SelectedIndex);
            CheckAddGroupButton();
            addButton.Enabled = false;
        }

        private void LayerBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawItem(layerBox, groupBox, e);
        }

        private void LayerBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAddButton();
        }

        private void CheckAddButton()
        {
            if (layerBox.SelectedIndex != -1 &&
                groupList.SelectedIndex != -1 &&
                !layerList.Items.Contains(layerBox.SelectedItem))
                addButton.Enabled = true;
            else
                addButton.Enabled = false;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            groups[groupList.SelectedIndex].Add(GetLayer(groupBox.SelectedIndex,
                layerBox.SelectedIndex));
            layerList.Items.Add(layerBox.SelectedItem);
        }

        private void AddGroupButton_Click(object sender, EventArgs e)
        {
            groups.Add(new List<Layer>());
            for (int i = 0; i < layerBox.Items.Count; i++)
                groups[groups.Count - 1].Add(GetLayer(groupBox.SelectedIndex, i));
            groupList.Items.Add(groupBox.SelectedItem);
        }
    }
}
