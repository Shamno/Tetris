using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Threading;


namespace テトリス
{
    public partial class Form1 : Form
    {
        const int NUM_COLORS = 8;
        const int LENGTH = 20;
        const int AREA_WIDTH = 10;
        const int AREA_HEIGHT = 20;
        int rotation;
        int pattern;
        int count;
        int[,] screen;
        Block[] blocks;
        Point center;
        bool gameOver;

        Random random = new Random();

        Bitmap bitmap;
        Rectangle[] srcRect;
        Rectangle[,] destRect;
        #region 長ブロック
        Block block1 = new Block(new Point[,]{{ new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                                                { new Point(0, -2), new Point(0, -1), new Point(0, 0), new Point(0, 1) }}, 1);

        #endregion

        #region L字ブロック
        Block block2 = new Block(new Point[,]{{ new Point(1, 0), new Point(0, 0), new Point(0, -1), new Point(0, -2) },
                                                { new Point(0, 1), new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                                                { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(0, 2) },
                                                { new Point(0, -1), new Point(0, 0), new Point(1, 0), new Point(2, 0) }}, 5 );

        #endregion

        #region 逆L字ブロック

        Block block3 = new Block(new Point[,]{  { new Point(-1, 0), new Point(0, 0), new Point(0, -1), new Point(0, -2) },
                                                { new Point(0, 1), new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                                                { new Point(-1, 0), new Point(0, 0), new Point(0, 1), new Point(0, 2) },
                                                { new Point(0, -1), new Point(0, 0), new Point(-1, 0), new Point(-2, 0) }}, 3);

        #endregion

        #region 四角ブロック

        Block block4 = new Block(new Point[,] { { new Point(1, 0), new Point(0, 0), new Point(0, 1), new Point(1, 1) } }, 2);

        #endregion

        #region 凸型ブロック

        Block block5 = new Block(new Point[,]{{ new Point(-1, 0), new Point(0, 0), new Point(1, 0), new Point(0, -1) },
                                                { new Point(0, 1), new Point(0, 0), new Point(0, -1), new Point(-1, 0) },
                                                { new Point(1, 0), new Point(0, 0), new Point(-1, 0), new Point(0, 1) },
                                                { new Point(0, -1), new Point(0, 0), new Point(0, 1), new Point(1, 0) }}, 7);

        #endregion

        #region カギ型ブロック

        Block block6 = new Block(new Point[,]{{new Point(-1, 0), new Point(0, 0), new Point(0, -1), new Point(1, -1) },
                                                { new Point(0, 1), new Point(0, 0), new Point(-1, 0), new Point(-1, -1) }}, 5);

        #endregion

        #region 逆カギ型ブロック

        Block block7 = new Block(new Point[,]{{ new Point(1, 0), new Point(0, 0), new Point(0, -1), new Point(-1, -1) },
                                                { new Point(0, -1), new Point(0, 0), new Point(-1, 0), new Point(-1, 1) }}, 6);

        #endregion


        class Block
        {
            public Point[,] points;
            public int color; // 1:水色 2:黄色 3:青 4:赤 5:橙 6:緑 7:紫

            public Block(Point[,] ps, int c)
            {
                points = ps;
                color = c;
            }
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            #region 画像データ読み込み
            srcRect = new Rectangle[8];
            destRect = new Rectangle[10, 20];
            bitmap = new Bitmap("Tetris.bmp");
            center = new Point(3, 2);

            for (int i = 0; i < srcRect.Length; i++)
            {
                srcRect[i] = new Rectangle(LENGTH * i, 0, LENGTH, LENGTH);
            }

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    destRect[x, y] = new Rectangle(x * LENGTH, y * LENGTH, LENGTH, LENGTH);
                }
            }

            #endregion

            gameOver = false;
            count = 0;
            rotation = 0;
            pattern = 0;
            screen = new int[AREA_WIDTH, AREA_HEIGHT];
            blocks = new Block[] { block1, block2, block3, block4, block5, block6, block7 };

        }

        bool CheckMove(Block block, Point point, int rotation)
        {

            for (int i = 0; i < block.points.GetLength(1); i++)
            {
                int x = point.X + block.points[rotation, i].X;
                int y = point.Y + block.points[rotation, i].Y;

                if (y >= AREA_HEIGHT || x < 0 || x >= AREA_WIDTH)
                {
                    return false;
                }
                else if (y>=0&&screen[x,y]>0)
                {
                    return false;
                }

            }

            return true;
        }
        bool CheckDelete(int y)
        {
            for(int x=0;x<AREA_WIDTH;x++)
            {
                if(screen[x,y]==screen[x,0])
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int y = 0; y < AREA_HEIGHT; y++)
            {
                for (int x = 0; x < AREA_WIDTH; x++)
                {
                    e.Graphics.DrawImage(bitmap, destRect[x, y], srcRect[screen[x,y]], GraphicsUnit.Pixel);
                }
            }

            for (int i = 0; i < blocks[pattern].points.GetLength(1); i++)
            {
                int x = center.X + blocks[pattern].points[rotation, i].X;
                int y = center.Y + blocks[pattern].points[rotation, i].Y;

                if(y>=0)
                {
                    if(gameOver==false)
                    {
                        e.Graphics.DrawImage(bitmap, destRect[x, y], srcRect[blocks[pattern].color], GraphicsUnit.Pixel);
                    }
                    else
                    {
                        e.Graphics.DrawImage(bitmap, destRect[x, y], srcRect[4], GraphicsUnit.Pixel);
                    }

                }

            }

        }

        void GameOver()
        {
            for(int y=0;y<AREA_HEIGHT;y++)
            {
                for(int x=0;x<AREA_WIDTH;x++)
                {
                    if(screen[x,y]>=1)
                    {
                        screen[x, y] = 4;
                    }
                }
            }

            timer1.Enabled = false;
            gameOver = true;
        }
        void DeleteLine(int y)
        {
            for(int Y=y;Y>=0;Y--)
            {
               for(int x=0;x<AREA_WIDTH;x++)

                    if(Y>0)
                    {
                        screen[x, Y] = screen[x, Y-1];
                    }
                    else
                    {
                        screen[x, 0] = 0;
                    }
            }
        }
        void Delete()
        {
            for(int y=0;y<AREA_HEIGHT;y++)
            {
                if(CheckDelete(y))
                {
                    DeleteLine(y);
                }
            }
        }
        void PutBlocks()
        {
            for(int i=0;i<blocks[pattern].points.GetLength(1);i++)
            {
                int x = center.X + blocks[pattern].points[rotation, i].X;
                int y = center.Y + blocks[pattern].points[rotation, i].Y;

                screen[x,y] = blocks[pattern].color;
            }

            Delete();
        }

        #region 操作
        protected override void OnKeyDown(KeyEventArgs e)
        {
            int newRotation = rotation;
            Point newCenter = center;
            if (gameOver == true) return;
            switch (e.KeyData)
            {
                #region 方向転換
                case Keys.Z:

                    newRotation = rotation + 1;
                    if (newRotation == blocks[pattern].points.GetLength(0))
                    {
                        newRotation = 0;
                    }
                    if (CheckMove(blocks[pattern], newCenter, newRotation))
                    {
                        rotation = newRotation;
                    }
                    break;

                case Keys.X:
                    newRotation = rotation - 1;
                    if (newRotation == -1)
                    {
                        newRotation = blocks[pattern].points.GetLength(0)-1;
                    }
                    if (CheckMove(blocks[pattern], newCenter, newRotation))
                    {
                        rotation = newRotation;
                    }
                    break;

                #endregion
                #region 移動
                case Keys.Down:

                    newCenter.Y++;
                    if (CheckMove(blocks[pattern], newCenter, newRotation))
                    {
                        center = newCenter;
                    }
                    else
                    {
                        PutBlocks();
                        NewBlocks();
                        if (!CheckMove(blocks[pattern], center, rotation))
                        {
                            PutBlocks();
                            GameOver();
                        }
                    }
                    break;

                case Keys.Right:
                    newCenter.X++;
                    if (CheckMove(blocks[pattern], newCenter, newRotation))
                    {
                        center = newCenter;
                    }
                    break;

                case Keys.Left:
                    newCenter.X--;
                    if (CheckMove(blocks[pattern], newCenter, newRotation))
                    {
                        center = newCenter;
                    }
                    break;
                    #endregion
            }
            Invalidate();

        }
        #endregion
        void NewBlocks()
        {
            center = new Point(3, 2);

            int n = random.Next(7);
            pattern = n;
            int m = random.Next(blocks[pattern].points.GetLength(0));
            rotation = m;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            int newRotation = rotation;
            Point newCenter = center;

            count++;

            if(count==20)
            {
                newCenter.Y++;
                if (CheckMove(blocks[pattern], newCenter, newRotation))
                {
                    center = newCenter;
                }
                else
                {
                    PutBlocks();
                    NewBlocks();
                    if(!CheckMove(blocks[pattern],center,rotation))
                    {
                        PutBlocks();
                        GameOver();
                    }
                }
                count = 0;

                Invalidate();
            }
        }


    }

}
