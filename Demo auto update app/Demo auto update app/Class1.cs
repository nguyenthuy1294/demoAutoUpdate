using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demoautoupdateapp
{
    public class ApplicationVersionInfo
    {

        public String id { get; set; }
        public List<urlFile> file { get; set; }
        public String date { get; set; }
        public String readme { get; set; }

        public ApplicationVersionInfo()
        { }

        public ApplicationVersionInfo(ApplicationVersionInfo version)
        {
            id = version.id;
            file = new List<urlFile>();
            foreach (urlFile urlfile in version.file)
                file.Add(new urlFile(urlfile));
            date = version.date;
            readme = version.readme;
        }
    }

    public class urlFile
    {
        public String inFile { get; set; }
        public String outFile { get; set; }

        public urlFile()
        { }
        public urlFile(urlFile u)
        {
            inFile = u.inFile;
            outFile = u.outFile;
        }

        public urlFile(String input, String output)
        {
            inFile = input;
            outFile = output;
        }
    }

}
