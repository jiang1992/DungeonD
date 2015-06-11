using System;
using System.Drawing;

namespace DungeonDrive
{
    public class Door
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public int direction;

        public int DrawX { get { return (int)(x * G.size + G.width / 2 - G.hero.x * G.size - G.size / 2); } }
        public int DrawY { get { return (int)(y * G.size + G.height / 2 - G.hero.y * G.size - G.size / 2); } }


        public Door(int x, int y, int size, int direction) {   // 1: North,   2: East,   3: South,   4: West
            this.direction = direction;

            int newHeight;
            int newWidth;

            if(direction <= 0 || direction > 4 || size <= 0 || size > 2)
            {
                Console.WriteLine("Door is effed in the a");
            }

            switch (direction)
            {
                case 1:
                case 3:
                    newHeight = 1;
                    newWidth = size;
                    break;

                case 2:
                case 4:
                    newHeight = size;
                    newWidth = 1;
                    break;
            }
            
        }
        public void draw(Graphics g)
        {
            g.FillRectangle(Brushes.Red, DrawX, DrawY, G.size * width, G.size * height);
        }
    }
}
