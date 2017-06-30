using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;

namespace Moralhud
{
    public class Moralhud : BaseScript
    {

        //private bool isHavingFun;
        protected float moralLevel = 100f;
        public HUD hud;
        public Random random = new Random();

        public Moralhud()
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


