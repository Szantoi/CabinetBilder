using AutoCad2025;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.Cnc))]

namespace CabinetBilder_AutoCad2025
{
    internal class Cnc
    {
        [CommandMethod("Cnc_Preparation")]
        public static void Cnc_Preparation()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor ed = acDoc.Editor;

            using DocumentLock acLockDoc = acDoc.LockDocument();
            if (acDb != null)
            {
                using Transaction acTra = acDb.TransactionManager.StartTransaction();
                if (acTra != null)
                {
                    BlockTable? acBlkTblCor = acTra.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    if (acBlkTblCor != null)
                    {
                        BlockTableRecord? acBlkTabRec = acTra.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        if (acBlkTabRec != null)
                        {
                            List<ObjectId> regionObjectIds = ConRegion.Select(acBlkTabRec, acTra);

                            List<ObjectId> circleObjectIds = ConCircle.Select(acBlkTabRec, acTra);
                            List<ObjectId> lineObjectIds = ConLine.Select(acBlkTabRec, acTra);

                            foreach (ObjectId regionObjectId in regionObjectIds)
                            {
                                Region? region = acTra.GetObject(regionObjectId, OpenMode.ForRead) as Region;
                                if (region != null)
                                {

                                    Line[] lines = ConRegion.ToLine(region);
                                    ed.WriteMessage($"\nthe Line {lines[0].StartPoint},{lines[0].EndPoint}");

                                    foreach (Line line in lines)
                                    {
                                        line.Layer = region.Layer;
                                        acBlkTabRec.AppendEntity(line);
                                        acTra.AddNewlyCreatedDBObject(line, true);
                                    }

                                }
                            }
                        }
                    }
                    acTra.Commit();
                }
            }
        }

        [CommandMethod("Cnc_MuvRot")]
        public static void Cnc_MuvRot()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor ed = acDoc.Editor;

