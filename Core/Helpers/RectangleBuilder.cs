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
        private static Random random = new Random();
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

        public Rectangle BuildRandom()
        {

          int w =  RectangleBuilder.random.Next(1, SizeMaxWidth);
          int h = RectangleBuilder.random.Next(1, SizeMaxHeight);
          do
          {
            w = RectangleBuilder.random.Next(1, SizeMaxWidth);
            h = RectangleBuilder.random.Next(1, SizeMaxHeight);
            
          } while (w<h);

          return new Rectangle(RectangleBuilder.random.Next(1, MaxCanvasHeight), RectangleBuilder.random.Next(1, MaxCanvasWidth),w,h);
        }
        public Rectangle BuildMaximized()
        {
          return new Rectangle(0, 0, SizeMaxWidth, SizeMaxHeight);
        }

        public IEnumerable<Rectangle> BuildRandomList(int count)
        {
            lock(_syncLock)
            {
                List<Rectangle> result = new List<Rectangle>();

                for (int i = 0; i < count; i++)
                {
                    result.Add(BuildRandom());
                }
                return result;
            }
        }


    }
}
