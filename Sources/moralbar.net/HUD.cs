using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Drawing;
using TinyTween;

namespace MoralBar
{
    public class HUD
    {
        private float _warningLevel;
        private Config _hudConfig;
        private Rectangle _moralBarBackdrop;
        private Rectangle _moralBarBack;
        private Rectangle _moralBar;

        protected Scaleform buttons = new Scaleform(Helpers.GenericConfig.INSTRUCTIONAL_BUTTONS);
      
        protected Color moralBarColourNormal;
        protected Color moralBarColourWarning;
        protected Tween<float> moralBarColorTween = new FloatTween();
        protected bool moralBarAnimationDir = true;
        protected PointF basePosition;

        public float MoralBarPositionX
        {
            get
            {
                return Helpers.StrToFloat(HUDConfig.Get(Helpers.HUDConfig.POSITIONX, "0"));
            }
        }

        public float MoralBarPositionY
        {
            get
            {
                return Helpers.StrToFloat(HUDConfig.Get(Helpers.HUDConfig.POSITIONY, "0"));
            }
        }

        public float MoralBarWidth
        {
            get
            {
                return Helpers.StrToFloat(HUDConfig.Get(Helpers.HUDConfig.WIDTH, Helpers.HUDConfig.WIDTH_DEFAULT));
            }
        }

        public float MoralBarHeight
        {
            get
            {
                return Helpers.StrToFloat(HUDConfig.Get(Helpers.HUDConfig.HEIGHT, Helpers.HUDConfig.HEIGHT_DEFAULT));
            }
        }

        public PointF Position
        {
            set
            {
                _moralBarBackdrop.Position = value;
                _moralBarBack.Position = new PointF(value.X, value.Y + 3f);
                _moralBar.Position = _moralBarBack.Position;
            }
        }

        public float WarningLevel
        {
            get
            {
                return _warningLevel;
            }
            set
            {
                _warningLevel = value;
            }
        }

        public Config HUDConfig
        {
            get
            {
                return _hudConfig;
            }
            set
            {
                _hudConfig = value;
            }
        }

        private void LoadHUDConfig()
        {
            HUDConfig = Helpers.GetConfig(Helpers.HUDConfig.CONFIG_FILE_NAME);
        }

        public HUD(float warningLevel)
        {
            LoadHUDConfig();

            basePosition = new PointF(MoralBarPositionX, MoralBarPositionY);
            WarningLevel = warningLevel;

            PointF moralBarBackdropPosition = basePosition;
            float offset = Helpers.StrToFloat(HUDConfig.Get(Helpers.HUDConfig.OFFSET, Helpers.HUDConfig.OFFSET_DEFAULT));

            PointF moralBarBackPosition = new PointF(moralBarBackdropPosition.X, moralBarBackdropPosition.Y + offset);
            PointF moralBarPosition = moralBarBackPosition;

            SizeF moralBarBackdropSize = new SizeF(MoralBarWidth, MoralBarHeight * 2);
            SizeF moralBarBackSize = new SizeF(MoralBarWidth, MoralBarHeight);
            SizeF moralBarSize = moralBarBackSize;

            Color moralBarBackdropColour = Color.FromArgb(100, 0, 0, 0);
            Color moralBarBackColour = GetNormalColour(50);

            moralBarColourNormal = GetNormalColour(150);
            moralBarColourWarning = GetWarningColour(255);

            _moralBarBackdrop = new Rectangle(moralBarBackdropPosition, moralBarBackdropSize, moralBarBackdropColour);
            _moralBarBack = new Rectangle(moralBarBackPosition, moralBarBackSize, moralBarBackColour);
            _moralBar = new Rectangle(moralBarPosition, moralBarSize, moralBarColourNormal);
        }

        private Color GetNormalColour(int alpha)
        {
            int red = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.NORMAL_COLOR_RED, "0"));
            int green = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.NORMAL_COLOR_GREEN, "0"));
            int blue = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.NORMAL_COLOR_BLUE, "0"));
            return Color.FromArgb(alpha, red, green, blue);
        }

        private Color GetWarningColour(int alpha)
        {
            int red = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.WARNING_COLOR_RED, "0"));
            int green = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.WARNING_COLOR_GREEN, "0"));
            int blue = Convert.ToInt32(HUDConfig.Get(Helpers.HUDConfig.WARNING_COLOR_BLUE, "0"));
            return Color.FromArgb(alpha, red, green, blue);
        }

        public void ReloadScaleformMovie()
        {
            buttons = new Scaleform(Helpers.GenericConfig.INSTRUCTIONAL_BUTTONS);
        }

        public void RenderBar(float moralLevel)
        {
            float moralBarPercentage = moralLevel;
            PointF safezoneBounds = HUD.GetSafezoneBounds();

            Position = new PointF(basePosition.X + safezoneBounds.X, basePosition.Y - safezoneBounds.Y);

            _moralBar.Size = new SizeF(MoralBarWidth / 100f * moralBarPercentage, MoralBarHeight);

            if (moralLevel < WarningLevel)
            {
                if (moralBarColorTween.State == TweenState.Stopped)
                {
                    moralBarAnimationDir = !moralBarAnimationDir;

                    moralBarColorTween.Start(
                      moralBarAnimationDir ? 100f : 255f,
                      moralBarAnimationDir ? 255f : 100f,
                      .5f,
                      ScaleFuncs.QuarticEaseOut
                    );
                }

                moralBarColorTween.Update(Game.LastFrameTime);

                _moralBar.Color = Color.FromArgb((int)Math.Floor(moralBarColorTween.CurrentValue), moralBarColourWarning);
            }
            else
            {
                _moralBar.Color = moralBarColourNormal;

                if (moralBarColorTween.State != TweenState.Stopped)
                {
                    moralBarColorTween.Stop(StopBehavior.ForceComplete);
                }
            }

            _moralBarBackdrop.Draw();
            _moralBarBack.Draw();
            _moralBar.Draw();
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
