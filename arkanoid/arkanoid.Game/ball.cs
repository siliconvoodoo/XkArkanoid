// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using System;

namespace arkanoid
{
    public class ball : SyncScript
    {
        // Declared public member fields and properties will show in the game studio

        public override void Start()
        {
            initBallPos = Entity.Transform.Position;  // backup this
            ResetBall();
            HitReaction.Callbacks.Add((s) => Rebound(s.Normal));

            barhitRecv = new EventReceiver<Shock>(BarControl.BarHitEK);
        }

        public Vector2 InitialBallspeed;
        public static EventKey BallLostEK = new EventKey("Global", "Ball lost");

        private Vector3 ballSpeed;
        private EventReceiver<Shock> barhitRecv;
        private Vector3 initBallPos;

        public override void Update()
        {
            MoveBall();
            Shock shock;
            bool onbar = barhitRecv.TryReceive(out shock);
            if (onbar)
            {
                var reflection = shock.Normal + new Vector3(shock.lateralImpulse, 0, 0);
                Rebound(reflection);
            }
        }

        public void FreezeBall()
        {
            ballSpeed = Vector3.Zero;
        }

        public void ResetBall()
        {
            ballSpeed = new Vector3(InitialBallspeed, 0.0f);
            Entity.Transform.Position = initBallPos;
        }

        private void MoveBall()
        {
            var dt = Game.DrawTime.Elapsed;

            var hLimit = 7;
            if (Entity.Transform.Position.X > hLimit || Entity.Transform.Position.X < -hLimit)
            {
                var right = Math.Sign(Entity.Transform.Position.X);
                Rebound(new Vector3(-right, 0.0f, 0.0f));
                // set at free position
                Entity.Transform.Position = new Vector3(hLimit*right, Entity.Transform.Position.Y, Entity.Transform.Position.Z);
            }

            var verticalLimit = 5;
            var touchedBottom = Entity.Transform.Position.Y < -verticalLimit;

            if (Entity.Transform.Position.Y > verticalLimit || touchedBottom)
            {
                var top = Math.Sign(Entity.Transform.Position.Y);
                Rebound(new Vector3(0.0f, -top, 0.0f));
                // set at free position
                Entity.Transform.Position = new Vector3(Entity.Transform.Position.X, verticalLimit*top,
                    Entity.Transform.Position.Z);
            }

            if (touchedBottom)
            {
                // signal game lost.
                BallLostEK.Broadcast();
            }

            var oldpos = Entity.Transform.Position;
            oldpos += ballSpeed*(float) dt.TotalSeconds;
            Entity.Transform.Position = oldpos;
        }

        public void Rebound(Vector3 reflect)
        {
            var v2 = reflect.XY();
            v2.Normalize();
            reflect = new Vector3(v2, 0.0f);
            var proj = Vector3.Dot(ballSpeed, reflect);
            ballSpeed -= 2 * proj * reflect;
        }
    }
}
