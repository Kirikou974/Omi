using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoralBar
{
    public class MoralBarHud : BaseScript
    {
        private int _tick = 0;
        private bool _moralInitialized = false;

        public Config Config
        {
            get;
            set;
        }
        public bool FunPlaceEntered
        {
            get;
            set;
        }
        public bool FunPlaceExited
        {
            get;
            set;
        }
        public bool MoralInitialized
        {
            get
            {
                return _moralInitialized;
            }
            set
            {
                _moralInitialized = value;
            }
        }
        public float CurrentHealth
        {
            get;
            set;
        }
        public float MoralLevel
        {
            get;
            set;
        }
        public FunPlace[] FunPlaces
        {
            get;
            set;
        }
        public FunPlace ActiveFunPlace
        {
            get;
            set;
        }
        public HUD Hud;

        public MoralBarHud()
        {
            Config = Helpers.GetConfig(Helpers.MoralBarConfig.CONFIG_FILE_NAME);
            float warningLevel = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.WARNING_LEVEL, Helpers.MoralBarConfig.WARNING_LEVEL_DEFAULT));
            Hud = new HUD(warningLevel);

            FunPlaceEntered = false;
            FunPlaceExited = true;


            //TODO change. Doesn't work
            FunPlaces = FunPlace.GetFunPlaceListFromString(Config.Get(Helpers.MoralBarConfig.FUNPLACES, Helpers.MoralBarConfig.FUNPLACES_DEFAULT));

            EventHandlers["moralbar:moralResult"] += new Action<dynamic>((dynamic res) =>
            {
                MoralLevel = res;
            });
            
            Tick += OnTick;
        }

        public bool IsPlayerNearFunPlace(Ped playerPed)
        {
            bool returnValue = false;
            Vector3 playerVector = new Vector3(playerPed.Position.X, playerPed.Position.Y, playerPed.Position.Z);
            foreach (FunPlace funplace in FunPlaces)
            {
                if (Vector3.DistanceSquared(funplace.Vector, playerVector) <= funplace.Width)
                {
                    returnValue = true;
                    ActiveFunPlace = funplace;
                    continue;
                }
            }

            return returnValue;
        }
        
        public void ConsumeMoral(Ped playerPed)
        {
            bool isPlayerNearFunPlace = false;// IsPlayerNearFunPlace(playerPed);

            if (isPlayerNearFunPlace)
            {
                if (!FunPlaceEntered)
                {
                    Screen.ShowNotification("Entering fun place.");
                    FunPlaceEntered = true;
                    FunPlaceExited = false;
                }
                if (FunPlaceEntered && Game.IsControlJustReleased(0, Control.Pickup))
                {
                    TriggerServerEvent("moralbar:addMoral", ActiveFunPlace.MoralRaise, ActiveFunPlace.MoralPrice);
                }
            }
            else
            {
                if(!FunPlaceExited)
                {
                    Screen.ShowNotification("Exiting fun place.");
                    FunPlaceEntered = false;
                    FunPlaceExited = true;
                    ActiveFunPlace = null;
                }
                if (MoralLevel > 0f)
                {
                    float moralFactor = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.FACTOR, Helpers.MoralBarConfig.FACTOR_DEFAULT));
                    float globalMultiplier = Helpers.StrToFloat(Config.Get(Helpers.GenericConfig.GLOBAL_MULTIPLIER, Helpers.GenericConfig.GLOBAL_MULTIPLIER_DEFAULT));
                    MoralLevel -= moralFactor * globalMultiplier;
                }
                else if (Convert.ToBoolean(Config.Get(Helpers.MoralBarConfig.DEATH, Helpers.MoralBarConfig.DEATH_DEFAULT)))
                {
                    CurrentHealth += 0.01f;
                    if (CurrentHealth > 1f)
                    {
                        if (playerPed.Health > 51)
                        {
                            playerPed.Health = playerPed.Health - 1;
                            CurrentHealth = 0f;
                        }
                        else
                        {
                            playerPed.Health = -1;
                        }
                    }
                }
            }
        }

        public void RenderUI(Ped playerPed)
        {
            Hud.RenderBar(MoralLevel);
        }

        public async Task OnTick()
        {
            _tick++;

            if ((_tick % 320 == 0) && !MoralInitialized)
            {
                TriggerServerEvent("moralbar:getMoral");
                MoralInitialized = true;
            }

            Hud.ReloadScaleformMovie();
            Ped playerPed = Game.PlayerPed;
            ConsumeMoral(playerPed);
            RenderUI(playerPed);

            if ((_tick % 1280) == 0)
            {
                _tick = 0;
                TriggerServerEvent("moralbar:saveMoral", MoralLevel);
            }

            await Task.FromResult(0);
        }
    }
}


