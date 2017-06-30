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
            Tick += OnTick;
            hud = new HUD();
            LoadConfig();
        }

        protected void LoadConfig()
        {
            string configContent = null;
            try
            {
                configContent = Function.Call<string>(Hash.LOAD_RESOURCE_FILE, "moralbar", "config.ini");
            }
            catch (Exception)
            {
            }
            Config = new Config(configContent);
        }

        public void ConsumeMoral(Ped playerPed)
        {
            if (MoralLevel > 0f)
            {
                MoralLevel -= StrToFloat(Config.Get("MoralFactor", "0.50030476")) * StrToFloat(Config.Get("GlobalMultiplier", "1.5"));
                return;
            }
            if (Convert.ToBoolean(Config.Get("DeathFood", "true")))
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

        private float StrToFloat(string convStr)
        {
            return float.Parse(convStr, CultureInfo.InvariantCulture.NumberFormat);
        }

    }
}


