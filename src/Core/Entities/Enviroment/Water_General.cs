#if VIRTICES_3D
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
////using Virtex.Lib.Vertices.Physics.BEPU.Collidables.MobileCollidables;
////using Virtex.Lib.Vertices.Physics.BEPU.Collidables;
using Virtex.Lib.Vertices.Physics.BEPU.DataStructures;



namespace Virtex.Lib.Vertices.Core.Entities
{
    public class Water_General : vxEntity3D
    {
        public struct VertexMultitextured
        {
            //
            //Water Graphic Stuff
            //
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Vector3 Tangent;
            public Vector3 BiNormal;


            public static int SizeInBytes = (3 + 3 + 2 + 3 + 3) * 4;
            public static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                 new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                 new VertexElement(sizeof(float) * 11, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0)
             );

        }

        private VertexBuffer vb;
        private IndexBuffer ib;
        VertexMultitextured[] myVertices;
        private int height = 128;
        private int width = 128;
        public float depth = 10;

        //
        //Physics Code
        //
        //Matrix World;

        private Vector3 myPosition;
        private Vector3 myScale;
        private Quaternion myRotation;

        Effect effect;

        private Vector3 basePosition;

        #region Properties

        public Vector3 Position
        {
            get { return basePosition; }
            set { basePosition = value; }
        }

        private string EnvAsset;

        float bumpHeight = 0.5f;
        Vector2 textureScale = new Vector2(4, 4);
        Vector2 bumpSpeed = new Vector2(0, -0.02f);
        float fresnelBias = 0.25f;
        float fresnelPower = 1.0f;
        float hdrMultiplier = 1.0f;
        Color deepWaterColor = Color.SkyBlue;
        Color shallowWaterColor = Color.SkyBlue;
        Color reflectionColor = Color.White;
        float reflectionAmount = 1.5f;
        float waterAmount = 0.75f;
        float waveAmplitude = 0.5f;
        float waveFrequency = 0.1f;

