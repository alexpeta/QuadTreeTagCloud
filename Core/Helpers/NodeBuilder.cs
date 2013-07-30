using Core.DataStructures;
using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class NodeBuilder
    {
        private Rectangle _surface = new Rectangle();
        private List<Rectangle> _rectangles = new List<Rectangle>();

        #region Constructors
        private NodeBuilder()
        {
        }
        #endregion Constructors

        #region Statics
        public static NodeBuilder ANode()
        {
            return new NodeBuilder();
        }
        #endregion Statics

        #region Builder
        public NodeBuilder WithSubSurfaces(params Rectangle[] rectangles)
        {
            if (rectangles != null && rectangles.Count() > Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT)
            {
                throw new ArgumentException("number of rectangles exceeds max number of nodes");
            }

            foreach (var item in rectangles)
            {
                _rectangles.Add(item);
            }
            return this;
        }
        public NodeBuilder WithSurface(Rectangle surface)
        {
            _surface = surface;
            return this;
        }
        public QuadTreeNode Build()
        {
            QuadTreeNode result = new QuadTreeNode(_surface,_rectangles.ToArray());
            _rectangles.Clear();
            _surface = null;
            return result;
        }
        #endregion Builder
    }
}
