using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerProcess
{
    public class DataBase
    {
        private List<string> localDB = new List<string>();
        const string path = "database";
        TextWriter tw;

        public DataBase(int id)
        {
            var fullpath = path + id.ToString() + ".txt";
            if (File.Exists(fullpath))
            {
                var lines = File.ReadLines(fullpath);
                localDB = lines.ToList();
                tw = new StreamWriter(fullpath, true);
            }
            else
            {
                var x = File.Create(fullpath);
                x.Close();
                tw = new StreamWriter(fullpath);

            }
        }

        ~DataBase()
        {
            try
            {
                tw.Close();
            }
            catch
            {

            }
            localDB.Clear();
        }

        public void Insert(string s)
        {
            localDB.Add(s);
            tw.WriteLine(s);
        }

        public string GetLine(int line)
        {
            if(line >=0 && line < localDB.Count())
            {
                return localDB[line];
            }
            return "";
        }
    }
}
