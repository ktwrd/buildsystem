using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using KBuildSystem.App.Helpers;

namespace KBuildSystem.App.Configuration
{
    public static class ConfigManager
    {
        public static readonly NumberFormatInfo numberFormat = new CultureInfo(@"en-US", false).NumberFormat;
        public readonly static Dictionary<string, object> Configuration = new Dictionary<string, object>();

        public static readonly string ApplicationDirectory = AppContext.BaseDirectory;
        public static readonly string ConfigDirectory = Path.Combine(ApplicationDirectory, @"AppConfig");

        public static Bindable<string> svwsAddress;
        public static BindableInt svwsPort;
        public static BindableBool svwsInsecure;
        public static Bindable<string> sysRootDataLocation;

        static ConfigManager()
        {
            Initialize();
        }
        public static void Initialize()
        {
            if (Configuration.Count > 0) return;

            foreach (KeyValuePair<string, string> entry in ConfigFilenames)
            {
                ReadConfigFile(Path.Combine(ConfigDirectory, entry.Value), entry);
            }

            svwsAddress = ReadString(@"svwsAddress", "127.0.0.1");
            svwsPort = ReadInt(@"svwsPort", 8090, 0, 65535);
            svwsInsecure = ReadBool(@"svwsInsecure", true);

            sysRootDataLocation = ReadString(@"sysRootDataLocation", Path.Combine(ApplicationDirectory, @"build_data"));

            SaveConfig();

            Console.WriteLine(GeneralHelper.FormatHeader(@"CONFIG START"));
            foreach (KeyValuePair<string, object> pair in Configuration)
            {
                Console.WriteLine(String.Format(@"{0}={1}", pair.Key.PadRight(39), pair.Value.ToString()));
            }
            Console.WriteLine(GeneralHelper.FormatHeader(@"CONFIG END"));
        }
        private static Dictionary<string, string> ConfigFilenames = new Dictionary<string, string>()
        {
            { @"global",    @"global.cfg"    },
            { @"sys",       @"system.cfg"    },
            { @"sv",        @"services.cfg"  }
        };

        private static void ReadConfigFile(string filename, KeyValuePair<string, string> entry)
        {
            if (!File.Exists(filename))
                return;

            using (StreamReader r = File.OpenText(filename))
                while (!r.EndOfStream)
                {
                    string l = r.ReadLine();
                    if (l.Length == 0 || l[0] == '#') continue;

                    string[] line = l.Split('=');
                    if (line.Length < 2)
                        continue;
                    if (!line[0].StartsWith(entry.Key))
                        continue;
                    Configuration[line[0].Trim()] = new Regex(String.Format(@"^{0}( |)=( |)", line[0].Trim())).Replace(l, @"");
                }
            Console.WriteLine(String.Format(@"Read config from: {0}", filename));
        }

