// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Physics;
using SiliconStudio.Xenko.Engine.Events;

namespace arkanoid
{
    public class Shock
    {
        public Entity Entity;
        public Vector3 Normal;
        public float lateralImpulse;
    }

    public class BrickShock : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public static EventKey<Shock> BrickHit = new EventKey<Shock>("Global", "Brick hit");

        public override async Task Execute()
        {
            var trigger = Entity.Get<PhysicsComponent>();
            trigger.ProcessCollisions = true;

            // Start state machine
            while (Game.IsRunning)
            {
                var firstCollision = await trigger.NewCollision();

                var oneContact = firstCollision.Contacts.First();
                var shockNormal = oneContact.Normal;
                if (oneContact.ColliderA.Entity != Entity)
                    shockNormal *= -1;

                var _2dnormal = shockNormal;
                _2dnormal.Z = 0.0f;
                var shock = new Shock { Entity = Entity, Normal = _2dnormal };

                BrickHit.Broadcast(shock);
            }
        }
    }
}