        /// <summary>
        /// Height of water bump texture.
        /// Min 0.0 Max 2.0 Default = .5
        /// </summary>
        public float BumpHeight
        {
            get { return bumpHeight; }
            set { bumpHeight = value; }
        }
        /// <summary>
        /// Scale of bump texture.
        /// </summary>
        public Vector2 TextureScale
        {
            get { return textureScale; }
            set { textureScale = value; }
        }
        /// <summary>
        /// Velocity of water flow
        /// </summary>
        public Vector2 BumpSpeed
        {
            get { return bumpSpeed; }
            set { bumpSpeed = value; }
        }
        /// <summary>
        /// Min 0.0 Max 1.0 Default = .025
        /// </summary>
        public float FresnelBias
        {
            get { return fresnelBias; }
            set { fresnelBias = value; }
        }
        /// <summary>
        /// Min 0.0 Max 10.0 Default = 1.0;
        /// </summary>
        public float FresnelPower
        {
            get { return FresnelPower; }
            set { fresnelPower = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 100 Default = 1.0
        /// </summary>
        public float HDRMultiplier
        {
            get { return hdrMultiplier; }
            set { hdrMultiplier = value; }
        }
        /// <summary>
        /// Color of deep water Default = Black;
        /// </summary>
        public Color DeepWaterColor
        {
            get { return deepWaterColor; }
            set { deepWaterColor = value; }
        }
        /// <summary>
        /// Color of shallow water Default = SkyBlue
        /// </summary>
        public Color ShallowWaterColor
        {
            get { return shallowWaterColor; }
            set { shallowWaterColor = value; }
        }
        /// <summary>
        /// Default = White
        /// </summary>
        public Color ReflectionColor
        {
            get { return reflectionColor; }
            set { reflectionColor = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 2.0 Default = .5
        /// </summary>
        public float ReflectionAmount
        {
            get { return reflectionAmount; }
            set { reflectionAmount = value; }
        }
        /// <summary>
        /// Amount of water color to use.
        /// Min = 0 Max = 2 Default = 0;
        /// </summary>
        public float WaterAmount
        {
            get { return waterAmount; }
            set { waterAmount = value; }
        }
        /// <summary>
        /// Min = 0.0 Max = 10 Defatult = 0.5
        /// </summary>
        public float WaveAmplitude
        {
            get { return waveAmplitude; }
            set { waveAmplitude = value; }
        }
        /// <summary>
        /// Min = 0 Max = 1 Default .1
        /// </summary>
        public float WaveFrequency
        {
            get { return waveFrequency; }
            set { waveFrequency = value; }
        }

        /// <summary>
        /// Default 128
        /// </summary>
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        /// <summary>
        /// Default 128
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        #endregion

        public Water_General(vxEngine vxEngine, Vector3 inPosition, Vector3 inSize) 
            : base (vxEngine, vxEngine.Model_Sandbox_WorkingPlane, inPosition)
        {

            width = (int)inSize.Z;
            height = (int)inSize.X;
            depth = inSize.Y;
            float buffer = 0.91f;

            Position = inPosition;
            basePosition = inPosition;
            
            myScale = Vector3.One;
            myRotation = new Quaternion(0, 0, 0, 1);

            EnvAsset = "test";

            loadContent();
        }

        public void SetDefault()
        {
            bumpHeight = 0.75f;
            textureScale = new Vector2(4, 4);
            bumpSpeed = new Vector2(0, 0.01f);
            fresnelBias = .025f;
            fresnelPower = 1.0f;
            hdrMultiplier = 4;
            deepWaterColor = Color.DarkTurquoise;
            shallowWaterColor = Color.Turquoise;
            reflectionColor = Color.LightBlue;
            reflectionAmount = 0.25f;
            waterAmount = 0f;
            waveAmplitude = 0.5f;
            waveFrequency = 0.1f;
        }
        public void loadContent()
        {

            SetDefault();
            
            effect.Parameters["tEnvMap"].SetValue(vxEngine.Assets.Textures.Texture_WaterEnvr);
            effect.Parameters["tNormalMap"].SetValue(vxEngine.Assets.Textures.Texture_WaterWaves);

            myPosition = new Vector3(basePosition.X, basePosition.Y, basePosition.Z);

            // Vertices
            myVertices = new VertexMultitextured[width * height];
            int Scale = 100;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    myVertices[x + y * width].Position = myPosition + new Vector3(y * Scale, 0, x * Scale);
                    myVertices[x + y * width].Normal = new Vector3(0, -1, 0);
                    myVertices[x + y * width].TextureCoordinate.X = (float)x / 30.0f;
                    myVertices[x + y * width].TextureCoordinate.Y = (float)y / 30.0f;
                }

            // Calc Tangent and Bi Normals.
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    // Tangent Data.
                    if (x != 0 && x < width - 1)
                        myVertices[x + y * width].Tangent = myVertices[x - 1 + y * width].Position - myVertices[x + 1 + y * width].Position;
                    else
                        if (x == 0)
                            myVertices[x + y * width].Tangent = myVertices[x + y * width].Position - myVertices[x + 1 + y * width].Position;
                        else
                            myVertices[x + y * width].Tangent = myVertices[x - 1 + y * width].Position - myVertices[x + y * width].Position;

                    // Bi Normal Data.
                    if (y != 0 && y < height - 1)
                        myVertices[x + y * width].BiNormal = myVertices[x + (y - 1) * width].Position - myVertices[x + (y + 1) * width].Position;
                    else
                        if (y == 0)
                            myVertices[x + y * width].BiNormal = myVertices[x + y * width].Position - myVertices[x + (y + 1) * width].Position;
                        else
                            myVertices[x + y * width].BiNormal = myVertices[x + (y - 1) * width].Position - myVertices[x + y * width].Position;
                }


            vb = new VertexBuffer(vxEngine.Game.GraphicsDevice, VertexMultitextured.VertexDeclaration, VertexMultitextured.SizeInBytes * width * height, BufferUsage.WriteOnly);
            vb.SetData(myVertices);

            short[] terrainIndices = new short[(width - 1) * (height - 1) * 6];
            for (short x = 0; x < width - 1; x++)
            {
                for (short y = 0; y < height - 1; y++)
                {
                    terrainIndices[(x + y * (width - 1)) * 6] = (short)((x + 1) + (y + 1) * width);
                    terrainIndices[(x + y * (width - 1)) * 6 + 1] = (short)((x + 1) + y * width);
                    terrainIndices[(x + y * (width - 1)) * 6 + 2] = (short)(x + y * width);

                    terrainIndices[(x + y * (width - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * width);
                    terrainIndices[(x + y * (width - 1)) * 6 + 4] = (short)(x + y * width);
                    terrainIndices[(x + y * (width - 1)) * 6 + 5] = (short)(x + (y + 1) * width);
                }
            }

            ib = new IndexBuffer(vxEngine.Game.GraphicsDevice, typeof(short), (width - 1) * (height - 1) * 6, BufferUsage.WriteOnly);
            ib.SetData(terrainIndices);
        }

        public override void RenderMeshPrepPass()
        {
            World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(myPosition);


            Matrix WVP = World * Current3DScene.Camera.View * Current3DScene.Camera.Projection;
            Matrix WV = World * Current3DScene.Camera.View;
            Matrix viewI = Matrix.Invert(Current3DScene.Camera.View);

            effect.Parameters["matWorldViewProj"].SetValue(WVP);
            effect.Parameters["matWorld"].SetValue(World);
            effect.Parameters["matWorldView"].SetValue(WV);
            effect.Parameters["matViewI"].SetValue(viewI);

            effect.Parameters["fBumpHeight"].SetValue(0);
            effect.Parameters["vTextureScale"].SetValue(Vector2.Zero);
            effect.Parameters["vBumpSpeed"].SetValue(Vector2.Zero);
            effect.Parameters["fFresnelBias"].SetValue(0);
            effect.Parameters["fFresnelPower"].SetValue(0);
            effect.Parameters["fHDRMultiplier"].SetValue(0);
            effect.Parameters["vDeepColor"].SetValue(Vector4.Zero);
            effect.Parameters["vShallowColor"].SetValue(Vector4.Zero);
            effect.Parameters["vReflectionColor"].SetValue(Vector4.Zero);
            effect.Parameters["fReflectionAmount"].SetValue(0);
            effect.Parameters["fWaterAmount"].SetValue(0);
            effect.Parameters["fWaveAmp"].SetValue(0);
            effect.Parameters["fWaveFreq"].SetValue(0);

            vxEngine.Game.GraphicsDevice.SetVertexBuffer(vb);
            vxEngine.Game.GraphicsDevice.Indices = ib;

            for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
            {
                effect.CurrentTechnique.Passes[0].Apply();
                vxEngine.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, width * height, 0, (width - 1) * (height - 1) * 2);

            }
        }

        public override void RenderMesh(string RenderTechnique)
        {
            World = Matrix.CreateScale(myScale) *
                            Matrix.CreateFromQuaternion(myRotation) *
                            Matrix.CreateTranslation(myPosition);


            Matrix WVP = World * Current3DScene.Camera.View * Current3DScene.Camera.Projection;
            Matrix WV = World * Current3DScene.Camera.View;
            Matrix viewI = Matrix.Invert(Current3DScene.Camera.View);

                effect.Parameters["matWorldViewProj"].SetValue(WVP);
                effect.Parameters["matWorld"].SetValue(World);
                effect.Parameters["matWorldView"].SetValue(WV);
                effect.Parameters["matViewI"].SetValue(viewI);

                effect.Parameters["fBumpHeight"].SetValue(bumpHeight);
                effect.Parameters["vTextureScale"].SetValue(textureScale);
                effect.Parameters["vBumpSpeed"].SetValue(bumpSpeed);
                effect.Parameters["fFresnelBias"].SetValue(fresnelBias);
                effect.Parameters["fFresnelPower"].SetValue(fresnelPower);
                effect.Parameters["fHDRMultiplier"].SetValue(hdrMultiplier);
                effect.Parameters["vDeepColor"].SetValue(deepWaterColor.ToVector4());
                effect.Parameters["vShallowColor"].SetValue(shallowWaterColor.ToVector4());
                effect.Parameters["vReflectionColor"].SetValue(reflectionColor.ToVector4());
                effect.Parameters["fReflectionAmount"].SetValue(reflectionAmount);
                effect.Parameters["fWaterAmount"].SetValue(waterAmount);
                effect.Parameters["fWaveAmp"].SetValue(waveAmplitude);
                effect.Parameters["fWaveFreq"].SetValue(waveFrequency);
            

                vxEngine.Game.GraphicsDevice.SetVertexBuffer(vb);
                vxEngine.Game.GraphicsDevice.Indices = ib;

                for (int p = 0; p < effect.CurrentTechnique.Passes.Count; p++)
                {
                    effect.CurrentTechnique.Passes[0].Apply();
                    vxEngine.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, width * height, 0, (width - 1) * (height - 1) * 2);

                }
            
        }


        public override void Update(GameTime gameTime)
        {

            effect.Parameters["fTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        }
    }
}
#endif