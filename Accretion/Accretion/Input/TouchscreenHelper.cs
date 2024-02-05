using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;

namespace Accretion.Input
{
    class TouchscreenHelper
    {
        private static List<Vector2> taps = new List<Vector2>();
        private static List<GestureSample> pinches = new List<GestureSample>();
        private static List<Vector2> holds = new List<Vector2>();
        //public static float? getPinchZoom()
        //{
        //    GestureSample? firstPinch = null;
        //    GestureSample? lastPinch = null;
        //    while (TouchPanel.IsGestureAvailable)
        //    {
        //        GestureSample gesture = TouchPanel.ReadGesture();

        //        switch (gesture.GestureType)
        //        {
        //            case GestureType.Pinch:
        //                if (!firstPinch.HasValue)
        //                {
        //                    firstPinch = gesture;
        //                }
        //                else
        //                {
        //                    lastPinch = gesture;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }

        //    if (lastPinch.HasValue)
        //    {
        //        float firstDistance = Math.Abs((firstPinch.Value.Position - firstPinch.Value.Position2).Length());
        //        float secondDistance = Math.Abs((lastPinch.Value.Position - lastPinch.Value.Position2).Length());
        //        return secondDistance / firstDistance;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public static float? getPinchRatio()
        {
            if (pinches.Count < 2)
            {
                updateGestureLists();
            }

            if (pinches.Count > 1)
            {
                float firstDistance = Math.Abs((pinches.First().Position - pinches.First().Position2).Length());
                float secondDistance = Math.Abs((pinches.Last().Position - pinches.Last().Position2).Length());
                pinches.Clear();
                return secondDistance / firstDistance;
            }
            if (pinches.Count == 1)
            {
                pinches.Clear();
            }

            return null;
        }

        public static List<Vector2> getTaps()
        {
            List<Vector2> returnTaps = null;

            if (taps.Count < 1)
            {
                updateGestureLists();
            }

            if (taps.Count > 0)
            {
                returnTaps = new List<Vector2>(taps);
                taps.Clear();
            }

            return returnTaps;
        }

        public static List<Vector2> getHolds()
        {
            List<Vector2> returnHolds = null;

            if (holds.Count < 1)
            {
                updateGestureLists();
            }            
            
            if (holds.Count > 0)
            {
               returnHolds = new List<Vector2>(holds);
               holds.Clear();
            }
            
            return returnHolds;
        }

        private static void updateGestureLists()
        {
            while (TouchPanel.EnabledGestures != GestureType.None && TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();

                switch (gesture.GestureType)
                {
                    case GestureType.Pinch:
                        pinches.Add(gesture);
                        break;
                    case GestureType.Tap:
                        taps.Add(gesture.Position);
                        break;
                    case GestureType.Hold:
                        holds.Add(gesture.Position);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
