using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace JsonEditor
{
    public partial class ViewJson : Form
    {

        DataSet DsData = new DataSet();
        private BindingSource bindingSource1 = new BindingSource();
        public ViewJson()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "JSON files|*.json";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (theDialog.FileName.Trim() != string.Empty)
                    {
                        using (StreamReader r = new StreamReader(theDialog.FileName))
                        {
                            string json = r.ReadToEnd();
                            DsData = ReadDataFromJson(json, XmlReadMode.InferTypedSchema);
                           // var dsData = dataSet;

                            string dsxml = ds2json(DsData);
                            dgTasks.DataSource = DsData.Tables[1].DefaultView;
                            dgTasks.DataSource = bindingSource1;
                            dgTasks.AutoSizeRowsMode =
               DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                            dgTasks.BorderStyle = BorderStyle.Fixed3D;
                            dgTasks.EditMode = DataGridViewEditMode.EditOnEnter;
                            for (int i = 0; i < DsData.Tables.Count-1; i++)
                            {
                                

                                comboBox1.Items.Add(DsData.Tables[i].TableName);
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
         //   Tasks tasks = new Tasks();
          //  tasks.tasks = (List<Task>)dgTasks.DataSource;
            
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Filter = "JSON Image|*.json";
            saveFileDialog1.Title = "Save a JSON File";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, JsonConvert.SerializeObject(DsData, Newtonsoft.Json.Formatting.Indented));
            }

        }

        private DataSet ReadDataFromJson(string jsonString, XmlReadMode mode = XmlReadMode.Auto)
        {
            //// Note:Json convertor needs a json with one node as root
            jsonString = $"{{ \"rootNode\": {{{jsonString.Trim().TrimStart('{').TrimEnd('}')}}} }}";
            //// Now it is secure that we have always a Json with one node as root 
            var xd = JsonConvert.DeserializeXmlNode(jsonString);

            //// DataSet is able to read from XML and return a proper DataSet
            var result = new DataSet();
            result.ReadXml(new XmlNodeReader(xd), mode);
            return result;


        }

        public string ds2json(DataSet ds)
        {
            return JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented);
        }

        private void dgTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgTasks.DataSource = DsData.Tables[comboBox1.SelectedItem.ToString()];
        }
    }
}
