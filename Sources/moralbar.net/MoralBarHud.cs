using CitizenFX.Core;
using System;
using System.Threading.Tasks;

namespace MoralBar
{
    public class MoralBarHud : BaseScript
    {

        //private bool isHavingFun;
        protected float moralLevel = 100f;
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
        }

        public void RenderUI(Ped playerPed)
        {
            hud.RenderBar(moralLevel);
        }

        public async Task OnTick()
        {
            hud.ReloadScaleformMovie();
            Ped playerPed = Game.PlayerPed;
            RenderUI(playerPed);
            await Task.FromResult(0);
        }
    }
}


