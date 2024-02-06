namespace Accretion.GraphicHelpers
{
    internal class PlatformSpecificStrings
    {
        public const string REPLAY_OR_BACK = "Press [Space] to play the level again or [Escape] to select a new level.";
        public const string CONTROLS = "Move by [Clicking] your mouse. Press [Esc] at any time to bring up the menu.";
        public const string POWERUP_CONTROL = "[Right Click]";
        public const string ZOOM_HINT = "Use the [Mouse Scroll Wheel] to zoom out";

#if WINDOWS
        public const string REPLAY_OR_BACK = "Press [Space] to play the level again or [Escape] to select a new level.";
        public const string CONTROLS = "Move by [Clicking] your mouse. Press [Esc] at any time to bring up the menu.";
        public const string POWERUP_CONTROL = "[Right Click]";
        public const string ZOOM_HINT = "Use the [Mouse Scroll Wheel] to zoom out";
#elif XBOX
        public const string REPLAY_OR_BACK = "Press [X] to play the level again or [B] to select a new level.";
        public const string CONTROLS = "Use the [Left Stick] and [A] to move. Press [B] at any time to bring up the menu.";
        public const string POWERUP_CONTROL = "Press [Right Trigger]";
        public const string ZOOM_HINT = "Use the [Right Stick] to zoom out";
#elif WINDOWS_PHONE
        public const string REPLAY_OR_BACK = "[Tap and Hold] the screen to play the level again or press [Back] to select a new level.";
        public const string CONTROLS = "[Tap] the screen to move and [Pinch] to zoom out. Press [Back] at any time to bring up the menu.";
        public const string POWERUP_CONTROL = "[Vertical Swipe]";
        public const string ZOOM_HINT = "[Pinch] to zoom out";
#endif
    }
}