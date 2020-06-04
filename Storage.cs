using System.Drawing;

namespace Chess_4_with_Framework
{
    public class Storage
    {
        public Point Point { get; set; }
        public EPieceType Type { get; set; }
        public EColor Color { get; set; }
        public bool Taken { get; set; }

        public Storage(Point point, EPieceType type, EColor color)
        {
            Point = point;
            Type = type;
            Color = color;
            Taken = false;
        }
    }
    
}