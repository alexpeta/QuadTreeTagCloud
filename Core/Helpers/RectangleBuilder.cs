using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class RectangleBuilder
    {
        public int SizeMaxWidth { get; private set; }
        public int SizeMaxHeight { get; private set; }
        public int MaxCanvasHeight { get; private set; }
        public int MaxCanvasWidth { get; private set; }

        #region Statics
        public static RectangleBuilder ARectangle()
        {
            return new RectangleBuilder();
        }
        #endregion Statics

        #region Constructors
        private RectangleBuilder()
        {
        }
        #endregion Constructors

        public RectangleBuilder WithMaxWidth(int maxWidth)
        {
            SizeMaxWidth = maxWidth;
            return this;
        }
        public RectangleBuilder WithMaxHeight(int maxHeight)
        {
            SizeMaxHeight = maxHeight;
            return this;
        }
        public RectangleBuilder WithCanvasMaxHeight(int canvasHeight)
        {
            MaxCanvasHeight = canvasHeight;
            return this;
        }
        public RectangleBuilder WithCanvasMaxWidth(int maxWidth)
        {
            MaxCanvasWidth = maxWidth;
            return this;
        }

        public Rectangle Build()
        {
            Random r = new Random();
            return new Rectangle(r.Next(1, MaxCanvasHeight), r.Next(1, MaxCanvasWidth), r.Next(1, SizeMaxWidth), r.Next(1, SizeMaxHeight));
        }
        public IEnumerable<Rectangle> BuildList(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Build();
            }
        }
    }
}
