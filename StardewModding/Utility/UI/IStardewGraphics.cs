﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dannnno.StardewMods.UI
{
    public interface IStardewGraphics
    {
        /// <summary>
        /// Get whether a menu can be opened
        /// </summary>
        public bool CanOpenMenu { get; }

        /// <summary>
        /// Get whether the game is ready for mods to interact
        /// </summary>
        public bool GameIsReady { get; }

        /// <summary>
        /// Get the width of the game window
        /// </summary>
        public int WindowWidth { get; }

        /// <summary>
        /// Get the height of the game window
        /// </summary>
        public int WindowHeight { get; }

        /// <summary>
        /// Get the tile size in the game window
        /// </summary>
        public int TileSize { get; }

        /// <summary>
        /// Get whether to show the menu's background
        /// </summary>
        public bool ShowMenuBackground { get; }

        /// <summary>
        /// Get the texture to use when fading to black
        /// </summary>
        public Texture2D FadeToBlackRectangle { get; }

        /// <summary>
        /// Get the cursor texture
        /// </summary>
        public Texture2D MouseCursor { get; }

        /// <summary>
        /// Get the bounding box of the game window
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Get the position of the mouse before this drawing event
        /// </summary>
        public Vector2 PreviousMousePosition { get; }


        #region Methods

        /// <summary>
        /// Draw a dialogue box
        /// </summary>
        /// <param name="x">X position of the box</param>
        /// <param name="y">Y position of the box</param>
        /// <param name="width">The width of the box</param>
        /// <param name="height">The height of the box</param>
        /// <param name="speaker">Whether to include the speaker</param>
        /// <param name="drawOnlyBox">Whether to only draw the box</param>
        /// <param name="message">The message</param>
        /// <param name="objectDialogueWithPortrait">Whether to include the pportrait</param>
        /// <param name="ignoreTitleSafe">Whether to ignore the title</param>
        public void DrawDialogueBox(int x, int y, int width, int height, bool speaker, bool drawOnlyBox, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false);

        /// <summary>
        /// Draw a dialogue box
        /// </summary>
        /// <param name="r">Bounding box</param>
        /// <param name="speaker">Whether to include the speaker</param>
        /// <param name="drawOnlyBox">Whether to only draw the box</param>
        /// <param name="message">The message</param>
        /// <param name="objectDialogueWithPortrait">Whether to include the pportrait</param>
        /// <param name="ignoreTitleSafe">Whether to ignore the title</param>
        /// <summary>
        public void DrawDialogueBox(Rectangle r, bool speaker = false, bool drawOnlyBox = true, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = false);

        #endregion
    }
}
