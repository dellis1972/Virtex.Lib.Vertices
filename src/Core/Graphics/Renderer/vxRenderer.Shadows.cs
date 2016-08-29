#if VIRTICES_3D

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//Virtex vxEngine Declaration
using Virtex.Lib.Vrtc.Core;
using Virtex.Lib.Vrtc.Utilities;
using Virtex.Lib.Vrtc.Core.Cameras;
using Virtex.Lib.Vrtc.Core.Settings;
using System.IO;

namespace Virtex.Lib.Vrtc.Graphics
{

    /// <summary>
    /// A simple renderer. Contains methods for rendering standard XNA models, as well as
    /// instanced rendering (see class InstancedModel), and rendering of selected triangles. 
    /// The scene light position can be set with the property lightPosition.
    /// </summary>
    public partial class vxRenderer
    { 
        public void InitialiseRenderTargetsForShadowMaps()
        {
            RT_ShadowMap = new RenderTarget2D(mGraphicsDevice, ShadowMapSize * 2, ShadowMapSize * 2, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
            RT_BlockTexture = new RenderTarget2D(mGraphicsDevice, ShadowMapSize * 2, ShadowMapSize * 2, false, SurfaceFormat.Single, DepthFormat.Depth24);

            int tSize = 32;

            _randomTexture3D = new Texture3D(mGraphicsDevice, tSize, tSize, tSize, false, SurfaceFormat.Rg32);
            _randomTexture2D = new Texture2D(mGraphicsDevice, tSize, tSize, false, SurfaceFormat.Rg32);

            Random random = new Random();

            Func<int, IEnumerable<UInt16>> randomRotations = (count) =>
            {
                return Enumerable
                   .Range(0, count)
                    .Select(i => (float)(random.NextDouble() * Math.PI * 2))
                    .SelectMany(r => new[] { Math.Cos(r), Math.Sin(r) })
                    .Select(v => (UInt16)((v * 0.5 + 0.5) * UInt16.MaxValue));
            };
            fillTextureWithBlockPattern(RT_BlockTexture, 32);
            _randomTexture3D.SetData(randomRotations(_randomTexture3D.Width * _randomTexture3D.Height * _randomTexture3D.Depth).ToArray());
            _randomTexture2D.SetData(randomRotations(_randomTexture2D.Width * _randomTexture2D.Height).ToArray());
            //_randomTexture2D = vxEngine.Assets.Textures.RandomValues;

			/*
            Stream streampng = File.OpenWrite("tiny.png");
            _randomTexture2D.SaveAsPng(streampng, tSize, tSize);
            streampng.Dispose();
            //texture.Dispose();
			*/
        }


        /// <summary>
        /// Sets the shadow transforms.
        /// </summary>
        /// <param name="camera">Camera.</param>
        public void setShadowTransforms(vxCamera3D camera)
        {
            Matrix viewProj = camera.View * camera.Projection;
            Matrix viewProjInverse = Matrix.Invert(viewProj);
            Matrix projInverse = Matrix.Invert(camera.Projection);
            Matrix viewInverse = Matrix.Invert(camera.View);


            // figure out closest geometry empassing near and far plances based on arena bounding box
            var viewSpaceBB = vxGeometryHelper.transformBoundingBox(bbDim, camera.View);
            var viewSpaceMin = Math.Min(-1, viewSpaceBB.Max.Z);
            var viewSpaceMax = Math.Min(0, viewSpaceBB.Min.Z);

            var viewDistance = new[]
                {
                   bbDim.Max.X - bbDim.Min.X,
                   bbDim.Max.Y - bbDim.Min.Y,
                   bbDim.Max.Z - bbDim.Min.Z,
                }.Max() - 200.0f;

            var splitPlanes = vxGeometryHelper.practicalSplitScheme(NumberOfShadowSplits, 1, viewDistance)
                .Select(v => -v)
                .ToArray();

            var splitDistances = splitPlanes.Select(c =>
            {
                var d = Vector4.Transform(new Vector3(0, 0, c), camera.Projection);
                return d.W != 0 ? d.Z / d.W : 0;
            }).ToArray();

            var splitData = Enumerable.Range(0, NumberOfShadowSplits).Select(i =>
            {
                var n = splitDistances[i];
                var f = splitDistances[i + 1];

                var viewSplit = vxGeometryHelper.splitFrustum(n, f, viewProjInverse).ToArray();
                var frustumCorners = viewSplit.Select(v => Vector3.Transform(v, ShadowView)).ToArray();
                var cameraPosition = Vector3.Transform(viewInverse.Translation, ShadowView);

                var viewMin = frustumCorners.Aggregate((v1, v2) => Vector3.Min(v1, v2));
                var viewMax = frustumCorners.Aggregate((v1, v2) => Vector3.Max(v1, v2));

                var arenaBB = vxGeometryHelper.transformBoundingBox(bbDim, ShadowView);

                var minZ = -arenaBB.Max.Z;
                var maxZ = -arenaBB.Min.Z;

                var range = Math.Max(
                    1.0f / camera.Projection.M11 * -splitPlanes[i + 1] * 2.0f,
                    -splitPlanes[i + 1] - (-splitPlanes[i])
                );

                // range is slightly too small, so add in some padding
                float padding = 5.0f;
                var quantizationStep = (range + padding) / (float)ShadowMapSize;

                var x = vxGeometryHelper.determineShadowMinMax1D(frustumCorners.Select(v => v.X), cameraPosition.X, range);
                var y = vxGeometryHelper.determineShadowMinMax1D(frustumCorners.Select(v => v.Y), cameraPosition.Y, range);

                var projectionMin = new Vector3(x[0], y[0], minZ);
                var projectionMax = new Vector3(x[1], y[1], maxZ);

                // Add in padding
                {
                    range += padding;
                    projectionMin.X -= padding / 2.0f;
                    projectionMin.Y -= padding / 2.0f;
                }

                // quantize
                if (mSnapShadowMaps)
                {
                    // compute range
                    var qx = (float)Math.IEEERemainder(projectionMin.X, quantizationStep);
                    var qy = (float)Math.IEEERemainder(projectionMin.Y, quantizationStep);

                    projectionMin.X = projectionMin.X - qx;
                    projectionMin.Y = projectionMin.Y - qy;

                    projectionMax.X = projectionMin.X + range;
                    projectionMax.Y = projectionMin.Y + range;
                }

                // compute offset into texture atlas
                int tileX = i % 2;
                int tileY = i / 2;

                // [x min, x max, y min, y max]
                float tileBorder = 3.0f / (float)ShadowMapSize;
                var tileBounds = new Vector4(
                    0.5f * tileX + tileBorder,
                    0.5f * tileX + 0.5f - tileBorder,
                    0.5f * tileY + tileBorder,
                    0.5f * tileY + 0.5f - tileBorder
                );

                var tileMatrix = Matrix.Identity;
                tileMatrix.M11 = 0.25f;
                tileMatrix.M22 = -0.25f;
                tileMatrix.Translation = new Vector3(0.25f + tileX * 0.5f, 0.25f + tileY * 0.5f, 0);

                return new
                {
                    Distance = f,
                    ViewFrustum = viewSplit,
                    Projection = Matrix.CreateOrthographicOffCenter(projectionMin.X, projectionMax.X, projectionMin.Y, projectionMax.Y, projectionMin.Z, projectionMax.Z),
                    TileTransform = tileMatrix,
                    TileBounds = tileBounds,
                };
            }).ToArray();

            ViewFrustumSplits = splitData.Select(s => s.ViewFrustum).ToArray();
            ShadowSplitProjections = splitData.Select(s => ShadowView * s.Projection).ToArray();
            ShadowSplitProjectionsWithTiling = splitData.Select(s => ShadowView * s.Projection * s.TileTransform).ToArray();
            ShadowSplitTileBounds = splitData.Select(s => s.TileBounds).ToArray();
            ShadowProjections = splitData.Select(s => s.Projection).ToArray();
        }


        /// <summary>
        /// Swaps the shadow map with block texture.
        /// </summary>
        public void swapShadowMapWithBlockTexture()
        {
            RenderTarget2D tmp = RT_ShadowMap;
            RT_ShadowMap = RT_BlockTexture;
            RT_BlockTexture = tmp;
        }

        /// <summary>
        /// Fills the texture with block pattern.
        /// </summary>
        /// <param name="targetTexture">Target texture.</param>
        /// <param name="blockSize">Block size.</param>
        public void fillTextureWithBlockPattern(Texture2D targetTexture, int blockSize)
        {
            targetTexture.SetData(
                Enumerable.Range(0, RT_ShadowMap.Height * RT_ShadowMap.Width).Select(i =>
                {
                    int x = i % RT_ShadowMap.Width;
                    int y = i / RT_ShadowMap.Height;

                    var xBlack = (x % blockSize) > (blockSize / 2);
                    var yBlack = (y % blockSize) > (blockSize / 2);

                    return (xBlack ^ yBlack) ? 1.0f : 0.0f;
                }).ToArray());
        }
    }
}
#endif