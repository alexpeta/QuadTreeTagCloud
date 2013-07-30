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
        private static Random r = new Random();
        private static readonly object _syncLock = new object();

        public int SizeMaxWidth { get; private set; }
        public int SizeMaxHeight { get; private set; }
        public int MaxCanvasHeight { get; private set; }
        public int MaxCanvasWidth { get; private set; }

        #region Statics
        public static RectangleBuilder ARectangle()
        {
            return new RectangleBuilder();
        }
        public static Rectangle GetRectangleWithRandomProperties()
        {
          return RectangleBuilder.ARectangle()
                                     .WithMaxWidth(500)
                                     .WithMaxHeight(300)
                                     .WithCanvasMaxHeight(900)
                                     .WithCanvasMaxWidth(1600)
                                     .Build();
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
          return new Rectangle(RectangleBuilder.r.Next(1, MaxCanvasHeight), RectangleBuilder.r.Next(1, MaxCanvasWidth), RectangleBuilder.r.Next(1, SizeMaxWidth), RectangleBuilder.r.Next(1, SizeMaxHeight));
        }
        public IEnumerable<Rectangle> BuildList(int count)
        {
            lock(_syncLock)
            {
                List<Rectangle> result = new List<Rectangle>();

                for (int i = 0; i < count; i++)
                {
                    result.Add(Build());
                }
                return result;
            }
        }


    }
}
