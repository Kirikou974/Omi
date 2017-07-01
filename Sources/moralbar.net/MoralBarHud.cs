using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MoralBar
{
    public class MoralBarHud : BaseScript
    {
        //private bool isHavingFun;
        private float _moralLevel = 100f;
        private bool _moralInitialized = false;
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
        protected Config Config
        {
            get;
            set;
        }

        private int _tick = 0;
        public float currentHealth;
        public float MoralLevel
        {
            get
            {
                return _moralLevel;
            }

            set
            {
                _moralLevel = value;
            }
        }

        public HUD hud;
        public Random random = new Random();

        public MoralBarHud()
        {
            LoadConfig();
            float warningLevel = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.WARNING_LEVEL, Helpers.MoralBarConfig.WARNING_LEVEL_DEFAULT));
            hud = new HUD(warningLevel);

            Tick += OnTick;
        }

        protected void LoadConfig()
        {
            Config = Helpers.GetConfig(Helpers.MoralBarConfig.CONFIG_FILE_NAME);
            EventHandlers["moralbar:moralResult"] += new Action<dynamic>((dynamic res) =>
            {
                MoralLevel = res;
            });
        }

        public void ConsumeMoral(Ped playerPed)
        {
            if (MoralLevel > 0f)
            {
                float moralFactor = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.FACTOR, Helpers.MoralBarConfig.FACTOR_DEFAULT));
                float globalMultiplier = Helpers.StrToFloat(Config.Get(Helpers.GenericConfig.GLOBAL_MULTIPLIER, Helpers.GenericConfig.GLOBAL_MULTIPLIER_DEFAULT));
                MoralLevel -= moralFactor * globalMultiplier;
                return;
            }
            if (Convert.ToBoolean(Config.Get(Helpers.MoralBarConfig.DEATH, Helpers.MoralBarConfig.DEATH_DEFAULT)))
            {
                currentHealth += 0.01f;
                if (currentHealth > 1f)
                {
                    if (playerPed.Health > 51)
                    {
                        playerPed.Health = playerPed.Health - 1;
                        currentHealth = 0f;
                        return;
                    }
                    playerPed.Health = -1;
                }
            }
        }

        public void RenderUI(Ped playerPed)
        {
            hud.RenderBar(_moralLevel);
        }

        public async Task OnTick()
        {
            _tick++;

            if ((_tick % 320 == 0) && !MoralInitialized)
            {
                TriggerServerEvent("moralbar:getMoral");
                MoralInitialized = true;
            }

            hud.ReloadScaleformMovie();
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


