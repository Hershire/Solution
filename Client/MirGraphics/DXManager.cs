using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Client.MirGraphics
{
    static class DXManager
    {
        public static bool DeviceLost { get; private set; }
        public static bool Blending { get; private set; }
        public static float Opacity { get; private set; }

        public static PresentParameters Parameters { get; private set; }

        public static Sprite Sprite { get; private set; }
        public static Line Line { get; private set; }
        public static Device Device { get; private set; }

        private static CreateFlags DevFlags;
        private static DeviceType DevType;

        public static Surface CurrentSurface { get; private set; }
        public static List<Texture> Lights { get; private set; }
        public static Texture RadarTexture { get; private set; }

        static DXManager()
        {
            Opacity = 1F;
            Lights = new List<Texture>();
        }

        public static void Create()
        {
            Parameters = new PresentParameters
            {
                BackBufferFormat = Format.X8R8G8B8,
                PresentFlag = PresentFlag.LockableBackBuffer,
                BackBufferWidth = Settings.ScreenSize.Width,
                BackBufferHeight = Settings.ScreenSize.Height,
                SwapEffect = SwapEffect.Discard,
                PresentationInterval = PresentInterval.Immediate,
                Windowed = !Settings.FullScreen
            };

            Caps DevCaps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
            DevType = DeviceType.Reference;
            DevFlags = CreateFlags.SoftwareVertexProcessing;
            if (DevCaps.VertexShaderVersion >= new Version(2, 0) && DevCaps.PixelShaderVersion >= new Version(2, 0))
            {
                DevType = DeviceType.Hardware;
                if (DevCaps.DeviceCaps.SupportsHardwareTransformAndLight)
                {
                    DevFlags = CreateFlags.HardwareVertexProcessing;
                    if (DevCaps.DeviceCaps.SupportsPureDevice)
                        DevFlags |= CreateFlags.PureDevice;
                }
            }

            Device = new Device(0, DevType, Main.This, DevFlags, Parameters);


            Device.DeviceLost += D_DeviceLost;
            Device.DeviceResizing += D_DeviceResizing;
            Device.DeviceReset += D_DeviceReset;

            Device.SetDialogBoxesEnabled(true);

            LoadTextures();

        }
        private static void LoadTextures()
        {
            Sprite = new Sprite(Device);
            Line = new Line(Device) { Width = 1F };

            RadarTexture = new Texture(Device, 2, 2, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            Device.SetRenderTarget(0, RadarTexture.GetSurfaceLevel(0));
            Device.Clear(ClearFlags.Target, Color.White, 0, 0);

            CreateLights();

            SetSurface(Device.GetBackBuffer(0, 0, BackBufferType.Mono));
        }
        private static void CreateLights()
        {
            //Optimize?
            Lights = new List<Texture>();
            for (int I = 1; I < 15; I++)
            {
                using (MemoryStream MStream = new MemoryStream())
                {
                    using (Bitmap B = new Bitmap(65 * I, 50 * I, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                    {
                        using (System.Drawing.Graphics G = System.Drawing.Graphics.FromImage(B))
                        {
                            using (System.Drawing.Drawing2D.GraphicsPath P = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                P.AddEllipse(new Rectangle(0, 0, 65 * I, 50 * I));
                                using (System.Drawing.Drawing2D.PathGradientBrush Br = new System.Drawing.Drawing2D.PathGradientBrush(P))
                                {
                                    G.Clear(Color.Black);
                                    G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                    Br.SurroundColors = new Color[] { Color.Black };
                                    Br.CenterColor = Color.Gray;
                                    G.FillPath(Br, P);
                                    Br.CenterColor = Color.White;
                                    Br.FocusScales = new PointF(0.2f, 0.2f);
                                    G.TranslateTransform(0, 0);
                                    G.FillPath(Br, P);
                                    G.Save();
                                    B.Save(MStream, System.Drawing.Imaging.ImageFormat.Bmp);
                                    MStream.Position = 0;
                                    Lights.Add(TextureLoader.FromStream(Device, MStream, 65 * I, 50 * I, 1, Usage.RenderTarget, Format.A8B8G8R8, Pool.Default, Filter.None, Filter.None, 0));
                                }
                            }
                        }
                    }
            }
            }
        }

        public static void SetSurface(Surface S)
        {
            if (CurrentSurface == S) return;

            CurrentSurface = S;
            Device.SetRenderTarget(0, S);
        }
        public static void SetOpacity(float opacity)
        {
            if (Opacity == opacity) return;

            Sprite.Flush();
            Device.RenderState.AlphaBlendEnable = true;
            if (opacity >= 1 || opacity < 0)
            {
                Device.RenderState.SourceBlend = Blend.SourceAlpha;
                Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                Device.RenderState.AlphaSourceBlend = Blend.One;
                Device.RenderState.BlendFactor = Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                Device.RenderState.SourceBlend = Blend.BlendFactor;
                Device.RenderState.DestinationBlend = Blend.InvBlendFactor;
                Device.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
                Device.RenderState.BlendFactor = Color.FromArgb((byte)(255 * opacity), (byte)(255 * opacity),
                                                                (byte)(255 * opacity), (byte)(255 * opacity));
            }
            Opacity = opacity;
            Sprite.Flush();
        }
        public static void SetBlend(bool Value)
        {
            if (Value == Blending) return;
            Blending = Value;

            Sprite.End();
            if (Blending)
            {
                Sprite.Begin(SpriteFlags.DoNotSaveState);
                Device.RenderState.AlphaBlendEnable = true;
                Device.RenderState.SourceBlend = Blend.One;
                Device.RenderState.DestinationBlend = Blend.One;
            }
            else
                Sprite.Begin(SpriteFlags.AlphaBlend);
        }

        public static void AttemptReset()
        {
            try
            {
                int Result;
                Device.CheckCooperativeLevel(out Result);
                switch ((ResultCode)Result)
                {
                    case ResultCode.DeviceNotReset:
                        Device.Reset(Parameters);
                        break;
                    case ResultCode.DeviceLost:
                        break;
                    case ResultCode.Success:
                        DeviceLost = false;
                        CurrentSurface = Device.GetBackBuffer(0, 0, BackBufferType.Mono);
                        Device.SetRenderTarget(0, CurrentSurface);
                        break;
                }
            }
            catch
            {
            }
        }
        public static void AttemptRecovery()
        {
            try
            {
                Sprite.End();
            }
            catch
            {
            }

            try
            {
                Device.EndScene();
            }
            catch
            {
            }
            try

            {
                SetSurface(Device.GetBackBuffer(0, 0, BackBufferType.Mono));
            }
            catch
            {
            }

        }

        private static void D_DeviceReset(object sender, EventArgs e)
        {
            LoadTextures();
        }
        private static void D_DeviceResizing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
        private static void D_DeviceLost(object sender, EventArgs e)
        {
            DeviceLost = true;
        }
    }
}
