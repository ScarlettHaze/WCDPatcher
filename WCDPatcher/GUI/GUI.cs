using System;
using System.Windows.Forms;
using WCDPatcher.Core.Validation;
using WCDPatcher.Core;



namespace WCDPatcher.GUI
{
    public partial class GUI : Form
    {
        /// <summary>
        /// The main UI for Wondercard Patcher
        /// </summary>
        public GUI()
        {
            InitializeComponent();
            this.Icon = (System.Drawing.Icon)Properties.Resources.ResourceManager.GetObject("irkick");
            lblromdesc.Text = "ROM description (" + GetDescCount() + ")";
            Mappings.AddDefaultBindings();
            PopulateInternationalVersions();

            cmbVersion.Text = (string)cmbVersion.Items[0];
            cmbLang.SelectedIndex = 0;




        }

        /// <summary>
        /// Gets a count out of 128 for the description text box length
        /// </summary>
        /// <returns></returns>
        public string GetDescCount()
        {
            return txtDesc.Text.Length.ToString() + "/128";
        }



        /// <summary>
        /// Shows a OpenFileDialog to the user, and lets them select a file
        /// </summary>
        /// <param name="Caption">The text to display at the top of the dialog</param>
        /// <param name="Filter">The OpenFileDialog filter to use</param>
        /// <returns></returns>
        public static string GetUserFile(string Caption, string Filter)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Title = Caption;
            OFD.Filter = Filter;
            OFD.ShowDialog();
            return OFD.FileName;
        }

