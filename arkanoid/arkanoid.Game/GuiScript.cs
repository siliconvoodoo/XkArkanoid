// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.UI;
using SiliconStudio.Xenko.UI.Controls;
using SiliconStudio.Xenko.UI.Panels;

namespace arkanoid
{
    /// <summary>
    /// The GUI script
    /// </summary>
    public class GuiScript : SyncScript
    {
		public SpriteFont Font { get; set; }
		
        public void UpdateText()
        {
            var tpf = Game.DrawTime.TimePerFrame;
            var fps = Game.DrawTime.FramePerSecond;
            var font = Font;
            var textBlock = new TextBlock
            {
                Font = font,
                TextSize = 18,
                TextColor = Color.Gold,
                Text = "tpf: " + tpf.ToString() + "\n" + 
                       "fps: " + fps.ToString() + "\n" +
                       "Break the bricks !\n" +
                       "Move : left/right arrows",
            };
            textBlock.SetCanvasRelativePosition(new Vector3(0.008f, 0.8f, 0));

            Entity.Get<UIComponent>().Page = new UIPage { RootElement = new Canvas { Children = { textBlock } } };
        }

        public override void Update()
        {
            UpdateText();
        }
    }
}
