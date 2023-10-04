
namespace WCDPatcher.Core.PCDReaders
{
   public static class InternalPCDMetaDataReader
    {
       public static string GetWonderCardRegion(string pcdPath)
       {
           byte[] tmp = IO.Patcher.ReadDataFromFile(pcdPath, WondercardPatcher.WondercardOffsets.GGID
               , 4, 0);

       

           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_English)) return "English";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_French)) return "French";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Italian)) return "Italian";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Japanese)) return "Japanese";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Korean)) return "Korean";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Spanish)) return "Spanish";

           return "";
       }
       public static string GetWonderCardRegion(byte[] GGID)
       {
           byte[] tmp = GGID;

           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_English)) return "English";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_French)) return "French";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Italian)) return "Italian";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Japanese)) return "Japanese";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Korean)) return "Korean";
           if (IO.ByteTools.IsByteArrayEqual(tmp, WondercardPatcher.WondercardData.GGID_Spanish)) return "Spanish";

           return "";
       }
    }
}
