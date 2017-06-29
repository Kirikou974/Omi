using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Drawing;
using TinyTween;

namespace Moralhud
{
    public class HUD
    {
        protected Scaleform buttons = new Scaleform("instructional_buttons");
        protected float moralBarWidth = 180f;
        protected float moraBarHeight = 6f;

        protected Color moralBarColourNormal;
        protected Color moralBarColourWarning;

        protected Rectangle moralBarBackdrop;
        protected Rectangle moralBarBack;
        protected Rectangle moralBar;
        
        protected Tween<float> moralBarColorTween = new FloatTween();
        protected bool moralBarAnimationDir = true;
        protected PointF basePosition = new PointF(0f, 539f);

        public PointF Position
        {
            set
            {
                moralBarBackdrop.Position = value;
                moralBarBack.Position = new PointF(value.X, value.Y + 3f);
                moralBar.Position = moralBarBack.Position;
            }
        }

        public HUD()
        {
            PointF moralBarBackdropPosition = basePosition;
            PointF moralBarBackPosition = new PointF(moralBarBackdropPosition.X, moralBarBackdropPosition.Y + 3f);
            PointF moralBarPosition = moralBarBackPosition;

            SizeF moralBarBackdropSize = new SizeF(moralBarWidth, 12f);
            SizeF moralBarBackSize = new SizeF(moralBarWidth, moraBarHeight);
            SizeF moralBarSize = moralBarBackSize;

            Color moralBarBackdropColour = Color.FromArgb(100, 0, 0, 0);
            Color moralBarBackColour = Color.FromArgb(50, 220, 162, 206);

            moralBarColourNormal = Color.FromArgb(150, 220, 162, 206);
            //moralBarColourWarning = Color.FromArgb(255, 255, 245, 220);

            moralBarBackdrop = new Rectangle(moralBarBackdropPosition, moralBarBackdropSize, moralBarBackdropColour);
            moralBarBack = new Rectangle(moralBarBackPosition, moralBarBackSize, moralBarBackColour);
            moralBar = new Rectangle(moralBarPosition, moralBarSize, moralBarColourNormal);
        }

        
        public void ReloadScaleformMovie()
        {
            buttons = new Scaleform("instructional_buttons");
        }

        public void RenderBar(float moralLevel)
        {
            float moralBarPercentage = (100f / 100f) * moralLevel;
            PointF safezoneBounds = HUD.GetSafezoneBounds();

            Position = new PointF(basePosition.X + safezoneBounds.X, basePosition.Y - safezoneBounds.Y);

            moralBar.Size = new SizeF(moralBarWidth / 100f * moralBarPercentage, moraBarHeight);

            moralBarBackdrop.Draw();
            moralBarBack.Draw();
            moralBar.Draw();
        }

        public static PointF GetSafezoneBounds()
        {
            float t = Function.Call<float>(Hash.GET_SAFE_ZONE_SIZE);

            return new PointF(
              (int)Math.Round((1280 - (1280 * t)) / 2),
              (int)Math.Round((720 - (720 * t)) / 2)
            );
        }

        public void RenderInstructions()
        {
            buttons.Render2D();
        }
    }
}
