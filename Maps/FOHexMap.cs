namespace FOCommon.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;

    public class FOHexMap
    {
        // Base offsets.
        private PointF baseOffset = new PointF(3000, 100);

        // Tile offsets from scenery/protos.
        private readonly float tileOffX = -64;
        private readonly float tileOffY = -16;

        // Hex sizes
        private readonly int hexW = 32;
        private readonly int hexH = 16;
        private readonly int hexHEdge = 12; // height of left/right edge.

        public FOHexMap() { }

        public FOHexMap(PointF baseOffset)
        {
            this.baseOffset = baseOffset;
        }

        /// <summary>
        /// Common algoritm used for both tile placement and other objects.
        /// </summary>
        /// <returns></returns>
        private PointF GetCoords(Point hex)
        {
            float x = ((hex.Y * hexH) - (hex.X * hexW));
            float y = (Math.Abs((hex.Y + (hex.X / 2))) * hexHEdge);

            if (hex.X > 1) x += (hex.X / 2) * hexH;

            return new PointF(baseOffset.X + x, baseOffset.Y + y);
        }

        /// <summary>
        /// Gets coordinates for scenery and other things that uses protoitems.
        /// </summary>
        /// <returns></returns>
        public PointF GetObjectCoords(Point hex, Size obj, Point Shift, Point Proto)
        {
            var coords = this.GetCoords(hex);

            coords.X -= obj.Width / 2;
            coords.X -= Proto.X;
            coords.X += Shift.X;
            coords.Y -= obj.Height;
            coords.Y -= Proto.Y;
            coords.Y += Shift.Y;

            return coords;
        }

        /// <summary>
        /// Gets coordinates for tiles, including roofs.
        /// </summary>
        /// <returns></returns>
        public PointF GetTileCoords(Point hex, bool isRoof)
        {
            var coords = this.GetCoords(hex);

            if (isRoof)
               coords.Y -= 92;

            return new PointF(tileOffX + coords.X, tileOffY + coords.Y);
        }
    }
}
