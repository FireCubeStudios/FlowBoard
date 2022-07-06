using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FlowBoard.Classes
{
    public class ProjectClass
    {
        public FileClass File { get; set; }
        public string Name { get; set; }
        public StorageFile RawFile { get; set; }

        public ProjectClass(string name)
        {
            Name = name;
            File = new FileClass();
            RawFile = null;
        }
    }
}