        /// <summary>
        /// Allows the user to select a saving location
        /// </summary>
        /// <param name="Caption">The dialog text to display</param>
        /// <param name="Filter">The SaveFileDialog filter to use</param>
        /// <returns></returns>
        public static string SaveUserFile(string Caption, string Filter)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Title = Caption;
            SFD.Filter = Filter;
            SFD.ShowDialog();
            return SFD.FileName;
        }

        private void btnBrowseDistro_Click(object sender, System.EventArgs e)
        {
            txtDistro.Text = GetUserFile("Select the Distribution ROM", "Deoxys Distribution Cart 2008 (*.nds)|*.nds");
            bool distroCheck = Validation.IsValidNDSROM(txtDistro.Text, false, true, true);
            if (!distroCheck) txtDistro.Text = "";
        }










        private void btnBrowseWondercard_Click(object sender, System.EventArgs e)
        {
            txtWondercard.Text = GetUserFile("Select the Wondercard file", "Wondercard Distribution Cards (*.pcd)|*.pcd");


        }

        private void btnBrowseNewRom_Click(object sender, System.EventArgs e)
        {
            txtNewRom.Text = SaveUserFile("Select the new ROM location", "Patched Distro (*.nds)|*.nds");
        }

        bool verboseValidateFields()
        {
            string final;
            bool activate = false;
            final = "Not all fields were valid\n\n";

            if (txtDesc.Text.Length > 128)
            {
                activate = true;
                final += "The ROM description was too long. A max of 128 characters (Including returns) is allowed.\n";
            }

            if (cmbLang.Text == "")
            { cmbLang.Focus(); return false; }

            if (activate)
            {
                MessageBox.Show(final, "Invalid Fields", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }


        }

        //void LoadDataFromLoadedRomAndLoadedWonderCard();
        //{
            //System.IO.FileStream fs = new System.IO.FileStream(txtDistro.Text, System.IO.FileMode.Open);
            //string desc = Batch.get_DESC(fs);
            //string title = Batch.get_TITLE(fs);
           // string
       // }

        private void btnPatch_Click(object sender, System.EventArgs e)
        {


            string lang, distro, wcard, newRom, desc, version, title;
            bool isValidHex, existsAsVersionString, existsAsByteArray,
                changeTitle, trimROM;
            if (txtWondercard.Text == "") return;
            if (txtNewRom.Text == "") return;
            System.Collections.Generic.Dictionary<string, string> macros = new System.Collections.Generic.Dictionary<string, string>();
            BatchProcessing.PopulateMacroList(txtDistro.Text, txtWondercard.Text, ref macros);

            lang = cmbLang.Text;
            distro = BatchProcessing.ConvertMacrosInString(txtDistro.Text, macros);
            wcard = BatchProcessing.ConvertMacrosInString(txtWondercard.Text,macros);
            newRom = BatchProcessing.ConvertMacrosInString(txtNewRom.Text,macros);
            desc = BatchProcessing.ConvertMacrosInString(txtDesc.Text,macros);
            version = cmbVersion.Text;

            changeTitle = (txtInternal.Text.Length != 0);
            trimROM = chbTrim.Checked;
            title = BatchProcessing.ConvertMacrosInString(txtInternal.Text,macros);

            isValidHex = WCDPatcher.Core.IO.ByteTools.IsValidFourWORDHexString(version);
            existsAsVersionString = Mappings.DoesKeyMappingExist(version);

            existsAsByteArray = (isValidHex ? Mappings.DoesByteMappingExist(Core.IO.ByteTools.ConvertHexStringToBytes(version))
                : false);


            bool mcOv = CheckMacroOverflow(false,macros);

            if (mcOv)
            {
                if (MessageBox.Show("Continue anyway? The overflow fields will use the original distro fields information", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }

            if (!verboseValidateFields()) return;
            if(!Validation.IsValidNDSROM(txtDistro.Text,false,true,true) && 
                !Validation.IsValidWondercard(txtWondercard.Text,false,true)) return;

            int friendDist = 0;
            if (chbNoFriendChange.Checked) friendDist = -1;
            else { friendDist = (int)nudFriendDist.Value; }

            Core.DistroPatcher.DistroPatcher.PatchDistro(wcard, distro, newRom, lang, version, desc, friendDist, (int)this.nudSeed.Value);

            if (changeTitle) Core.DistroPatcher.NDSUtilities.SetNDSTitle(newRom, title, false);
            Core.DistroPatcher.NDSUtilities.FixAllCRC(newRom);

            if (chbTrim.Checked) Core.DistroPatcher.NDSUtilities.trimRom(newRom);

            if (!System.IO.File.Exists(newRom)) return;
            MessageBox.Show("Patching completed. New rom was output to " + newRom, "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        public bool CheckMacroOverflow(bool silent,System.Collections.Generic.Dictionary<string,string> macros)
        {

            string descWMAC = BatchProcessing.ConvertMacrosInString(txtDesc.Text,macros);
            string intnmWMAC = BatchProcessing.ConvertMacrosInString(txtInternal.Text,macros);

            bool descIsOver = (descWMAC.Length > 128);
            bool intIsOver = (intnmWMAC.Length > 128);

            string finalmessage = "One or more of the input fields exceeded the maximum length once the macros were expanded. \n" +
                "The overflowed fields are as follows\n\n";

            if (descIsOver) finalmessage += "ROM description field (" + descWMAC.Length.ToString() + "/128)\n";
            if (intIsOver) finalmessage += "Internal ROM identifier (" + intnmWMAC.Length.ToString() + "/12)";

            if (!silent && (descIsOver || intIsOver)) MessageBox.Show(finalmessage, "Field overflow", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return (descIsOver || intIsOver);


        }










        private void btnBatch_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.Description = "Please selec the folder containing wondercards (*.pcd)";
            FBD.ShowDialog();
            string path = FBD.SelectedPath;

            if (!System.IO.Directory.Exists(path)) return;

            bool recursive = (MessageBox.Show("Search all subdirectories?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);

            BatchProcessing.DoWonderCardFolder(recursive, path, this);
        }

        private void btnMacros_Click(object sender, EventArgs e)
        {
            string macros = "%t = Internal ROM title (12 characters)\r\n" +
                            "%l = Original distro language\r\n" +
                            "%d = Original ROM description\r\n" +
                            "%n = Original ROM name (Description title)\r\n" +
                            "%c = Original ROM company\r\n" +
                            "%f = Wondercard parent directory name\r\n" +
                            "%w = Wondercard name (minus extension)";


            MessageBox.Show(this, "Text macros, able to be used in all fields excepting Wondercard and target game field\n\n" +
                macros, "Macro List", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (MessageBox.Show("Would you like to copy the macros to the clipboard?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Forms.Clipboard.SetText(macros);
            }
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            lblromdesc.Text = "ROM description (" + GetDescCount() + ")";
        }

        public string GetInternalCount()
        {
            return txtInternal.Text.Length + "/12";
        }

        private void txtInternal_TextChanged(object sender, EventArgs e)
        {
            lblpinternal.Text = "Internal ROM identifier ( " + GetInternalCount() + ")";
        }





        /// <summary>
        /// Populates cmbVersion with all region key
        /// </summary>
        public void PopulateInternationalVersions()
        {
            cmbVersion.Items.Clear();
            string[] keys = Mappings.GetKeys();

            for (int i = 0; i < keys.Length; i++)
            {
                string c = keys[i];
                cmbVersion.Items.Add(c);
            }

        }

        private void btnMacros_Click_1(object sender, EventArgs e)
        {
            btnMacros_Click(this, null);
        }

        private void chbNoFriendChange_CheckedChanged(object sender, EventArgs e)
        {
            if (chbNoFriendChange.Checked) nudFriendDist.Enabled = false;
            if (!chbNoFriendChange.Checked) nudFriendDist.Enabled = true;
        }

        private void chbEnableSeed_CheckedChanged(object sender, EventArgs e)
        {
            if (chbEnableSeed.Checked) nudSeed.Enabled = true;
            else { nudSeed.Enabled = false; nudSeed.Value = 0; }
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is the option that allows you to restrict your wonder card to certain games.\n" +
                "Acceptable values are the ones in the list, and a WORD value, which is a 2 byte value, like this: 0014 or 801D\n" +
                "To make a custom set of restrictions, add the below values together in hex (base-16) using for example, the Windows Vista/7 calculator\n\n" +
                "Diamond = 0004\nPearl = 0008\nPlatinum = 0010\nPokemon SoulSilver = 0001\nPokemon HeartGold = 8000\n\n" +
                "For example, to make a mystery gift only sendable to Platinum and SoulSilver, use '0011'\n\nLeaving this value as nothing will make no change to the restriction value"
                , "Help", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1); //todo: make a forms based calulator
        }

        private void GUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }







    }
}
