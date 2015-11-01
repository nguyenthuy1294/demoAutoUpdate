using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Demoautoupdateapp
{

  
    public partial class Form1 : Form
    {
        private String urlFileConfig = "https://github.com/nguyenthuy1294/demoAutoUpdate/blob/master/version.config";
        private  String outFileConfig = Application.StartupPath+ @"\version.config";
        private String appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                txtResult.Text = (int.Parse(txtNo1.Text) + int.Parse(txtNo2.Text)).ToString();
            }
            catch (Exception ex) {                
                txtResult.Text = "Error";
            }
        }

        private void cbCheckVersion_CheckedChanged(object sender, EventArgs e)
        {
            lbversion.Text = "";
            if (cbCheckVersion.Checked)
            {
                //get version in app
                lbversion.Text = "AssemblyVersion: "+Assembly.GetExecutingAssembly().GetName().Version.ToString();
                lbversion.Text += " \nAssemblyFileVersion: " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //get fiel from web 

            lbStatus.Text = "Download config.";
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri(urlFileConfig),
                 outFileConfig );
            }

            lbStatus.Text += "\nCheck current version";
              List<  ApplicationVersionInfo> list = new List<ApplicationVersionInfo>();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(outFileConfig);

                //read struct data
                XmlNodeList root = doc.SelectNodes("/listVersion/version");

                //read version
                foreach (XmlNode version in root)
                {
                    ApplicationVersionInfo data = new ApplicationVersionInfo();
                    data.id = version.SelectSingleNode("id").InnerText;
                    data.date = version.SelectSingleNode("date").InnerText;
                    data.readme = version.SelectSingleNode("readme").InnerText;
                    foreach (XmlNode files in version.SelectNodes("files"))
                    {
                        data.file.Add(new urlFile( files.SelectSingleNode("file").SelectSingleNode("in").InnerText,  files.SelectSingleNode("file").SelectSingleNode("out").InnerText ));
                    }

                    list.Add(data);
                }

                lbStatus.Text += "\nDelete config";
                System.IO.File.Delete(outFileConfig);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (list.Count > 0)
            {
                //max version (id)
                ApplicationVersionInfo versionMax = list[0];
                if (list.Count > 1)
                    for (int i = 1; i < list.Count; i++)
                    {
                        if (list[i].id.CompareTo(versionMax.id) > 0)
                        {
                            versionMax = new ApplicationVersionInfo(list[i]);
                        }
                    }

                //check current version

                if (versionMax.id.CompareTo(appVersion) <= 0)
                {
                    lbStatus.Text += "\n\nit is current version";
                }
                else
                {
                    bool exe = false;
                    //download file
                    lbStatus.Text += "\nDownload file app...";
                    txtReadme.Text = versionMax.readme;
                    foreach (urlFile url in versionMax.file)
                    {
                        if (url.outFile.Contains(".exe"))
                        {
                            exe = true;
                        }
                        lbStatus.Text += "\n    File: "+ url.outFile;
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                            wc.DownloadFileAsync(new System.Uri(url.inFile), Application.StartupPath + @"\"+url.outFile );
                        }
                    }

                    if (exe)
                        Application.Restart();
                }
            }
            else
                lbStatus.Text += "\n\nversion not found";
            
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            try
            {
                txtResult.Text = (int.Parse(txtNo1.Text) - int.Parse(txtNo2.Text)).ToString();
            }
            catch (Exception ex) { txtResult.Text = "Error"; }
        }

    }
}
