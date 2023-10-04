using System.Windows.Forms;

namespace WCDPatcher.Core.Validation
{
    public static class ErrorDialogs
    {
        public static DialogResult FileNotFoundBox(string filepath, MessageBoxButtons buttons, bool Severe,string extraInfoOrQuestion="")
        {
            return MessageBox.Show("The selected file '" + filepath + "' could not be found\n" +
                "Please make sure the file exists, and try again" + (extraInfoOrQuestion == "" ? "" : "\n\n" + extraInfoOrQuestion)
                , "File not found", buttons, (Severe ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation));
        }

        public static DialogResult WonderCardIsInvalid(string filepath, MessageBoxButtons buttons, bool Severe, string extraInfoOrQuestion)
        {
            return MessageBox.Show("The selected wondercard file '" + filepath + "' did not match wondercard criteria. It may be an invalid wondercard file\n" +
                "Please make sure the file is a wondercard file and try again" + (extraInfoOrQuestion == "" ? "" : "\n\n" + extraInfoOrQuestion)
                , "Wondercard validation failed", buttons, (Severe ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation));
        }

        public static DialogResult NDSFileIsInvalid(string Filepath, MessageBoxButtons buttons, bool Severe, string extraInfoOrQuestion)
        {
            return MessageBox.Show("The selected file '" + Filepath + "' is not a valid NDS ROM\n" +
                "Please make sure the file is a NDS ROM and try again" + (extraInfoOrQuestion == "" ? "" : "\n\n" + extraInfoOrQuestion)
                , "NDS ROM is invalid", buttons, (Severe ? MessageBoxIcon.Error : MessageBoxIcon.Exclamation));
        }
    }
}
