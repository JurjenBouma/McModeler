using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.Direct2D;
using SlimDX.Windows;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;
using MCModeler.Editors;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using FactorD2D = SlimDX.Direct2D.Factory;
using FactoryDXGI = SlimDX.DXGI.Factory;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MCModeler
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexTex
    {
        public Vector3 Position;
        public Vector2 UV;
        public VertexTex(Vector3 pos, Vector2 uV)
        { Position = pos; UV = uV; }
        public const int Stride = 20;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexCol
    {
        public Vector3 Position;
        public Color4 Color;
        public VertexCol(Vector3 pos, Color4 color)
        { Position = pos; Color = color; }
        public const int Stride = 28;
    }
    public static class Renderer
    {
        public delegate void UpdateLoop();

        public static Device device;
        public static DeviceContext context;
        public static Camera camera;

        static SwapChain swapChain;
        static Viewport viewport;
        static RenderTargetView renderTarget;
        static DepthStencilView depthStencilView;
        static Texture2D depthStencilBuffer;
        static RasterizerState backCullState;
        static BlendState transBlendState;
        static InputLayout inputLayoutTextured;
        static InputLayout inputLayoutColor;

        static  Matrix viewMatrix;
        static  Matrix worldMatrix;
        static  Matrix projMatrix;
        public static float FOV = (float)Math.PI * 0.5f;  

        static EffectMatrixVariable WVP;
        static Effect fx;
        static EffectTechnique techniqueTextured;
        static EffectTechnique techniqueSelected;
        static EffectTechnique techniqueColor;
        static EffectResourceVariable fxTex;

        static Model ActiveModel;
        static CoordinateGrid grid;
        public static Editor ActiveEditor;

        public static Matrix WorldMatrix { get { return worldMatrix; } }
        public static Matrix ViewMatrix { get { return viewMatrix; } }
        public static Matrix ProjectionMatrix { get { return projMatrix; } }
        public static Viewport ViewPort { get { return viewport; } }
        public static EffectTechnique TexturedTechnique { get { return techniqueTextured; } }
        public static EffectTechnique ColoredTechnique { get { return techniqueColor; } }
        public static EffectTechnique SelectedTechnique { get { return techniqueSelected; } }

        public static UpdateLoop Update;

        public static void Initialize3D(IntPtr handle,int viewPortWidth,int viewPortHeight)
        {
            #region Set Swapchain and Viewport
            var swapDiscription = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = handle,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapDiscription, out device, out swapChain);
            swapChain.IsFullScreen = false;
            context = device.ImmediateContext;
            using (var factory = swapChain.GetParent<FactoryDXGI>())
                factory.SetWindowAssociation(handle, WindowAssociationFlags.IgnoreAltEnter);
            viewport = new Viewport(0, 0, viewPortWidth, viewPortHeight);
            context.Rasterizer.SetViewports(viewport);
            #endregion
            #region Set RenderTarget and DepthStencil
            SetTargetAndDepthSView(viewPortWidth, viewPortHeight);
            #endregion
            #region Set Blend&RasterizerStates
            RasterizerStateDescription backCull = new RasterizerStateDescription()
            {
                FillMode = SlimDX.Direct3D11.FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterclockwise = true,
                IsDepthClipEnabled = true
            };

            backCullState = RasterizerState.FromDescription(device, backCull);

            BlendStateDescription transDesc = new BlendStateDescription()
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            transDesc.RenderTargets[0].BlendEnable = true;
            transDesc.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            transDesc.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            transDesc.RenderTargets[0].BlendOperation = BlendOperation.Add;
            transDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            transDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
            transDesc.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            transDesc.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            transBlendState = BlendState.FromDescription(device, transDesc);

            context.Rasterizer.State = backCullState;
            context.OutputMerger.BlendState = transBlendState;
            #endregion

            LoadShaders();
            LoadInputLayouts();

            worldMatrix = Matrix.Translation(-8, 0,-8);
            projMatrix = Matrix.PerspectiveFovRH(FOV, (float)viewPortWidth / viewPortHeight, 0.01f, 1000.0f);
            SetupCamera();
            grid = new CoordinateGrid(new Color4(0.95f,0.95f,0.95f));
        }
        private static void SetTargetAndDepthSView(int viewPortWidth, int viewPortHeight)
        {
            using (var resource = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0))
                renderTarget = new RenderTargetView(device, resource);
            var depthStencilDesc = new Texture2DDescription()
            {
                Width = viewPortWidth,
                Height = viewPortHeight,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            depthStencilBuffer = new Texture2D(device, depthStencilDesc);
            depthStencilView = new DepthStencilView(device, depthStencilBuffer);
            context.OutputMerger.SetTargets(depthStencilView, renderTarget);
        }
        public static void SetInputLayout(string layoutName)
        {
            if (layoutName == "Textured")
                context.InputAssembler.InputLayout = inputLayoutTextured;
            else if (layoutName == "Color")
                context.InputAssembler.InputLayout = inputLayoutColor;
        }
        private static void SetupCamera()
        {
            var pos = new Vector3(0,0,32);
            var target = new Vector3(0, 0, 0);
            var up = new Vector3(0, 1, 0);

            camera = new Camera(pos, target, up);
            camera.Pitch(-1);
            viewMatrix = camera.GetViewMatrix();
        }
        private static void LoadShaders()
        {
            ShaderBytecode bytecode = null;
            bytecode = ShaderBytecode.CompileFromFile("Shaders.fx", null, "fx_5_0", ShaderFlags.None, EffectFlags.None, null, null);
            fx = new Effect(device, bytecode);
            bytecode.Dispose();
            techniqueTextured = fx.GetTechniqueByName("Textured");
            techniqueSelected = fx.GetTechniqueByName("Selected");
            techniqueColor = fx.GetTechniqueByName("Color");
            WVP = fx.GetVariableByName("WVP").AsMatrix();
            fxTex = fx.GetVariableByName("tex").AsResource();
        }
        private static void LoadInputLayouts()
        {
            var passDescTextured = techniqueTextured.GetPassByIndex(0).Description;
            var elementsTextured = new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0,InputClassification.PerVertexData,0),
                                    new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12,0,InputClassification.PerVertexData,0)};
            inputLayoutTextured = new InputLayout(device, passDescTextured.Signature, elementsTextured);

            var passDescLine = techniqueColor.GetPassByIndex(0).Description;
            var elementsLine= new[] { new InputElement("POSITION", 0, Format.R32G32B32_Float, 0,0,InputClassification.PerVertexData,0),
                                    new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12,0,InputClassification.PerVertexData,0)};
            inputLayoutColor = new InputLayout(device, passDescLine.Signature, elementsLine);
        }
        public static void SetActiveModel(Model model)
        {
            if(ActiveModel != null)
                ActiveModel.Dispose();
            ActiveModel = model;
        }
        public static Model GetActiveModel(){ return ActiveModel;}
        public static void SetTexture(ShaderResourceView texture){fxTex.SetResource(texture); }
        public static void ResetWorldMatrix() {worldMatrix = Matrix.Translation(-8, 0, -8);}
        public static void PreMultiplyWorldMatrix(Matrix multMatrix) { worldMatrix = multMatrix * worldMatrix; }
        public static void PostMultiplyWorldMatrix(Matrix multMatrix) { worldMatrix *= multMatrix;}

        public static void SetWVP(Matrix additionalMatrix)
        {
            Matrix OWVP = additionalMatrix * worldMatrix * viewMatrix * projMatrix;
            WVP.SetMatrix(OWVP);
        }

        public static void Resize(int viewPortWidth, int viewPortHeight)
        {
            if (renderTarget != null && viewPortWidth > 0 && viewPortHeight > 0)
            {
                viewport = new Viewport(0, 0, viewPortWidth, viewPortHeight);
                projMatrix = Matrix.PerspectiveFovRH(FOV, (float)viewPortWidth / viewPortHeight, 0.01f, 1000.0f);
                context.Rasterizer.SetViewports(viewport);

                renderTarget.Dispose();
                depthStencilBuffer.Dispose();
                swapChain.ResizeBuffers(2, viewPortWidth, viewPortHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);
                SetTargetAndDepthSView(viewPortWidth, viewPortHeight);
            }
        }

        public static void RenderFrame()
        {
            if (Update != null)
                Update();

            context.ClearRenderTargetView(renderTarget, new Color4(0.17f,0.47f,0.79f));
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            viewMatrix = camera.GetViewMatrix();

            SetWVP(Matrix.Identity);
            if(ActiveModel != null)
                ActiveModel.Draw();

            SetWVP(Matrix.Identity);
            grid.Draw();

            if (ActiveEditor != null)
            {
                SetWVP(Matrix.Identity);
                ActiveEditor.DrawUI();
            }
            swapChain.Present(0, PresentFlags.None);
        }

        public static void CleanUp()
        {
            device.Dispose();
            inputLayoutTextured.Dispose();
            inputLayoutColor.Dispose();
            renderTarget.Dispose();
            swapChain.Dispose();
            depthStencilView.Dispose();
            backCullState.Dispose();
            transBlendState.Dispose();
            depthStencilBuffer.Dispose();
            fx.Dispose();
            if(ActiveModel != null)
             ActiveModel.Dispose();
            grid.Dispose();
        }
    }
}
