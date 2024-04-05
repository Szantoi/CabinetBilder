using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabinetBilder_AutoCad2025
{
    public class Patameter
    {
        public int Accuracy { get; set; }
        public class CNC()
        {
            public class Layer()
            {
                public string Separator { get; } = "_";
                public byte[] ColorAttention { get; } = [0, 0, 255];

                public class Sheet()
                {
                    public string Prefix { get; } = "DIM%Z#";
                    public byte[] Corol { get; } = [255, 0, 255];
                }

                public class Drill()
                {
                    public string Prefix { get; } = "D%Z#";
                    public string Postfix { get; } = "%R#35";
                    public byte[] Corol { get; } = [255, 0, 0];
                    public bool DepthIsCorrect(double i, double materialTicknes, Vector3d normal)
                    {
                        bool l = false;
                        Physics.Drill drillPhy = new();
                        if (Math.Abs(normal.Z) == 1)
                        {
                            l = i > -1 * (materialTicknes + drillPhy.Overboring) && i < 0;
                        }
                        else
                        {
                            l = i > -1 * drillPhy.DepthMaxY && i < 0;
                        }
                        return l;
                    }
                }

                public class Milling()
                {
                    public string Prefix { get; } = "M%T#1124%Z#";
                    public byte[] Corol { get; } = [255, 255, 0];
                    public bool DepthIsCorrect(double i, double materialTicknes)
                    {
                        Physics.Milling millingPhy = new();
                        return i > -1 * (materialTicknes + millingPhy.Overboring) && i < 0;
                    }
                }
            }

            public class Physics()
            {
                public class Drill()
                {
                    public double Overboring { get; } = 1;
                    public double DimeterMAX { get; } = 35;
                    public double DimeterMIN { get; } = 35;
                    public double DepthMaxY { get; } = 35;
                }
                public class Milling()
                {
                    public double Overboring { get; } = 1;
                }

            }
        }

        public Patameter()
        {
            this.Accuracy = 2;
        }


    }
}
