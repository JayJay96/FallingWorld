using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FallingWorld
{

    public class GameLibComponent : DrawableGameComponent
    {
        public static ContentManager LibContent;

        public GameLibComponent(Game game)
            :base(game)
        {
            ResourceContentManager resxContent;
            resxContent = new ResourceContentManager(game.Services, ResourceContent.ResourceManager);
            LibContent = resxContent;
        }
    }
}
