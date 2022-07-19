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
                Filter = "文本文件|*.txt;*.json"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
                Initialize(filePath.Text = ofd.FileName);
            ofd.Dispose();
        }

        private void FilePath_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                if (!File.Exists(filePath.Text))
                    MessageBox.Show("文件不存在！", "",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    Initialize(filePath.Text);
        }

        private void ClearAll()
        {
            encodingLabel.Enabled = false;
            encodingBox.Enabled = false;
            encodingBox.DropDownStyle = ComboBoxStyle.DropDownList;
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
            });
            GC.Collect();
        }

        private void Initialize(string path)
        {
            encodingBox.SelectedIndex = -1;
            if (Path.GetExtension(path) == ".json")
                JSONInitialize(path);
            else
                encodingBox.SelectedIndex = DetectEncoding(path);
        }

        private void TXTInitialize(string path, Encoding encoding)
        {
            ClearAll();
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
            character = Path.GetFileNameWithoutExtension(path);
            List<string> errorImages = new List<string>();
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
                            errorImages.Add(attrs[9]);
                            continue;
                        }
                    }
                    else if (attrs[0] == "2")
                        if (HasGroupLayerId(int.Parse(attrs[9]), out int index))
                            groupLayers[index].name = attrs[1];
                }
            }
            if (errorImages.Count > 0)
            {
                string message = "图片读取失败：";
                foreach (string layerId in errorImages)
                    message += $"{character}_{layerId}、";
                message = message.Substring(0, message.Length - 1);
                MessageBox.Show(message, "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            encodingLabel.Enabled = true;
            encodingBox.Enabled = true;
            detectEncoding.Enabled = true;
            if (IsEmpty(groupLayers))
            {
                MessageBox.Show("加载失败！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            InitializeGroupBox(groupBox);
            layerPanel.Enabled = true;
            batchButton.Enabled = true;
        }

        private void JSONInitialize(string path)
        {
            ClearAll();
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(path));
            character = Path.GetFileNameWithoutExtension(path);
            character = Path.GetFileNameWithoutExtension(character);
            List<string> errorImages = new List<string>();
            string json = File.ReadAllText(path);
            MatchCollection matches = Regex.Matches(json,
                "{(?:\\s*\"(?:(?:\\\\.)|[^\\\\\"])*\"\\s*:\\s*(?:\"(?:(?:\\\\.)|[^\\\\\"])*\"|\\d+)\\s*,?\\s*)*}");
            List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
            for (int i = 0; i < matches.Count; i++)
            {
                items.Add(new Dictionary<string, string>());
                MatchCollection pairs = Regex.Matches(matches[i].Value,
                "\"((?:(?:\\\\.)|[^\\\\\"])*)\"\\s*:\\s*(\"(?:(?:\\\\.)|[^\\\\\"])*\"|\\d+)");
                foreach (Match pair in pairs)
                    items[i].Add(pair.Groups[1].Value, pair.Groups[2].Value);
            }
            foreach (Dictionary<string, string> item in items)
            {
                if (!item.ContainsKey("layer_type"))
                    continue;
                int layerType = int.Parse(item["layer_type"]);
                if (layerType == 0)
                {
                    FileInfo[] infos = dir.GetFiles($"{character}_{item["layer_id"]}.*");
                    try
                    {
                        if (item.ContainsKey("group_layer_id"))
                        {
                            int groupLayerId = int.Parse(item["group_layer_id"]);
                            if (!HasGroupLayerId(groupLayerId, out int index))
                                groupLayers.Add(new GroupLayer
                                {
                                    groupLayerId = groupLayerId,
                                    layers = new List<Layer>()
                                });
                            groupLayers[index].layers.Add(new Layer
                            {
                                name = GetTrueString(item["name"]),
                                left = int.Parse(item["left"]),
                                top = int.Parse(item["top"]),
                                opacity = byte.Parse(item["opacity"]),
                                layerId = int.Parse(item["layer_id"]),
                                Image = Image.FromFile(infos[0].FullName)
                            });
                        }
                        else
                            groupLayers[0].layers.Add(new Layer
                            {
                                name = GetTrueString(item["name"]),
                                left = int.Parse(item["left"]),
                                top = int.Parse(item["top"]),
                                opacity = byte.Parse(item["opacity"]),
                                layerId = int.Parse(item["layer_id"]),
                                Image = Image.FromFile(infos[0].FullName)
                            });
                    }
                    catch (IndexOutOfRangeException)
                    {
                        errorImages.Add(item["layer_id"]);
                        continue;
                    }
                }
                else if (layerType == 2)
                    if (HasGroupLayerId(int.Parse(item["layer_id"]), out int index))
                        groupLayers[index].name = GetTrueString(item["name"]);
            }
            if (errorImages.Count > 0)
            {
                string message = "图片读取失败：";
                foreach (string layerId in errorImages)
                    message += $"{character}_{layerId}、";
                message = message.Substring(0, message.Length - 1);
                MessageBox.Show(message, "",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            encodingBox.DropDownStyle = ComboBoxStyle.DropDown;
            encodingBox.Text = "ASCII";
            if (IsEmpty(groupLayers))
            {
                MessageBox.Show("加载失败！", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            InitializeGroupBox(groupBox);
            layerPanel.Enabled = true;
            batchButton.Enabled = true;
        }

        string GetTrueString(string str)
        {
            str = str.Substring(1, str.Length - 1);
            return Regex.Unescape(str);
        }

        private bool IsEmpty(List<GroupLayer> groupLayers)
        {
            foreach (GroupLayer groupLayer in groupLayers)
                if (groupLayer.layers.Count > 0)
                    return false;
            return true;
        }

        private void InitializeGroupBox(ComboBox box)
        {
            foreach (GroupLayer groupLayer in groupLayers)
                box.Items.Add(groupLayer.name);
            box.SelectedIndex = 0;
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
                Image tempImage = Layer.GenerateImage(layers);
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

        private void SetLayerBox(ComboBox box, int index)
        {
            box.Items.Clear();
            foreach (Layer layer in groupLayers[index].layers)
                box.Items.Add(layer.name);
            box.DropDownHeight = Math.Min(box.Items.Count, 10) * 100 + 2;
            box.SelectedIndex = -1;
        }

        private void LayerBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawItem(layerBox, groupBox, e);
        }

        private void DrawItem(ComboBox layerBox, ComboBox groupBox, DrawItemEventArgs e)
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
            SetLayerBox(layerBox, groupBox.SelectedIndex);
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
                selectedBox.Items.Add(layerBox.SelectedItem);
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
                !selectedBox.Items.Contains(layerBox.SelectedItem))
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
            ResultImage = Layer.GenerateImage(selectedLayers);
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
                Text = GetResultName(selectedLayers)
            };
            pw.ShowDialog();
            pw.Dispose();
        }

        private string GetResultName(List<Layer> layers)
        {
            string name = character;
            foreach (Layer layer in layers)
                if (layer.Image != null)
                    name += "_" + layer.layerId;
            return name;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = GetResultName(selectedLayers),
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
            TXTInitialize(filePath.Text, encodings[encodingBox.SelectedIndex]);
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

        private void BatchButton_Click(object sender, EventArgs e)
        {
            BatchWin bw = new BatchWin(InitializeGroupBox,
                SetLayerBox, LayerBox_MeasureItem, DrawItem,
                (i, j) => groupLayers[i].layers[j], GetResultName);
            bw.ShowDialog();
            bw.Dispose();
        }
    }
}
