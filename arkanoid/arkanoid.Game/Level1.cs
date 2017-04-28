// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System.Collections.Generic;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Physics;
using System.Linq;

namespace arkanoid
{
    public class Level1 : StartupScript, ILevel
    {
        private List<Entity> instances = new List<Entity>();

        public bool HasRemainingBrick()
        {
            return instances.Any();
        }

        public void SignalDeletedBrick(Entity e)
        {
            instances.Remove(e);
        }

        public override void Start()
        {
            Prepare();
        }

        public void Prepare()
        {
            ClearAll();

            var brickPrefab = Content.Load<Prefab>("brick");
            EnablePrefab(brickPrefab);

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 4; ++j)
                    instances.AddRange(InstantiateBrickPrefab(brickPrefab, i, j));
            }

            // disable the prefab
            DisablePrefab(brickPrefab);
        }

        public void ClearAll()
        {
            foreach (var e in instances)
            {
                SceneSystem.SceneInstance.RootScene.Entities.Remove(e);
            }
            instances.Clear();
        }

        public void DisablePrefab(Prefab p)
        {
            p.Entities.First().Components.Get<SpriteComponent>().RenderGroup = RenderGroup.Group31;
            p.Entities.First().Components.Get<SpriteComponent>().Enabled = false;
            p.Entities.First().Components.Get<RigidbodyComponent>().Enabled = false;
        }

        public void EnablePrefab(Prefab p)
        {
            p.Entities.First().Components.Get<SpriteComponent>().RenderGroup = RenderGroup.Group0;
            p.Entities.First().Components.Get<SpriteComponent>().Enabled = true;
            p.Entities.First().Components.Get<RigidbodyComponent>().Enabled = true;
        }

        private List<Entity> InstantiateBrickPrefab(Prefab brickPrefab, float x, float y)
        {
            // get the collection of entities instantiated from this prefab
            var instance = brickPrefab.Instantiate();
            var brick = instance[0];
        
            // Change the X coordinate
            brick.Transform.Position.X = x - 5;
            brick.Transform.Position.Y = 2 + y / 2;

            // Add the bullet to the scene
            SceneSystem.SceneInstance.RootScene.Entities.Add(brick);

            return instance;
        }
    }
}
