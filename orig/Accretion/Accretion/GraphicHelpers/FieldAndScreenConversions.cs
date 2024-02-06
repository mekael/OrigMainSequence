using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Accretion.GameplayElements.Objects;

namespace Accretion.GraphicHelpers
{
    public static class FieldAndScreenConversions
    {
        public static Vector2 GetScreenLocation(Vector2 fieldLocation, Vector2 cameraFieldLocation, float zoomLevel)
        {
            return (fieldLocation - cameraFieldLocation) / zoomLevel;
        }

        public static Vector2 GetFieldLocation(Vector2 screenLocation, Vector2 cameraFieldLocation, float zoomLevel)
        {
            return screenLocation * zoomLevel + cameraFieldLocation;
        }

        public static int GetScreenScaledRadius(int radius, float zoomLevel)
        {
            int scaledradius = (int)Math.Ceiling((double)radius / zoomLevel);
            if (scaledradius < 1)
            {
                scaledradius = 1;
            }

            return scaledradius;
        }

        public static Boolean IsVisible(SpaceObject spaceObject, Vector2 cameraFieldLocation, Vector2 windowSize, float zoomLevel)
        {
            int scaledRadius = GetScreenScaledRadius(spaceObject.getRadius(), zoomLevel);
            Vector2 screenLocation = GetScreenLocation(spaceObject.getFieldLocation(), cameraFieldLocation, zoomLevel);
            return (screenLocation.X + scaledRadius > 0 &&
                screenLocation.X - scaledRadius < windowSize.X &&
                screenLocation.Y + scaledRadius > 0 &&
                screenLocation.Y - scaledRadius < windowSize.Y);
        }
    }
}
