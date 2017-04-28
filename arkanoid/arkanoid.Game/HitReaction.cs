// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core;

namespace arkanoid
{
    public class HitReaction : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
       
        public static List<Action<Shock>> Callbacks = new List<Action<Shock>>();

        public static EventKey ShockTreatedEK = new EventKey("Global", "Shock treated");

        public override async Task Execute()
        {
            var brickHit = new EventReceiver<Shock>(BrickShock.BrickHit);

            while (Game.IsRunning)
            {
                var result = await brickHit.ReceiveAsync();
                
                TreatShock(result);
            }
        }
        
        private void TreatShock(Shock shock)
        {
            foreach (var c in Callbacks)
                c(shock);
            SceneSystem.SceneInstance.RootScene.Entities.Remove(shock.Entity);
            SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "global script holder")
                .Components.Get<Level1>().SignalDeletedBrick(shock.Entity);

            ShockTreatedEK.Broadcast();
        }
    }
}
