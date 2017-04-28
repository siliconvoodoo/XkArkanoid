// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Input;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Physics;
using SiliconStudio.Xenko.Engine.Events;

namespace arkanoid
{
    public class BarControl : AsyncScript
    {
        public override async Task Execute()
        {
            Init();
            while (Game.IsRunning)
            {
                ProcessInput();

                await Script.NextFrame();
            }
        }

        private void Init()
        {
            var trigger = Entity.Get<PhysicsComponent>();
            trigger.ProcessCollisions = true;

            trigger.Collisions.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    var collision = (Collision) args.Item;
                    var contact = collision.Contacts.First();
                    var shockNormal = contact.Normal;
                    if (contact.ColliderA.Entity != Entity)
                        shockNormal *= -1;
                    BarHitEK.Broadcast(new Shock
                    {
                        Entity = Entity,
                        Normal = new Vector3(shockNormal.XY(), 0.0f),
                        lateralImpulse = -Math.Sign(currentMovement) * 0.3f
                    });
                }
            };
        }

        public static EventKey<Shock> BarHitEK = new EventKey<Shock>("Global", "bar hit");

        public float BarSpeed { get; set; } = 1.0f;
        private float currentMovement;
       
        private void ProcessInput()
        {
            var dt = Game.DrawTime.Elapsed;

            bool left = Input.IsKeyDown(Keys.Left);
            bool right = Input.IsKeyDown(Keys.Right);

            currentMovement = 0.0f;
            if (left && !right && Entity.Transform.Position.X > -7)
            {
                currentMovement = -BarSpeed;
            }
            if (right && !left && Input.IsKeyDown(Keys.Right) && Entity.Transform.Position.X < 7)
            {
                currentMovement = BarSpeed;
            }

            var posCopy = Entity.Transform.Position;
            posCopy.X += currentMovement * (float)dt.TotalSeconds;
            Entity.Transform.Position = posCopy;
        }
    }
}
