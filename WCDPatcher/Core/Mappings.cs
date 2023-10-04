
using WCDPatcher.Core.IO;
namespace WCDPatcher.Core
{
    public static class Mappings
    {
        static System.Collections.Generic.Dictionary<string, byte[]> _gameAllowanceMappings;
        public static byte[] GetAllowanceBlock(GameIDs games)
        {
            int combInt = (int)games;
            byte[] resultingByte = System.BitConverter.GetBytes(combInt);

            byte[] final = { 0x00, 0x00 };
            for (int i = 0; i < 2; i++)
            {
                final[i] = resultingByte[i];
            }
            System.Array.Reverse(final);
            return final;
        }

        [System.Flags]
        public enum GameIDs
        {
            Pearl = 0x0008,
            Diamond = 0x0004,
            Platinum = 0x0010,
            HeartGold = 0x8000,
            SoulSilver = 0x0001
        }

        public static void AddDefaultBindings()
        {
            addMapping("No change", null);
            addMapping("All games", GetAllowanceBlock(GameIDs.Diamond | GameIDs.Pearl | GameIDs.Platinum | GameIDs.SoulSilver | GameIDs.HeartGold));
            addMapping("Diamond only",GetAllowanceBlock(GameIDs.Diamond));
            addMapping("Pearl only",GetAllowanceBlock(GameIDs.Pearl));
            addMapping("Platinum only", GetAllowanceBlock(GameIDs.Platinum));
            addMapping("Diamond and Pearl", GetAllowanceBlock(GameIDs.Diamond | GameIDs.Pearl));
            addMapping("Diamond, Pearl and Platinum", GetAllowanceBlock(GameIDs.Diamond | GameIDs.Pearl | GameIDs.Platinum));
            addMapping("HeartGold only", GetAllowanceBlock(GameIDs.HeartGold));
            addMapping("SoulSilver only", GetAllowanceBlock(GameIDs.SoulSilver));
            addMapping("HeartGold and SoulSilver", GetAllowanceBlock(GameIDs.HeartGold | GameIDs.SoulSilver));
        }

        /// <summary>
        /// Looks up the relevant bytes for the specified key
        /// </summary>
        /// <param name="version">The specified key</param>
        /// <returns></returns>
        public static byte[] LookupMapping(string version)
        {
            if (ByteTools.IsValidFourWORDHexString(version))
            {
                return ByteTools.ConvertHexStringToBytes(version);
            }
            if (!_gameAllowanceMappings.ContainsKey(version)) return null;
            return _gameAllowanceMappings[version];
        }

        /// <summary>
        /// Looks up a key from a set of bytes
        /// </summary>
        /// <param name="data">The bytes too retrieve a key for</param>
        /// <returns></returns>
        public static string LookupMappingKey(byte[] data)
        {

            if (data == null) return ""; if (data.Length != 4) return ""; if (_gameAllowanceMappings == null) Init_gameAllowanceMappings();

            string[] arrayS = IO.ArrayTools.GetKeyArray(_gameAllowanceMappings);

            for (int i = 0; i < arrayS.Length; i++)
            {
                if (ByteTools.IsByteArrayEqual(_gameAllowanceMappings[arrayS[i]], data))
                {
                    return arrayS[i];
                }
            }

            return "";
        }

        /// <summary>
        /// Checks to see if a mapping exists for that key
        /// </summary>
        /// <param name="version">Key to check</param>
        /// <returns></returns>
        public static bool DoesKeyMappingExist(string version)
        {
            if (_gameAllowanceMappings == null) Init_gameAllowanceMappings();
            return _gameAllowanceMappings.ContainsKey(version);
        }

        /// <summary>
        /// Checks to see if their is a key for the specified bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool DoesByteMappingExist(byte[] buffer)
        {
            if (_gameAllowanceMappings == null) Init_gameAllowanceMappings();
            return _gameAllowanceMappings.ContainsValue(buffer);
        }

        private static void addMapping(string key,byte[] block)
        {
            if (_gameAllowanceMappings == null) Init_gameAllowanceMappings();
            _gameAllowanceMappings.Add(key, block);
        }
        private static void Init_gameAllowanceMappings()
        {
            _gameAllowanceMappings = new System.Collections.Generic.Dictionary<string, byte[]>();
        }

        public static string[] GetKeys()
        {
            if (_gameAllowanceMappings == null) Init_gameAllowanceMappings();
            return ArrayTools.GetKeyArray(_gameAllowanceMappings);
        }

       


    }
}
