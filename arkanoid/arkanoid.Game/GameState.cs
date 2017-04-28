// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Engine.Events;
using SiliconStudio.Xenko.UI.Controls;
using SiliconStudio.Xenko.UI.Events;

namespace arkanoid
{
    public class GameState : AsyncScript
    {
        // Declared public member fields and properties will show in the game studio
        
        public Entity ballEntity;
        public Entity barEntity;
        public Entity deathEntity;
        public Entity globalScriptHolder;
        public Entity levelComplete;

        private Vector3 deathOrigPos;
        private EventReceiver ballLostListener;
        private EventReceiver brickErasedListener;

        public override async Task Execute()
        {
            Init();

            while (Game.IsRunning)
            {
                var lost = ballLostListener.TryReceive();
                if (lost)
                    GameOver();

                var shockTreated = brickErasedListener.TryReceive();
                if (shockTreated)
                    CheckIfWon();
                
                await Script.NextFrame();
            }
        }

        private void Init()
        {
            ballLostListener = new EventReceiver(ball.BallLostEK);
            brickErasedListener = new EventReceiver(HitReaction.ShockTreatedEK);
            deathOrigPos = deathEntity.Transform.Position;
            var restartUi = SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "restart ui").Components.Get<UIComponent>();
            var btn = (Button) restartUi.Page.RootElement.VisualChildren.First();
            btn.Click += BtnOnClick;
            ExecuteReset();
        }

        public void CheckIfWon()
        {
            var levelScript = SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "global script holder")
                .Components.Get<Level1>();
            var has = levelScript.HasRemainingBrick();
            if (!has)
                GameWon();
        }

        public void GameWon()
        {
            levelComplete.Transform.Position = Vector3.Zero;
            FreezeBall();
            levelComplete.Components.Get<SpriteComponent>().Enabled = true;
        }

        private void FreezeBall()
        {
            var script = (ball) ballEntity.Components.Get<ScriptComponent>();
            script.FreezeBall();
        }

        public void GameOver()
        {
            FreezeBall();
            deathEntity.Transform.Position = Vector3.Zero;
            DisplayRestartButton();
        }

        private void DisplayRestartButton()
        {
            var restartUi = SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "restart ui").Components.Get<UIComponent>();
            restartUi.Enabled = true;
        }

        private void BtnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            ExecuteReset();
        }

        public void ExecuteReset()
        {
            var script = (ball)ballEntity.Components.Get<ScriptComponent>();
            script.ResetBall();
            HideRestartButton();
            var level = globalScriptHolder.Components.Get<Level1>();
            level.Prepare();
            deathEntity.Transform.Position = deathOrigPos;
            levelComplete.Components.Get<SpriteComponent>().Enabled = false;
        }

        private void HideRestartButton()
        {
            SceneSystem.SceneInstance.RootScene.Entities.First(e => e.Name == "restart ui")
                .Components.Get<UIComponent>()
                .Enabled = false;
        }
    }
}