            using DocumentLock acLockDoc = acDoc.LockDocument();
            if (acDb != null)
            {
                using Transaction acTra = acDb.TransactionManager.StartTransaction();
                if (acTra != null)
                {
                    BlockTable? acBlkTblCor = acTra.GetObject(acDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                    if (acBlkTblCor != null)
                    {
                        BlockTableRecord? acBlkTabRec = acTra.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                        if (acBlkTabRec != null)
                        {

                            double materialTicknes = 0;
                            List<ObjectId> regionObjectIds = ConRegion.Select(acBlkTabRec, acTra);
                            if (regionObjectIds != null)
                            {
                                foreach (ObjectId regionObjectId in regionObjectIds)
                                {
                                    Region? region = acTra.GetObject(regionObjectId, OpenMode.ForWrite) as Region;
                                    if (region != null)
                                    {
                                        ConMuveRot.AlignWithXYPlane(region);
                                        //(Matrix3d translationMatrix, Matrix3d rotationMatrix) = ConMuveRot.MoveAndRotate(region, ConMuveRot.GetRegionRollAngle(region));

                                        //region.TransformBy(translationMatrix);
                                        //region.TransformBy(rotationMatrix);

                                        //List<ObjectId> circleObjectIds = ConCircle.Select(acBlkTabRec, acTra);
                                        //foreach (ObjectId circleObjectId in circleObjectIds)
                                        //{
                                        //    Circle? circle = acTra.GetObject(circleObjectId, OpenMode.ForWrite) as Circle;
                                        //    if (circle != null)
                                        //    {
                                        //        circle.TransformBy(translationMatrix);
                                        //        circle.TransformBy(rotationMatrix);
                                        //    }

                                        //    List<ObjectId> lineObjectIds = ConLine.Select(acBlkTabRec, acTra);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                    acTra.Commit();
                }
            }
        }

        public static void CirclesSet(List<ObjectId> circleObjectIds, double materialTicknes, Database acDb, Transaction acTra)
        {
            Patameter.CNC.Layer.Drill drillLay = new();
            Patameter.CNC.Layer layer = new();

            byte[] colorAttention = layer.ColorAttention;


            foreach (ObjectId circleObjectId in circleObjectIds)
            {
                using Circle? circle = acTra.GetObject(circleObjectId, OpenMode.ForWrite) as Circle;
                if (circle != null)
                {
                    string[] depths = circle.Layer.Split(layer.Separator);
                    if (depths.Length >= 2)
                    {
                        bool ISnummer = double.TryParse(depths[1], out double depth);
                        if (ISnummer)
                        {
                            depth = Math.Abs(depth) * -1;
                            if (drillLay.DepthIsCorrect(depth, materialTicknes, circle.Normal))
                            {
                                string sDrillName = $"{drillLay.Prefix}{depth}{drillLay.Postfix}";
                                if (ConLayer.CheckCreate(sDrillName, acDb, acTra, drillLay.Corol))
                                {
                                    circle.Layer = sDrillName;
                                    circle.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);

                                }
                            }
                            else
                            {
                                circle.Color = Color.FromRgb(colorAttention[0], colorAttention[1], colorAttention[2]);
                            }
                        }
                    }
                    else
                    {
                        if (circle.Layer.Contains(drillLay.Prefix) && circle.Layer.Contains(drillLay.Postfix))
                        {

                        }
                        else
                        {
                            circle.Color = Color.FromRgb(colorAttention[0], colorAttention[1], colorAttention[2]);
                        }
                    }
                }
            }
        }

        public static void LinesSet(List<ObjectId> lineObjectIds, double materialTicknes, Database acDb, Transaction acTra)
        {
            Patameter.CNC.Layer.Milling millingLayer = new();
            Patameter.CNC.Layer layer = new();

            byte[] colorAttention = layer.ColorAttention;

            foreach (ObjectId lineObjectId in lineObjectIds)
            {
                using Line? line = acTra.GetObject(lineObjectId, OpenMode.ForWrite) as Line;
                if (line != null)
                {
                    string[] depths = line.Layer.Split(layer.Separator);
                    if (depths.Length >= 2)
                    {
                        bool IsNummer = double.TryParse(depths[1], out double depth);
                        if (IsNummer)
                        {
                            depth = Math.Abs(depth) * -1;
                            if (millingLayer.DepthIsCorrect(depth, materialTicknes))
                            {
                                string sMillingName = $"{millingLayer.Prefix}{depth}";
                                if (ConLayer.CheckCreate(sMillingName, acDb, acTra, millingLayer.Corol))
                                {
                                    line.Layer = sMillingName;
                                    line.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                                }
                            }
                            else
                            {
                                line.Color = Color.FromRgb(colorAttention[0], colorAttention[1], colorAttention[2]);
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (line.Layer.Contains(millingLayer.Prefix))
                        {

                        }
                        else
                        {
                            line.Color = Color.FromRgb(colorAttention[0], colorAttention[1], colorAttention[2]);
                        }
                    }
                }
            }
        }

        public static double RegionSet(List<ObjectId> regionObjectIds, Database acDb, Transaction acTra)
        {
            Patameter.CNC.Layer.Sheet sheetLay = new();

            double minZ = 0;
            double maxZ = 0;
            foreach (ObjectId regionObjectId in regionObjectIds)
            {
                using Region? regionMinMax = acTra.GetObject(regionObjectId, OpenMode.ForRead) as Region;
                if (regionMinMax != null)
                {
                    if (minZ > regionMinMax.GeometricExtents.MinPoint.Z)
                    {
                        minZ = regionMinMax.GeometricExtents.MinPoint.Z;
                    }
                    if (maxZ < regionMinMax.GeometricExtents.MaxPoint.Z)
                    {
                        maxZ = regionMinMax.GeometricExtents.MaxPoint.Z;
                    }
                }
            }
            double materialTicknes = Math.Round(maxZ - minZ, 2);
            string sLayerName = $"{sheetLay.Prefix}{materialTicknes}";
            bool l = ConLayer.CheckCreate(sLayerName, acDb, acTra, sheetLay.Corol);

            foreach (ObjectId regionObjectId in regionObjectIds)
            {
                using Region? region = acTra.GetObject(regionObjectId, OpenMode.ForWrite) as Region;
                if (region != null && l)
                {
                    region.Layer = sLayerName;
                }
            }

            return materialTicknes;
        }

    }
}
