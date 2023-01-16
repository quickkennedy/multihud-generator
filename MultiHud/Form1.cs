using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiHud
{
    public partial class Form1 : Form
    {
        string filePath1 = null;
        string filePath2 = null;

        readonly string[] ignore = new string[] {
            "resource/",
            "resource/scheme/",
        };

        readonly string[] SHUTUP = new string[] {
            "resource/",
            "resource/scheme/",
        };

        readonly string[] classes = new string[] {
            "scout",
            "solly",
            "pyro",
            "demo",
            "heavy",
            "med",
            "engi",
            "sniper",
            "spy",
        };

        public Form1()
        {
            InitializeComponent();

            Generate(new string[] { "hudplayerhealth" }, new string[] { "scout" }, new string[] { "resource/ui/hudplayerhealth.res" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selecthud1 = new FolderBrowserDialog();

            if (selecthud1.ShowDialog() == DialogResult.OK)
            {
                filePath1 = selecthud1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selecthud1 = new FolderBrowserDialog();

            if (selecthud1.ShowDialog() == DialogResult.OK)
            {
                filePath2 = selecthud1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (filePath1 != null && filePath2 != null)
            {
                Scan();
            }
            else // either one of them is null so user fucked up and didnt select a hud properly
            {
                MessageBox.Show("error code 1 - make sure both huds are selected");
            }
        }

        #region SCAN

        private void Scan()
        {
            string[] _;

            _ = filePath1.Split('/');
            string hud1 = _[_.Length];
            
            _ = filePath2.Split('/');
            string hud2 = _[_.Length];

            DirectoryInfo hud1DIRECTORY = new DirectoryInfo(filePath1);
            DirectoryInfo hud2DIRECTORY = new DirectoryInfo(filePath2);

            Stack<string> hud1RES = new Stack<string>();
            Stack<string> hud2RES = new Stack<string>();

            Stack<string> hud1TXT = new Stack<string>();
            Stack<string> hud2TXT = new Stack<string>();

            // RES FILES
            
            foreach (FileInfo file in hud1DIRECTORY.GetFiles("*.res"))
            {
                hud1RES.Push(file.FullName);
            }

            foreach (FileInfo file in hud2DIRECTORY.GetFiles("*.res"))
            {
                hud2RES.Push(file.FullName);
            }

            // TXT FILES

            foreach (FileInfo file in hud1DIRECTORY.GetFiles("*.txt"))
            {
                hud1TXT.Push(file.FullName);
            }

            foreach (FileInfo file in hud2DIRECTORY.GetFiles("*.txt"))
            {
                hud2TXT.Push(file.FullName);
            }
        }

        #endregion

        #region FILE METHODS

        private List<string[]> CfgFile(string[] files, string[] realfiles, string[] names)
        {
            /*
                // clear files
                sixense_clear_bindings
                sixense_write_bindings file1.txt
                sixense_write_bindings file2.txt

                // shared setup
                con_filter_text #base
                con_filter_enable 1

                // file 1
                con_logfile cfg/file1.txt
                exec hud/file1.cfg

                // file 2
                con_logfile cfg/file2.txt
                exec hud/file2.cfg

                // cleanup
                con_logfile console.log
                hud_reloadscheme
            */

            // 12 + 4 * file

            List<string[]> output = new List<string[]>();

            List<string> names2 = names.ToList<string>();

            names2.Add("def");

            string[] names3 = names2.ToArray();

            foreach (string name in names3)
            {
                string[] cfgfile = new string[10 + 5 * files.Length];

                /*
                // clear files
                sixense_clear_bindings
                sixense_write_bindings file1.txt
                sixense_write_bindings file2.txt

                // shared setup
                con_filter_text #base
                con_filter_enable 1
                */

                cfgfile[0] = "// clear files";
                cfgfile[1] = "sixense_clear_bindings";

                int counter3 = 2;

                foreach (string file in files)
                {
                    cfgfile[counter3] = "sixense_write_bindings " + file + ".txt";

                    ++counter3;
                }

                cfgfile[counter3] = "";
                cfgfile[counter3 + 1] = "// shared setup";
                cfgfile[counter3 + 2] = "con_filter_text #base";
                cfgfile[counter3 + 3] = "con_filter_enable 1";
                cfgfile[counter3 + 4] = "";

                int counter = counter3 + 5;

                int counter2 = 0;

                foreach (string file in files)
                {
                    /*
                    
                    // file 1
                    con_logfile cfg/file1.txt
                    exec hud/file1.cfg 
                    */
                    cfgfile[counter] = "// " + file;
                    cfgfile[counter + 1] = "con_logfile cfg/" + file + ".txt";
                    //cfgfile[counter + 2] = "exec hud/" + file + "_" + name + ".cfg";
                    //echo "#base ../resource/ui/hudplayerhealth_scout.res"

                    string realfile = realfiles[counter2].Substring(0, realfiles[counter2].Length - 4); // removes .res

                    //MessageBox.Show(realfile);

                    cfgfile[counter + 2] = "echo \"#base ../" + realfile + "_" + name + ".res\"";

                    cfgfile[counter + 3] = "";

                    counter += 4;

                    ++counter2;
                }

                /*
                
                // cleanup
                con_logfile console.log
                hud_reloadscheme
                */

                cfgfile[cfgfile.Length - 4] = "";
                cfgfile[cfgfile.Length - 3] = "// cleanup";
                cfgfile[cfgfile.Length - 2] = "con_logfile console.log";
                cfgfile[cfgfile.Length - 1] = "hud_reloadscheme";

                output.Add(cfgfile);
            }

            return output;
        }

        private string[] ResFile(string file, string realfile)
        {
            /*
                #base "../../cfg/hud_hudplayerhealth.txt"
                #base "hudplayerhealth_def.res"

                "Resource/UI/HudPlayerHealth.res"
                {	
                }
            */

            return new string[]
            {
                "#base \"../../cfg/" + file + ".txt\"",
                "#base \"" + file + "_def.res\"",
                "",
                "\"" + realfile + "\"",
                "{",
                "}"
            };
        }

        #endregion

        #region GENERATE

        private void Generate(string[] files, string[] names, string[] realfiles)
        {
            int counter = 0;

            // .CFG INTERNAL
            
            // .CFG EXTERNAL

            if (!System.IO.Directory.Exists("hud_cfg_external/cfg/swap"))
            {
                System.IO.Directory.CreateDirectory("hud_cfg_external/cfg/swap");
            }

            // .RES EXTERNAL

            if (!System.IO.Directory.Exists("hud_res_external/resource"))
            {
                System.IO.Directory.CreateDirectory("hud_res_external/resource");
            }

            // files that once exec'd will swap entire huds
            List<string[]> hudcfgfiles = CfgFile(files, realfiles, names); // OWNED

            counter = 0;

            List<string> names2 = names.ToList<string>();

            names2.Add("def");

            string[] names3 = names2.ToArray();

            foreach (string[] hudcfgfile in hudcfgfiles)
            {
                File.WriteAllLines("hud_cfg_external/cfg/swap/" + names3[counter] + ".cfg", hudcfgfile);

                ++counter;
            }

            // fake res files

            List<string[]> resfiles = new List<string[]>();

            counter = 0;

            foreach (string file in files)
            {
                resfiles.Add(ResFile(file, realfiles[counter]));

                ++counter;
            }

            counter = 0;

            foreach (string[] resfile in resfiles)
            {
                string[] dirAsArray = ("hud_res_external/" + realfiles[counter]).Split('/');

                List<string> dirAsList = dirAsArray.ToList<string>();

                dirAsList.RemoveAt(dirAsList.Count - 1);

                string dirAsString = "";

                foreach (string item in dirAsList)
                {
                    dirAsString += item + "/";
                }

                if (!System.IO.Directory.Exists(dirAsString))
                {
                    System.IO.Directory.CreateDirectory(dirAsString);
                }

                File.WriteAllLines("hud_res_external/" + realfiles[counter], resfile);

                ++counter;
            }

            File.WriteAllText("hud_res_external/info.vdf", "\"hud swapper\" { \"ui_version\" \"3\" }");
        }

        #endregion
    }
}
