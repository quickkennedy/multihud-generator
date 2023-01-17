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
        public class HudFile
        {
            public string fakefile;
            public string realfile;
        }

        List<string> log = new List<string>();

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

            // GENERATE takes some file names that dont have a path nor a .res at the end, one variant besides def, and a list of the real files (relative to one folder into custom)
            //Generate(new string[] { "hudplayerhealth" }, new string[] { "scout" }, new string[] { "resource/ui/hudplayerhealth.res" });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selecthud1 = new FolderBrowserDialog();

            if (selecthud1.ShowDialog() == DialogResult.OK)
            {
                filePath1 = selecthud1.SelectedPath;

                label1.Text += filePath1 + " selected as first hud" + Environment.NewLine;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog selecthud1 = new FolderBrowserDialog();

            if (selecthud1.ShowDialog() == DialogResult.OK)
            {
                filePath2 = selecthud1.SelectedPath;

                label1.Text += filePath2 + " selected as second hud" + Environment.NewLine;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (filePath1 != null && filePath2 != null)
            {
                label1.Text += "finished copy! scanning directories for useful files." + Environment.NewLine;

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

            //_ = filePath1.Split('/');
            //string hud1 = _[_.Length];
            
            //_ = filePath2.Split('/');
            //string hud2 = _[_.Length];

            DirectoryInfo hud1DIRECTORY = new DirectoryInfo(filePath1);
            DirectoryInfo hud2DIRECTORY = new DirectoryInfo(filePath2);

            List<HudFile> hud1RES = new List<HudFile>();
            List<HudFile> hud2RES = new List<HudFile>();

            List<HudFile> hud1TXT = new List<HudFile>();
            List<HudFile> hud2TXT = new List<HudFile>();

            // RES FILES
            
            foreach (FileInfo file in hud1DIRECTORY.GetFiles("*.res", SearchOption.AllDirectories))
            {
                HudFile hudFile = new HudFile();

                hudFile.fakefile = file.Name;
                hudFile.realfile = file.FullName;

                hud1RES.Add(hudFile);
            }

            foreach (FileInfo file in hud2DIRECTORY.GetFiles("*.res", SearchOption.AllDirectories))
            {
                HudFile hudFile = new HudFile();

                hudFile.fakefile = file.Name;
                hudFile.realfile = file.FullName;

                hud2RES.Add(hudFile);
            }

            // TXT FILES

            foreach (FileInfo file in hud1DIRECTORY.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                HudFile hudFile = new HudFile();

                hudFile.fakefile = file.Name;
                hudFile.realfile = file.FullName;

                hud1TXT.Add(hudFile);
            }

            foreach (FileInfo file in hud2DIRECTORY.GetFiles("*.txt", SearchOption.AllDirectories))
            {
                HudFile hudFile = new HudFile();

                hudFile.fakefile = file.Name;
                hudFile.realfile = file.FullName;

                hud2TXT.Add(hudFile);
            }

            // FILE VALIDATION

            List<HudFile> hud1 = new List<HudFile>();
            List<HudFile> hud2 = new List<HudFile>();

            foreach (HudFile hudFile in hud1RES)
            {
                if (hudFile.realfile.Contains(@"resource\ui\") || hudFile.realfile.Contains(@"scripts\")) // verifies its in the folders i'm allowing (:<
                {
                    string file = File.ReadAllText(hudFile.realfile);
                    
                    if (!(file.Contains("event") && file.Contains("animate"))) // verifies its not an animation
                    {
                        //label1.Text += hudFile.realfile + Environment.NewLine;
                        hud1.Add(hudFile);
                    }
                }
            }

            foreach (HudFile hudFile in hud2RES)
            {
                if (hudFile.realfile.Contains(@"resource\ui\") || hudFile.realfile.Contains(@"scripts\")) // verifies its in the folders i'm allowing (:<
                {
                    string file = File.ReadAllText(hudFile.realfile);

                    if (!(file.Contains("event") && file.Contains("animate"))) // verifies its not an animation
                    {
                        hud2.Add(hudFile);
                    }
                }
            }

            #region COPY FILES OVER

            int removeText;

            removeText = filePath1.Length;

            List<HudFile> iohud1 = new List<HudFile>();

            label1.Text += "hud1 " + "(" + hud1.Count + ")" + Environment.NewLine;

            foreach (HudFile hudFile in hud1)
            {
                HudFile newHudFile = new HudFile();

                newHudFile.fakefile = hudFile.fakefile;
                newHudFile.realfile = hudFile.realfile;

                //newHudFile.realfile.Replace(@"\", "/");
                newHudFile.realfile = hudFile.realfile.Substring(removeText);

                iohud1.Add(newHudFile);
            }

            label1.Text += "iohud " + "(" + iohud1.Count + ")" + Environment.NewLine;

            // copy dirs to avoid io errors
            CopyDirs(filePath1, "MULTIHUD");

            for (int i = 0; i < iohud1.Count; ++i)
            {
                string dest = @"MULTIHUD\" + iohud1[i].realfile.Substring(1);

                dest = dest.Substring(0, dest.Length - 4) + "_def.res";

                //label1.Text += dest + Environment.NewLine;

                File.Copy(hud1[i].realfile, dest, true);
            }

            removeText = filePath2.Length;

            List<HudFile> iohud2 = new List<HudFile>();

            foreach (HudFile hudFile in hud2)
            {
                HudFile newHudFile = new HudFile();

                newHudFile.fakefile = hudFile.fakefile;
                newHudFile.realfile = hudFile.realfile;

                //newHudFile.realfile.Replace(@"\", "/");
                newHudFile.realfile = hudFile.realfile.Substring(removeText);

                iohud2.Add(newHudFile);
            }

            // copy dirs to avoid io errors
            CopyDirs(filePath2, "MULTIHUD");

            for (int i = 0; i < iohud2.Count; ++i)
            {
                string dest = @"MULTIHUD\" + iohud2[i].realfile.Substring(1);

                dest = dest.Substring(0, dest.Length - 4) + "_scout.res";

                File.Copy(hud2[i].realfile, dest, true);
            }

            #endregion

            #region GENERATION

            List<string> files = new List<string>();
            List<string> fakefiles = new List<string>();
            string[] names = new string[]
            {
                "scout"
            };

            label1.Text += "generating iohud " + "(" + iohud1.Count + ")" + Environment.NewLine;

            foreach (HudFile hudFile in iohud1)
            {
                //label1.Text += hudFile.realfile + " - " + hudFile.fakefile + Environment.NewLine;
                files.Add(hudFile.realfile);
                fakefiles.Add(hudFile.fakefile);
            }


            Generate(fakefiles.ToArray(), files.ToArray(), names);

            label1.Text += "finished generation! safe to exit now." + Environment.NewLine;

            #endregion

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
                    string newfile = file;

                    newfile = newfile.Substring(0, newfile.Length - 4);

                    cfgfile[counter3] = "sixense_write_bindings " + newfile + ".txt";

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

                    string newfile = file;

                    newfile = newfile.Substring(0, newfile.Length - 4);

                    cfgfile[counter] = "// " + newfile;
                    cfgfile[counter + 1] = "con_logfile cfg/" + newfile + ".txt";
                    //cfgfile[counter + 2] = "exec hud/" + file + "_" + name + ".cfg";
                    //echo "#base ../resource/ui/hudplayerhealth_scout.res"

                    string realfile = realfiles[counter2].Substring(0, realfiles[counter2].Length - 4); // removes .res

                    //MessageBox.Show(realfile);

                    cfgfile[counter + 2] = "echo \"#base .." + realfile + "_" + name + ".res\"";

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

            string newfile = file;

            newfile = newfile.Substring(0, newfile.Length - 4);

            if (newfile == "hudlayout")
            {
                return new string[]
                {
                    "#base \"../cfg/" + newfile + ".txt\"",
                    "#base \"" + newfile + "_def.res\"",
                    "",
                    "\"" + realfile + "\"",
                    "{",
                    "}"
                };
            }
            else
            {
                return new string[]
                {
                    "#base \"../../cfg/" + newfile + ".txt\"",
                    "#base \"" + newfile + "_def.res\"",
                    "",
                    "\"" + realfile + "\"",
                    "{",
                    "}"
                };
            }
        }

        #endregion

        #region GENERATE

        private void Generate(string[] files, string[] realfiles, string[] names)
        {
            int counter = 0;

            // .CFG INTERNAL
            
            // .CFG EXTERNAL

            if (!System.IO.Directory.Exists("MULTIHUD/cfg/swap"))
            {
                System.IO.Directory.CreateDirectory("MULTIHUD/cfg/swap");
            }

            // .RES EXTERNAL

            if (!System.IO.Directory.Exists("MULTIHUD/resource"))
            {
                System.IO.Directory.CreateDirectory("MULTIHUD/resource");
            }

            // files that once exec'd will swap entire huds
            List<string[]> hudcfgfiles = CfgFile(files, realfiles, names); // OWNED

            counter = 0;

            List<string> names2 = names.ToList<string>();

            names2.Add("def");

            string[] names3 = names2.ToArray();

            foreach (string[] hudcfgfile in hudcfgfiles)
            {
                File.WriteAllLines("MULTIHUD/cfg/swap/" + names3[counter] + ".cfg", hudcfgfile);

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
                string[] dirAsArray = ("MULTIHUD/" + realfiles[counter]).Split('/');

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

                File.WriteAllLines("MULTIHUD/" + realfiles[counter], resfile);

                ++counter;
            }

            File.WriteAllText("MULTIHUD/info.vdf", "\"hud swapper\" { \"ui_version\" \"3\" }");
        }

        #endregion

        #region COPY
        
        private void CopyDirs(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            // as stolen from https://code-maze.com/copy-entire-directory-charp/

            var allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
            foreach (string dir in allDirectories)
            {
                string dirToCreate = dir.Replace(sourceDir, destinationDir);
                Directory.CreateDirectory(dirToCreate);
            }
        }

        private void CopyFile(string sourceFile, string destinationFile)
        {
            File.Copy(sourceFile, destinationFile, true);
        }

        private void Copy(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            // as stolen from https://code-maze.com/copy-entire-directory-charp/

            var allDirectories = Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories);
            foreach (string dir in allDirectories)
            {
                string dirToCreate = dir.Replace(sourceDir, destinationDir);
                Directory.CreateDirectory(dirToCreate);
            }

            var allFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            foreach (string newPath in allFiles)
            {
                try
                {
                    File.Copy(newPath, newPath.Replace(sourceDir, destinationDir), true);
                }
                catch (Exception e)
                {
                    label1.Text += e.ToString() + Environment.NewLine;
                }
            }
        }


        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "";
        }
    }
}
