using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace MoralBar
{
    public class MoralBarHud : BaseScript
    {
        //private bool isHavingFun;
        protected float moralLevel = 100f;
        protected Config Config
        {
            get;
            set;
        }
        public float currentHealth;
        public float MoralLevel
        {
            get
            {
                return moralLevel;
            }

            set
            {
                moralLevel = value;
            }
        }

        public HUD hud;
        public Random random = new Random();

        public MoralBarHud()
        {
            LoadConfig();
            Tick += OnTick;
            float warningLevel = Helpers.StrToFloat(Config.Get(Helpers.MoralBarConfig.WARNING_LEVEL, Helpers.MoralBarConfig.WARNING_LEVEL_DEFAULT));
            hud = new HUD(warningLevel);
        }

        protected void LoadConfig()
        {
            Config = Helpers.GetConfig(Helpers.MoralBarConfig.CONFIG_FILE_NAME);
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
            hud.RenderBar(moralLevel);
        }

        public async Task OnTick()
        {
            hud.ReloadScaleformMovie();
            Ped playerPed = Game.PlayerPed;
            ConsumeMoral(playerPed);
            RenderUI(playerPed);
            await Task.FromResult(0);
        }
    }
}


