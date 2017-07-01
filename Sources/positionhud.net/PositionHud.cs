using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionHud
{
    public class PositionHud : BaseScript
    {

        public PositionHud()
        {
            Tick += OnTick;
        }

        public async Task OnTick()
        {
            Ped playerPed = Game.PlayerPed;
            
            if (Game.IsControlJustReleased(0, Control.ScriptedFlyZUp))
            {
                string position = string.Format("Your position is : X={0}, Y={1}, Z={2}", playerPed.Position.X.ToString(), playerPed.Position.Y.ToString(), playerPed.Position.Z.ToString());
                Screen.ShowNotification(position);
            }
            await Task.FromResult(0);
        }
    }
}
