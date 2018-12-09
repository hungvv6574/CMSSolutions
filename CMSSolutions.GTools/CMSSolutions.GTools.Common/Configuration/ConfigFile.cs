using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using CMSSolutions.Extensions;
using CMSSolutions.GTools.Common.Extensions;
using CMSSolutions.IO;
using CMSSolutions.GTools.Common.Models;

namespace CMSSolutions.GTools.Common.Configuration
{
    public class ConfigFile
    {
        public ConnectionDetails Connection { get; set; }

        public string SelectedGenerator { get; set; }

        [XmlIgnore]
        public string FileName { get; private set; }

        [XmlArray]
        [XmlArrayItem("Table")]
        public List<string> SelectedTables { get; set; }

        [XmlArray]
        [XmlArrayItem("Template")]
        public List<string> TemplatesToRun { get; set; }

        public ConfigFile()
        {
            TemplatesToRun = new List<string>();
        }

        public static ConfigFile Load(string fileName)
        {
            var configFile = new FileInfo(fileName).XmlDeserialize<ConfigFile>();
            configFile.FileName = fileName;
            return configFile;
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                this.SaveAs(FileName);
            }
            else
            {
                var dlgSaveFile = new SaveFileDialog();
                dlgSaveFile.Filter = "CMSSolutions GTools Files|*.vxsf";
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                {
                    this.SaveAs(dlgSaveFile.FileName);
                }
            }
        }

        public void SaveAs(string fileName)
        {
            this.XmlSerialize(fileName);
            FileName = fileName;
        }
    }
}