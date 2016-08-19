//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Input;
//
////Virtex vxEngine Declaration
//using Microsoft.Xna.Framework.Media;
//using vxVertices.Core;
// 
//
//namespace MetricRacer.Base
//{
//    public class xAdds : xEnvrio
//    {
//        Video video;
//        VideoPlayer player;
//        /// <summary>
//        /// Creates a New Instance of the Base Ship Class
//        /// </summary>
//        /// <param name="AssetPath"></param>
//        public xAdds(vxEngine vxEngine, Model entityModel, Vector3 StartPosition)
//            : base(vxEngine, entityModel, StartPosition)
//        {
//            video = vxEngine.Game.Content.Load<Video>("vids/test/Test");
//            player = new VideoPlayer();
//            //player.Play(video);
//
//            //player.IsLooped = true;
//
//            DoEdgeDetect = false;
//            RenderEvenInDebug = true;
//            mainTechnique = "NoLightning";
//            DoShadowMap = false;
//
//                //player.Play(video);
//            
//        }
//
//        public override void Update(GameTime gameTime)
//        {
//            /*
//            if (player.PlayPosition.TotalMilliseconds > player.Video.Duration.TotalMilliseconds - 100)
//            {
//                player.Stop();
//                player.Play(video);
//            }
//
//            ModelTexture = player.GetTexture();
//            */
//            base.Update(gameTime);
//        }
//
//        public override void RenderMesh(string RenderTechnique)
//        {    
//            
//            base.RenderMesh(RenderTechnique);
//        }
//
//    }
//}
