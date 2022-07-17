using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace krkrFgiEditor
{
    public partial class MainWin : Form
    {
        public MainWin()
        {
            InitializeComponent();
            encodings = new Encoding[]
            {
                Encoding.GetEncoding(932),
                Encoding.UTF8,
                Encoding.Unicode,
            };
            selectedBoxTip.SetToolTip(selectedBox, "按backspace删除当前项");
            fgiBoxTip.SetToolTip(fgiBox, "点击查看大图");
            groupLayers = new List<GroupLayer>();
            selectedLayers = new List<Layer>();
            layerAlphas = new Dictionary<int, int>();
        }

        private Image resultImage;

        private readonly Encoding[] encodings;
        private readonly List<GroupLayer> groupLayers;
        private readonly List<Layer> selectedLayers;
        private Image ResultImage
        {
            get
            {
                return resultImage;
            }
            set
            {
                if (resultImage != null)
                    resultImage.Dispose();
                resultImage = value;
            }
        }
        private string character;
        private readonly Dictionary<int, int> layerAlphas;

        private void OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "文本文件|*.txt"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                encodingBox.SelectedIndex = -1;
                encodingBox.SelectedIndex = DetectEncoding(filePath.Text = ofd.FileName);
            }
            ofd.Dispose();
        }

        private void FilePath_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                if (!File.Exists(filePath.Text))
                    MessageBox.Show("文件不存在！", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    encodingBox.SelectedIndex = -1;
                    encodingBox.SelectedIndex = DetectEncoding(filePath.Text);
                }
        }

        private void ClearAll()
        {
            encodingLabel.Enabled = false;
            encodingBox.Enabled = false;
            detectEncoding.Enabled = false;
            layerPanel.Enabled = false;
            saveButton.Enabled = false;
            batchButton.Enabled = false;
            fgiBox.Image = null;
            ResultImage = null;
            groupBox.Items.Clear();
            layerBox.Items.Clear();
            layerAlphas.Clear();
            selectedLayers.Clear();
            selectedBox.Items.Clear();
            groupLayers.Clear();
            groupLayers.Add(new GroupLayer
            {
                name = "(none)",
                groupLayerId = -1,
                layers = new List<Layer>()
            }); ;
            GC.Collect();
        }

        private void Initialize(string path, Encoding encoding)
        {
            ClearAll();
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
            character = Path.GetFileNameWithoutExtension(path);
            using (StreamReader sr = new StreamReader(path, encoding))
            {
                foreach (string line in Regex.Split(sr.ReadToEnd(), "\r\n"))
                {
                    string[] attrs = line.Split('\t');
                    if (attrs[0] == "0")
                    {
                        FileInfo[] infos = dir.GetFiles($"{character}_{attrs[9]}.*");
                        try
                        {
                            if (!string.IsNullOrEmpty(attrs[10]))
                            {
                                int groupLayerId = int.Parse(attrs[10]);
                                if (!HasGroupLayerId(groupLayerId, out int index))
                                    groupLayers.Add(new GroupLayer
                                    {
                                        groupLayerId = groupLayerId,
                                        layers = new List<Layer>()
                                    });
                                groupLayers[index].layers.Add(new Layer
                                {
                                    name = attrs[1],
                                    left = int.Parse(attrs[2]),
                                    top = int.Parse(attrs[3]),
                                    opacity = byte.Parse(attrs[7]),
                                    layerId = int.Parse(attrs[9]),
                                    Image = Image.FromFile(infos[0].FullName)
                                });
                            }
                            else
                                groupLayers[0].layers.Add(new Layer
                                {
                                    name = attrs[1],
                                    left = int.Parse(attrs[2]),
                                    top = int.Parse(attrs[3]),
                                    opacity = byte.Parse(attrs[7]),
                                    layerId = int.Parse(attrs[9]),
                                    Image = Image.FromFile(infos[0].FullName)
                                });
                        }
                        catch (IndexOutOfRangeException)
                        {
                            MessageBox.Show($"图片读取失败：{character}_{attrs[9]}", "",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }
                    }
                    else if (attrs[0] == "2")
                        if (HasGroupLayerId(int.Parse(attrs[9]), out int index))
                            groupLayers[index].name = attrs[1];
                }
            }
            foreach (GroupLayer groupLayer in groupLayers)
                groupBox.Items.Add(groupLayer.name);
            groupBox.SelectedIndex = 0;
            encodingLabel.Enabled = true;
            encodingBox.Enabled = true;
            detectEncoding.Enabled = true;
            layerPanel.Enabled = true;
            batchButton.Enabled = true;
        }

        private void Layer_HoveredIndexChanged(object sender, EventArgs e)
        {
            if (layerBox.HoveredIndex == -1 ||
                selectedLayers.Contains(groupLayers[groupBox.SelectedIndex].layers[layerBox.HoveredIndex]))
                ShowFgi(ResultImage);
            else
            {
                List<Layer> layers = new List<Layer>(selectedLayers)
                {
                    groupLayers[groupBox.SelectedIndex].layers[layerBox.HoveredIndex]
                };
                if (autoSort.Checked)
                    SortLayers(layers);
                Image tempImage = GenerateImage(layers);
                ShowFgi(tempImage);
                tempImage.Dispose();
            }
        }

        private bool HasGroupLayerId(int groupLayerId, out int index)
        {
            for (index = 0; index < groupLayers.Count; index++)
                if (groupLayers[index].groupLayerId == groupLayerId)
                    return true;
            return false;
        }

        private void SetLayerBox(int index)
        {
            layerBox.Items.Clear();
            foreach (Layer layer in groupLayers[index].layers)
                layerBox.Items.Add(layer.name);
            layerBox.DropDownHeight = Math.Min(layerBox.Items.Count, 10) * 100 + 2;
        }

        private void LayerBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index > -1)
            {
                if ((e.State & DrawItemState.ComboBoxEdit) != 0)
                    e.Graphics.DrawString(layerBox.Items[e.Index].ToString(),
                        e.Font, new SolidBrush(e.ForeColor), e.Bounds,
                        StringFormat.GenericDefault);
                else
                {
                    int height = layerBox.GetItemHeight(e.Index);
                    int barWidth = layerBox.Items.Count > 10 ? SystemInformation.VerticalScrollBarWidth : 0;
                    Image icon = Program.ResizeImage(groupLayers[groupBox.SelectedIndex].layers[e.Index].Image,
                        height, height);
                    e.Graphics.DrawImage(icon, e.Bounds.Left + (height - icon.Width) / 2,
                        e.Bounds.Top + (height - icon.Height) / 2);
                    e.Graphics.DrawString(layerBox.Items[e.Index].ToString(),
                        e.Font, new SolidBrush(e.ForeColor),
                        new Rectangle(e.Bounds.Left + height, e.Bounds.Top,
                        layerBox.ClientSize.Width - height - barWidth,
                        height),
                        new StringFormat()
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    icon.Dispose();
                }
            }
        }

        private void LayerBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index > -1)
                e.ItemHeight = 100;
        }

        private void LayerBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAdd();
            if (layerBox.SelectedIndex == -1)
                cancelButton.Enabled = false;
            else
                cancelButton.Enabled = true;
        }

        private void GroupBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLayerBox(groupBox.SelectedIndex);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            layerBox.SelectedIndex = -1;
            ShowFgi(ResultImage);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            selectedLayers.Add(groupLayers[groupBox.SelectedIndex].layers[layerBox.SelectedIndex]);
            if (autoSort.Checked)
                SortSelectedLayers();
            else
                selectedBox.Items.Add(layerBox.Items[layerBox.SelectedIndex]);
        }

        private void SelectedBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (selectedBox.SelectedIndex != -1 && e.KeyChar == '\b')
            {
                selectedLayers.RemoveAt(selectedBox.SelectedIndex);
                selectedBox.Items.RemoveAt(selectedBox.SelectedIndex);
            }
        }

        private void CheckAdd()
        {
            if (layerBox.SelectedIndex != -1 &&
                !selectedBox.Items.Contains(layerBox.Items[layerBox.SelectedIndex]))
                addButton.Enabled = true;
            else
                addButton.Enabled = false;
        }

        private void SelectedBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelected();
        }

        private void CheckSelected()
        {
            if (selectedBox.SelectedIndex == -1)
            {
                upButton.Enabled = false;
                downButton.Enabled = false;
                deleteButton.Enabled = false;
            }
            else
            {
                deleteButton.Enabled = true;
                if (selectedBox.SelectedIndex > 0)
                    upButton.Enabled = true;
                else
                    upButton.Enabled = false;
                if (selectedBox.SelectedIndex < selectedBox.Items.Count - 1)
                    downButton.Enabled = true;
                else
                    downButton.Enabled = false;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            selectedLayers.RemoveAt(selectedBox.SelectedIndex);
            selectedBox.Items.RemoveAt(selectedBox.SelectedIndex);
        }

        private void SelectedBox_ItemsChanged(object sender, EventArgs e)
        {
            if (!selectedBox.Enabled)
                return;
            CheckAdd();
            CheckSelected();
            if (selectedBox.Items.Count > 0)
            {
                saveButton.Enabled = true;
                fgiBox.Enabled = true;
            }
            else
            {
                saveButton.Enabled = false;
                fgiBox.Enabled = false;
            }
            ResultImage = GenerateImage(selectedLayers);
            ShowFgi(ResultImage);
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            MoveSelected(selectedBox.SelectedIndex, true);
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            MoveSelected(selectedBox.SelectedIndex, false);
        }

        private void MoveSelected(int index, bool isUp)
        {
            Layer layer = selectedLayers[index];
            string item = selectedBox.Items[index].ToString();
            selectedLayers.RemoveAt(index);
            selectedBox.Enabled = false;
            selectedBox.Items.RemoveAt(index);
            if (isUp)
                index--;
            else
                index++;
            selectedBox.Enabled = true;
            selectedLayers.Insert(index, layer);
            selectedBox.Items.Insert(index, item);
            selectedBox.SelectedIndex = index;
        }

        private Image GenerateImage(List<Layer> layers)
        {
            if (layers.Count == 0)
                return null;
            int left = int.MaxValue, right = 0, top = int.MaxValue, bottom = 0;
            foreach (Layer layer in layers)
            {
                int layerRight = layer.left + layer.Image.Width;
                int layerBottom = layer.top + layer.Image.Height;
                if (layer.left < left)
                    left = layer.left;
                if (layerRight > right)
                    right = layerRight;
                if (layer.top < top)
                    top = layer.top;
                if (layerBottom > bottom)
                    bottom = layerBottom;
            }
            int width = right - left;
            int height = bottom - top;
            Image image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            foreach (Layer layer in layers)
                if (layer.opacity == 255)
                    g.DrawImage(layer.Image, layer.left - left, layer.top - top);
                else
                    g.DrawImage(Program.GetTransparent(layer.Image, layer.opacity),
                        layer.left - left, layer.top - top);
            g.Dispose();
            return image;
        }

        private void ShowFgi(Image image)
        {
            if (image == null)
                fgiBox.Image = null;
            else
                fgiBox.Image = Program.ResizeImage(image,
                    fgiBox.Width, fgiBox.Height);
        }

        private void FgiBox_Click(object sender, EventArgs e)
        {
            PictureWin pw = new PictureWin(ResultImage)
            {
                Text = GetResultName()
            };
            pw.ShowDialog();
            pw.Dispose();
        }

        private string GetResultName()
        {
            string name = character;
            foreach (Layer layer in selectedLayers)
                name += "_" + layer.layerId;
            return name;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = GetResultName(),
                Filter = "图片文件|*.png"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ResultImage.Save(sfd.FileName);
                MessageBox.Show("保存成功！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            sfd.Dispose();
        }

        private void EncodingBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (encodingBox.SelectedIndex == -1)
                return;
            if (!File.Exists(filePath.Text))
            {
                MessageBox.Show("文件不存在！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Initialize(filePath.Text, encodings[encodingBox.SelectedIndex]);
        }

        private int DetectEncoding(string path)
        {
            Dictionary<int, int> errorChars = new Dictionary<int, int>();
            for (int i = 0; i < encodings.Length; i++)
            {
                errorChars.Add(i, 0);
                using (StreamReader sr = new StreamReader(path, encodings[i]))
                {
                    foreach (char c in sr.ReadToEnd())
                    {
                        int unicode = c;
                        if (unicode < 0x0009 ||
                            unicode > 0x000a && unicode < 0x000d ||
                            unicode > 0x000d && unicode < 0x0020 ||
                            unicode > 0x007e && unicode < 0x3000 ||
                            unicode > 0x301f && unicode < 0x3041 ||
                            unicode > 0x30ff && unicode < 0x4e00 ||
                            unicode > 0x9fff && unicode < 0xff01 ||
                            unicode > 0xff9f)
                            errorChars[i]++;
                    }
                }
            }
            return errorChars.Aggregate((a, b) => a.Value < b.Value ? a : b).Key;
        }

        private void DetectEncoding_Click(object sender, EventArgs e)
        {
            if (!File.Exists(filePath.Text))
            {
                MessageBox.Show("文件不存在！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            encodingBox.SelectedIndex = DetectEncoding(filePath.Text);
        }

        private void AutoSort_CheckedChanged(object sender, EventArgs e)
        {
            if (autoSort.Checked)
                SortSelectedLayers();
        }

        private void SortSelectedLayers()
        {
            if (selectedLayers.Count == 0)
                return;
            SortLayers(selectedLayers);
            selectedBox.Enabled = false;
            selectedBox.Items.Clear();
            for (int i = 0; i < selectedLayers.Count; i++)
            {
                if (i == selectedLayers.Count - 1)
                    selectedBox.Enabled = true;
                selectedBox.Items.Add(selectedLayers[i].name);
            }
        }

        private void SortLayers(List<Layer> layers)
        {
            foreach (Layer layer in layers)
                if (!layerAlphas.ContainsKey(layer.layerId))
                    layerAlphas.Add(layer.layerId,
                        (int)((long)Program.AddAlphas(layer.Image) * layer.opacity / 255));
            layers.Sort(new Comparison<Layer>((Layer a, Layer b)
                => layerAlphas[b.layerId] - layerAlphas[a.layerId]));
        }
    }
}
