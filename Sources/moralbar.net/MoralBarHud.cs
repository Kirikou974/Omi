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

        public int SaveFrequency
        {
            get;
            set;
        }
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

        public enum MoralState
        {
            Idle = 1,
            Raising = 2,
            Decreasing = 3
        }

        public MoralState State = MoralState.Idle;

        public List<FunPlace> FunPlaces = new List<FunPlace>();
        public FunPlace ActiveFunPlace
        {
            get;
            set;
        }
        public HUD Hud;

        public MoralBarHud()
        {
            Config = Helpers.GetConfig(Helpers.MoralBarConfig.CONFIG_FILE_NAME);
            float warningLevel = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.WARNING_LEVEL, "15"));
            SaveFrequency = Convert.ToInt32(Config.Get(Helpers.MoralBarConfig.SAVE_FREQUENCY, "3600"));
            Hud = new HUD(warningLevel);

            FunPlaceEntered = false;
            FunPlaceExited = true;

            FunPlaces = FunPlace.GetFunPlaceListFromString(Config.Get(Helpers.MoralBarConfig.FUNPLACES, "(X:0,Y:0,Z:0)"));

            EventHandlers["moralbar:moralResult"] += new Action<dynamic>((dynamic res) =>
            {
                MoralLevel = res;
                MoralInitialized = true;
            });

            EventHandlers["playerSpawned"] += new Action<dynamic>((dynamic res) =>
            {
                TriggerServerEvent("moralbar:getMoral");
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
            bool isPlayerNearFunPlace = IsPlayerNearFunPlace(playerPed);
            HandleMoralState(isPlayerNearFunPlace);
            float globalMultiplier = Helpers.StrToFloat(Config.Get(Helpers.GenericConfig.GLOBAL_MULTIPLIER, "1.5"));

            switch (State)
            {
                case MoralState.Idle:
                    break;
                case MoralState.Raising:
                    if(MoralLevel < 100)
                    {
                        MoralLevel += ActiveFunPlace.MoralRaise * globalMultiplier;
                        ActiveFunPlace.MoralTicks++;
                    }
                    break;
                case MoralState.Decreasing:
                    if (MoralLevel > 0f)
                    {
                        float moralFactor = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.FACTOR, Helpers.MoralBarConfig.FACTOR_DEFAULT));
                        MoralLevel -= moralFactor * globalMultiplier;
                    }
                    else if (Convert.ToBoolean(Config.Get(Helpers.MoralBarConfig.DEATH, "true")))
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
                    break;
                default:
                    break;
            }
        }

        private void HandleMoralState(bool isPlayerNearFunPlace)
        {
            if (isPlayerNearFunPlace)
            {
                if (State == MoralState.Decreasing)
                {
                    Screen.ShowNotification(Config.Get(Helpers.MoralBarConfig.ENTER_FUN_PLACE_TEXT, string.Empty));
                    State = MoralState.Idle;
                    return;
                }
                if (State == MoralState.Idle && Game.IsControlJustReleased(0, Control.Pickup))
                {
                    Screen.ShowNotification(Config.Get(Helpers.MoralBarConfig.HAVING_FUN_TEXT, string.Empty));
                    State = MoralState.Raising;
                    return;
                }
                if(State == MoralState.Raising && Game.IsControlJustReleased(0, Control.Pickup))
                {
                    Screen.ShowNotification(Config.Get(Helpers.MoralBarConfig.STOP_HAVING_FUN_TEXT,string.Empty));
                    State = MoralState.Idle;
                    PayForMoral();
                    ActiveFunPlace.MoralTicks = 0;
                    return;
                }
            }
            else
            {
                if (State != MoralState.Decreasing)
                {
                    Screen.ShowNotification(Config.Get(Helpers.MoralBarConfig.EXIT_FUN_PLACE_TEXT, string.Empty));
                    State = MoralState.Decreasing;
                    PayForMoral();
                    ActiveFunPlace = null;
                    return;
                }
            }
        }

        private void PayForMoral()
        {
            float ticksFactor = ActiveFunPlace.MoralTicks / Convert.ToInt32(Config.Get(Helpers.MoralBarConfig.GLOBAL_RAISE_FACTOR, "0"));
            TriggerServerEvent("moralbar:payMoral", ActiveFunPlace.MoralPrice, ticksFactor);
            SaveMoral();
        }

        public void RenderUI(Ped playerPed)
        {
            Hud.RenderBar(MoralLevel);
        }

        public async Task OnTick()
        {
            _tick++;

            Hud.ReloadScaleformMovie();
            Ped playerPed = Game.PlayerPed;

            if (MoralInitialized)
            {
                ConsumeMoral(playerPed);
                RenderUI(playerPed);

                if ((_tick % SaveFrequency) == 0 && (State == MoralState.Decreasing || State == MoralState.Idle))
                {
                    _tick = 0;
                    SaveMoral();
                }
            }
            await Task.FromResult(0);
        }

        private void SaveMoral()
        {
            TriggerServerEvent("moralbar:saveMoral", MoralLevel);
        }
    }
}


