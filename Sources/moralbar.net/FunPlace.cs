﻿using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoralBar
{
    public class FunPlace
    {
        public Vector3 Vector { get; set; }
        public int Width { get; set; }
        public float MoralRaise { get; set; }
        public int MoralPrice { get; set; }
        public int Speed { get; set; }
        public int MoralTicks = 0;

        public FunPlace(float X, float Y, float Z, int? width = null, float? moralRaise = null, int? moralPrice = null)
        {
            Config config = Helpers.GetConfig(Helpers.MoralBarConfig.CONFIG_FILE_NAME);
            Vector = new Vector3(X, Y, Z);
            if(width.HasValue)
            {
                Width = width.Value;
            }
            else
            {
                Width = Convert.ToInt32(config.Get(Helpers.MoralBarConfig.FUNPLACE_WIDTH, "0"));
            }
            if(moralRaise.HasValue)
            {
                MoralRaise = moralRaise.Value;
            }
            else
            {
                MoralRaise = Helpers.StrToFloat(config.Get(Helpers.MoralBarConfig.FUNPLACE_RAISE, "0"));
            }
            if (moralPrice.HasValue)
            { 
                MoralPrice = moralPrice.Value;
            }
            else
            {
                MoralPrice = Convert.ToInt32(config.Get(Helpers.MoralBarConfig.FUNPLACE_PRICE, "0"));
            }
        }

        public static List<FunPlace> GetFunPlaceListFromString(string vectorsString)
        {
            List<FunPlace> funPlaces = new List<FunPlace>();
            string[] vectorStringArray = vectorsString.Split('|');

            foreach (string vectorString in vectorStringArray)
            {
                funPlaces.Add(GetFunPlaceFromString(vectorString));
            }

            return funPlaces;
        }

        public static FunPlace GetFunPlaceFromString(string vectorString)
        {
            string tempVector = vectorString.Trim('(').Trim(')');
            string[] positions = tempVector.Split(',');
            float x = 0;
            float y = 0;
            float z = 0;
            int? width = null;
            int? price = null;
            float? raise = null;

            foreach (string position in positions)
            {
                string[] detail = position.Split(':');

                var value = detail[1];

                switch (detail[0])
                {
                    case "X":
                        x = Helpers.StrToFloat(value);
                        break;
                    case "Y":
                        y = Helpers.StrToFloat(value);
                        break;
                    case "Z":
                        z = Helpers.StrToFloat(value);
                        break;
                    case "Price":
                        price = Convert.ToInt32(value);
                        break;
                    case "Width":
                        width = Convert.ToInt32(value);
                        break;
                    case "Raise":
                        raise = Helpers.StrToFloat(value);
                        break;
                    default:
                        break;
                }
            }
            
            FunPlace funplace = new FunPlace(x, y, z, width, raise, price);

            return funplace;
        }
    }
}