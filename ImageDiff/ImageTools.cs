using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDiff
{
    public static class ImageTool
    {
        private class Coords
        {
            public int MaxX { get; set; }
            public int MaxY { get; set; }
            public int MinX { get; set; }
            public int MinY { get; set; }
            public int GetWidth()
            {
                return MaxX - MinX;
            }
            public int GetHeight()
            {
                return MaxY - MinY;
            }
            public Coords(int maxX, int maxY, int minX, int minY)
            {
                this.MaxX = maxX;
                this.MaxY = maxY;
                this.MinX = minX;
                this.MinY = minY;
            }

        }
        private static bool rgbCompare(Color pixel1, Color pixel2)
        {
            int maxDifference = 15;
            if (Math.Abs(pixel1.R - pixel2.R) < maxDifference ||
                Math.Abs(pixel1.G - pixel2.R) < maxDifference ||
                Math.Abs(pixel1.B - pixel2.B) < maxDifference)
                return true;
            return false;
        }
        private static void MergeAreaCoords(ref List<Coords> listOfAreas, int i, int j)
        {
            if (listOfAreas[i].MaxX < listOfAreas[j].MaxX)
                listOfAreas[i].MaxX = listOfAreas[j].MaxX;
            if (listOfAreas[i].MaxY < listOfAreas[j].MaxY)
                listOfAreas[i].MaxY = listOfAreas[j].MaxY;

            if (listOfAreas[i].MinX > listOfAreas[j].MinX)
                listOfAreas[i].MinX = listOfAreas[j].MinX;
            if (listOfAreas[i].MinY > listOfAreas[j].MinY)
                listOfAreas[i].MinY = listOfAreas[j].MinY;
            listOfAreas.RemoveAt(j);
        }
        private static bool CompareAreaCoords(ref List<Coords> listOfAreas, int i, int j)
        {
            if (listOfAreas[i].MinX > listOfAreas[j].MinX && listOfAreas[i].MinX < listOfAreas[j].MaxX &&
                    listOfAreas[i].MinY > listOfAreas[j].MinY && listOfAreas[i].MinY < listOfAreas[j].MaxY)
            {
                MergeAreaCoords(ref listOfAreas, i, j);
                return true;
            }
            else if (listOfAreas[i].MinX > listOfAreas[j].MinX && listOfAreas[i].MinX < listOfAreas[j].MaxX &&
                        listOfAreas[i].MaxY > listOfAreas[j].MinY && listOfAreas[i].MaxY < listOfAreas[j].MaxY)
            {
                MergeAreaCoords(ref listOfAreas, i, j);
                return true;
            }
            return false;
        }
        const int range = 6;
        public static Bitmap GetDifferenceImage(Bitmap BMimage1, Bitmap BMimage2)
        {
            if (BMimage1.Width == BMimage2.Width && BMimage1.Height == BMimage2.Height)
            {
                int width = BMimage1.Width;
                int height = BMimage1.Height;
                Bitmap BMimage3 = new Bitmap(width, height);
                Dictionary<int, List<Point>> dictOfCoords = new Dictionary<int, List<Point>>();
                List<Coords> listOfAreas = new List<Coords>();
                int ID = 1;
                int tempID;
                Func<Point, bool> isPointSuitable;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (!rgbCompare(BMimage1.GetPixel(x, y), BMimage2.GetPixel(x, y)))
                        {
                            if (dictOfCoords.Count > 0)
                            {
                                isPointSuitable = point =>
                                    Math.Abs(point.X + x) <= range && Math.Abs(point.Y + y) <= range ||
                                    Math.Abs(point.X - x) <= range && Math.Abs(point.Y - y) <= range ||
                                    Math.Abs(point.X + x) <= range && Math.Abs(point.Y - y) <= range ||
                                    Math.Abs(point.X - x) <= range && Math.Abs(point.Y + y) <= range ||
                                    Math.Abs(point.X + x) <= range && Math.Equals(point.Y, y) ||
                                    Math.Abs(point.X - x) <= range && Math.Equals(point.Y, y) ||
                                    Math.Equals(point.X, x) && Math.Abs(point.Y + y) <= range ||
                                    Math.Equals(point.X, x) && Math.Abs(point.Y - y) <= range;
                                tempID = dictOfCoords.FirstOrDefault((r => r.Value.Any(isPointSuitable))).Key;
                                if (tempID != 0)
                                    dictOfCoords[tempID].Add(new Point(x, y));
                                else
                                {
                                    dictOfCoords.Add(ID, new List<Point>());
                                    dictOfCoords[ID].Add(new Point(x, y));
                                    ID++;
                                }

                            }
                            else
                            {
                                dictOfCoords.Add(ID, new List<Point>());
                                dictOfCoords[ID].Add(new Point(x, y));
                                ID++;
                            }
                        }
                    }
                }
                foreach (var coordList in dictOfCoords.Values)
                {
                    listOfAreas.Add(new Coords(coordList.Max(r => r.X), coordList.Max(r => r.Y),
                        coordList.Min(r => r.X), coordList.Min(r => r.Y)));
                }

                for (int i = 0; i < listOfAreas.Count; i++)
                {
                    for (int j = 0; j < listOfAreas.Count; j++)
                    {
                        if (!CompareAreaCoords(ref listOfAreas, i, j))
                            CompareAreaCoords(ref listOfAreas, j, i);
                    }
                }

                foreach (var coordList in listOfAreas)
                {
                    Graphics g = Graphics.FromImage(BMimage1);
                    g.DrawRectangle(new Pen(Color.Red, 2f), new Rectangle(
                        coordList.MinX,
                        coordList.MinY,
                        coordList.GetWidth(),
                        coordList.GetHeight()));
                }
                return BMimage1;
            }
            return null;
        }
    }
}