        public static void SaveConfig()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            foreach (KeyValuePair<string, string> entry in ConfigFilenames)
            {
                string location = Path.Combine(ConfigDirectory, entry.Value);
                if (entry.Key == @"global")
                {
                    location = Path.Combine(ApplicationDirectory, entry.Value);
                }
                writeToFile(location);
                Console.WriteLine(String.Format(@"Wrote config to: {0}", location));
            }
        }
        private static bool writeToFile(string location)
        {
            string prefix = @"";
            foreach (KeyValuePair<string, string> entry in ConfigFilenames)
            {
                if (Path.GetFileName(location) == entry.Value)
                {
                    prefix = entry.Key;
                    break;
                }
            }
            try
            {
                using (Stream stream = new SafeWriteStream(location))
                using (StreamWriter w = new StreamWriter(stream))
                {
                    foreach (KeyValuePair<string, object> p in Configuration)
                    {
                        string regexFormula = String.Format("^{0}", prefix);
                        Regex rx = new Regex(regexFormula);
                        Match doesMatch = rx.Match(p.Key);
                        if (doesMatch.Success)
                        {
                            w.WriteLine(String.Format(@"{0} = {1}", p.Key, p.Value));
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static Bindable<T> ReadValue<T>(string key, T defaultValue, bool strictEnum = false) where T : IComparable
        {
            object test;
            T val = defaultValue;
            int intVal = 0;

            if (Configuration.TryGetValue(key, out test))
            {
                if (typeof(T).IsEnum)
                {
                    if (Int32.TryParse(test.ToString(), System.Globalization.NumberStyles.Any, numberFormat, out intVal))
                    {
                        if (!strictEnum) val = (T)(object)intVal;
                    }
                    else
                    {
                        try
                        {
                            val = (T)Enum.Parse(typeof(T), test.ToString(), true);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                    val = (T)test;

            }

            Bindable<T> ret = new Bindable<T>(val) { Description = key, Default = defaultValue };
            Configuration[key] = ret;
            return ret;
        }
        private static Bindable<string> ReadString(string key, string defaultValue)
        {
            return ReadValue<string>(key, defaultValue);
        }
        private static string ReadStringRaw(string key, string defaultValue)
        {
            string stringVal = defaultValue;
            object test;
            if (Configuration.TryGetValue(key, out test))
            {
                System.Diagnostics.Debug.Assert(!(test is HasObjectValue));
                stringVal = test.ToString();
            }

            return stringVal;
        }
        private static BindableBool ReadBool(string key, bool defaultValue)
        {
            BindableBool ret = new BindableBool(ReadBoolRaw(key, defaultValue)) { Default = defaultValue, Description = key };
            Configuration[key] = ret;
            return ret;
        }
        private static bool ReadBoolRaw(string key, bool defaultValue)
        {
            int intVal = 0;
            bool boolVal = defaultValue;
            object test;
            if (Configuration.TryGetValue(key, out test))
            {
                System.Diagnostics.Debug.Assert(!(test is HasObjectValue));

                if (Int32.TryParse(test.ToString(), System.Globalization.NumberStyles.Any, numberFormat, out intVal))
                    boolVal = intVal == 1;
            }

            return boolVal;
        }
        private static BindableBool ReadMigrateBool(string oldName, string newName, bool defaultValue)
        {
            if (Configuration.ContainsKey(oldName))
            {
                bool previousValue = ReadBoolRaw(oldName, defaultValue);
                Configuration.Remove(oldName);
                // we also want to remove the newer value, because we assume the user may have switched back to another stream in the mean time.
                Configuration.Remove(newName);

                return ReadBool(newName, previousValue);
            }
            return ReadBool(newName, defaultValue);
        }
        private static BindableInt ReadInt(string key, int defaultValue, int minValue = Int32.MinValue, int maxValue = Int32.MaxValue)
        {
            BindableInt ret = new BindableInt(ReadIntRaw(key, defaultValue)) { MinValue = minValue, MaxValue = maxValue, Default = defaultValue };
            Configuration[key] = ret;
            return ret;
        }
        private static int ReadIntRaw(string key, int defaultValue)
        {
            int val = defaultValue;
            object test;
            if (Configuration.TryGetValue(key, out test))
            {
                System.Diagnostics.Debug.Assert(!(test is HasObjectValue));

                if (!Int32.TryParse(test.ToString(), System.Globalization.NumberStyles.Any, numberFormat, out val))
                    val = defaultValue;
            }

            return val;
        }
        private static BindableFloat ReadFloat(string key, float defaultValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            BindableFloat ret = new BindableFloat(ReadFloatRaw(key, defaultValue)) { MinValue = minValue, MaxValue = maxValue, Default = defaultValue };
            Configuration[key] = ret;
            return ret;
        }
        private static float ReadFloatRaw(string key, float defaultValue)
        {
            float val = defaultValue;
            object test;
            if (Configuration.TryGetValue(key, out test))
            {
                System.Diagnostics.Debug.Assert(!(test is HasObjectValue));

                if (!float.TryParse(test.ToString(), System.Globalization.NumberStyles.Any, numberFormat, out val))
                    val = defaultValue;
            }

            return val;
        }
        private static BindableDouble ReadDouble(string key, double defaultValue, double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            BindableDouble ret = new BindableDouble(ReadDoubleRaw(key, defaultValue)) { MinValue = minValue, MaxValue = maxValue, Default = defaultValue };
            Configuration[key] = ret;
            return ret;
        }
        private static double ReadDoubleRaw(string key, double defaultValue)
        {
            double val = defaultValue;
            object test;
            if (Configuration.TryGetValue(key, out test))
            {
                System.Diagnostics.Debug.Assert(!(test is HasObjectValue));

                if (!Double.TryParse(test.ToString(), System.Globalization.NumberStyles.Any, numberFormat, out val))
                    val = defaultValue;
            }

            return val;
        }
    }
}
