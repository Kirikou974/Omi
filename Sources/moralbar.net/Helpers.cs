using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoralBar
{
    public static class Helpers
    {
        public static Config GetConfig(string configFileName)
        {
            string configContent = null;
            try
            {
                configContent = Function.Call<string>(Hash.LOAD_RESOURCE_FILE, "moralbar", configFileName);
            }
            catch (Exception)
            {
            }

            Config config = new Config(configContent);
            return config;
        }

        public static float StrToFloat(string convStr)
        {
            return float.Parse(convStr, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static class MoralBarConfig
        {
            public const string FACTOR = "MoralFactor";
            public const string FACTOR_DEFAULT = "0.00030476";
            public const string CONFIG_FILE_NAME = "configmoral.ini";
            public const string DEATH = "DeathMoral";
            public const string DEATH_DEFAULT = "true";
            public const string WARNING_LEVEL = "WarningLevel";
            public const string WARNING_LEVEL_DEFAULT = "15";
            public const string FUNPLACES= "FunPlaces";
            public const string FUNPLACES_DEFAULT = "(X:0,Y:0,Z:0)";
            public const string FUNPLACE_PRICE = "FunPlaceDefaultCost";
            public const string FUNPLACE_PRICE_DEFAULT = "0";
            public const string FUNPLACE_RAISE = "FunPlaceDefaultRaise";
            public const string FUNPLACE_RAISE_DEFAULT = "0";
            public const string FUNPLACE_WIDTH = "FunPlaceDefaultWidth";
            public const string FUNPLACE_WIDTH_DEFAULT = "0";
        }
        public static class HUDConfig
        {
            public const string NORMAL_COLOR_RED = "HUDNormalColor_RED";
            public const string NORMAL_COLOR_GREEN = "HUDNormalColor_GREEN";
            public const string NORMAL_COLOR_BLUE = "HUDNormalColor_BLUE";
            public const string WARNING_COLOR_RED = "HUDWarningColor_RED";
            public const string WARNING_COLOR_GREEN = "HUDWarningColor_GREEN";
            public const string WARNING_COLOR_BLUE = "HUDWarningColor_BLUE";
            public const string CONFIG_FILE_NAME = "confighud.ini";
            public const string WIDTH = "HUDWidth";
            public const string WIDTH_DEFAULT = "180";
            public const string HEIGHT = "HUDHeight";
            public const string HEIGHT_DEFAULT = "6";
            public const string OFFSET = "HDOffset";
            public const string OFFSET_DEFAULT = "3";
            public const string POSITIONX = "HUDPositionX";
            public const string POSITIONY = "HUDPositionY";
        }

        public static class GenericConfig
        {
            public const string GLOBAL_MULTIPLIER = "GlobalMultiplier";
            public const string GLOBAL_MULTIPLIER_DEFAULT = "1.5";
            public const string INSTRUCTIONAL_BUTTONS = "instructional_buttons";
        }
    }
}
