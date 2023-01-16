﻿using System;
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

            generateCfgFiles(new string[] { "hudplayerclass" }, classes, new string[] { "resource/ui/hudplayerclass.res" });
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
                Generate();
            }
            else // either one of them is null so user fucked up and didnt select a hud properly
            {
                MessageBox.Show("error code 1 - make sure both huds are selected");
            }
        }

        private void Generate()
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

        private string[] CfgFile(string file, string[] names)
        {
            /*
                alias hudplayerhealth_clear "sixense_clear_bindings; sixense_write_bindings hud_hudplayerhealth.txt"
                alias hudplayerhealth_log "con_filter_text #base; con_filter_enable 1; con_logfile cfg/hud_hudplayerhealth.txt"
                alias hudplayerhealth_unlog "con_logfile console.log"

                alias hudplayerhealth_alt "hudplayerhealth_clear; hudplayerhealth_log; exec hud/hudplayerhealth_alt.cfg; hudplayerhealth_unlog; hud_reloadscheme"
                alias hudplayerhealth_def "hudplayerhealth_clear; hud_reloadscheme"
            */

            string[] output = new string[5 + names.Length];

            output[0] = "alias " + file + "_clear \"sixense_clear_bindings; sixense_write_bindings hud_" + file + ".txt\"";
            output[1] = "alias " + file + "_log \"con_filter_text #base; con_filter_enable 1; con_logfile cfg/hud_" + file + ".txt\"";
            output[2] = "alias " + file + "_unlog \"con_logfile console.log\"";

            int counter = 4;

            foreach (string name in names)
            {
                output[counter] = "alias " + file + "_" + name + " \"" + file + "_clear; " + file + "_log; exec hud/" + file + "_" + name + ".cfg; " + file + "_unlog; hud_reloadscheme\"";
                
                counter++;
            }

            output[output.Length - 1] = "alias " + file + "_def \"" + file + " _clear; hud_reloadscheme\"";

            return output;
        }

        private string[] CfgFile2(string file, string[] names)
        {
            //echo "#base ../resource/ui/hudplayerhealth_alt.res"

            string[] output = new string[names.Length];

            int counter = 0;

            foreach (string name in names)
            {
                output[counter] = "echo \"#base ../resource/ui/" + file + "_" + name + ".res\"";

                ++counter;
            }

            return output;
        }

        private List<string[]> CfgFile3(string[] files, string[] names)
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

            foreach (string name in names)
            {
                string[] cfgfile = new string[13 + 4 * files.Length];

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
                cfgfile[2] = "sixense_write_bindings file1.txt";
                cfgfile[3] = "sixense_write_bindings file2.txt";
                cfgfile[4] = "";
                cfgfile[5] = "// shared setup";
                cfgfile[6] = "con_filter_text #base";
                cfgfile[7] = "con_filter_enable 1";
                cfgfile[8] = "";

                int counter = 9;

                foreach (string file in files)
                {
                    /*
                    
                    // file 1
                    con_logfile cfg/file1.txt
                    exec hud/file1.cfg 
                    */
                    cfgfile[counter] = "// " + file;
                    cfgfile[counter + 1] = "con_logfile cfg/" + file + ".txt";
                    cfgfile[counter + 2] = "exec hud/" + file + ".cfg";
                    cfgfile[counter + 3] = "";

                    counter += 4;
                }

                output.Add(cfgfile);
            }

            return output;
        }

        private void generateCfgFiles(string[] files, string[] names, string[] realfiles)
        {
            int counter = 0;

            // .CFG INTERNAL

            if (!System.IO.Directory.Exists("hud_cfg_internal/cfg"))
            {
                System.IO.Directory.CreateDirectory("hud_cfg_internal/cfg");
            }

            if (!System.IO.Directory.Exists("hud_cfg_internal/hud"))
            {
                System.IO.Directory.CreateDirectory("hud_cfg_internal/hud");
            }

            // .CFG EXTERNAL

            if (!System.IO.Directory.Exists("hud_cfg_external/cfg"))
            {
                System.IO.Directory.CreateDirectory("hud_cfg_external/cfg");
            }

            // .RES EXTERNAL

            if (!System.IO.Directory.Exists("hud_res_external/resource"))
            {
                System.IO.Directory.CreateDirectory("hud_res_external/resource");
            }

            foreach (string file in files)
            {
                string[] txt = CfgFile(file, names);

                File.WriteAllLines("hud_cfg_internal/cfg/" + file + ".txt", txt);

                string[] cfgs = CfgFile2(file, names);

                counter = 0;

                foreach (string cfg in cfgs)
                {
                    File.WriteAllText("hud_cfg_internal/hud/" + file + "_" + names[counter] + ".cfg", cfg);
                    
                    ++counter;
                }
            }

            // files that once exec'd will swap entire huds
            List<string[]> hudcfgfiles = CfgFile3(files, names); // OWNED

            counter = 0;

            foreach (string[] hudcfgfile in hudcfgfiles)
            {
                File.WriteAllLines("hud_cfg_external/cfg/" + names[counter] + ".cfg", hudcfgfile);

                ++counter;
            }

            // fake res files

            List<string[]> resfiles = new List<string[]>();

            counter = 0;

            foreach (string file in files)
            {
                resfiles.Add(Resfile(file, realfiles[counter]));

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
        }

        private string[] Resfile(string file, string realfile)
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
                "#base \"../../cfg/hud_" + file + ".txt\"",
                "#base \"" + file + "_def.res\"",
                "",
                "\"" + realfile + ".res\"",
                "{",
                "}"
            };
        }
    }
}
