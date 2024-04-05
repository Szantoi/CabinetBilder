using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AutoCad2025;
using CabinetBilder_UI;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.Cam))]
namespace CabinetBilder_AutoCad2025
{
    internal class Cam
    {
        [CommandMethod("Cam_Preparation")]
        public static void Preparation()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

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
                            int cycle = 0;
                            while (ConBlockReference.Contain(acBlkTabRec, acTra) && cycle <= 5)
                            {
                                List<ObjectId> blocTabRefs = ConBlockReference.Select(acBlkTabRec, acTra);
                                foreach (ObjectId objectId in blocTabRefs)
                                {
                                    ConBlockReference.Explode(objectId, acTra, acBlkTabRec);
                                }
                            }
                            cycle = 0;
                            while (ConSolid3D.Contain(acBlkTabRec, acTra) && cycle <= 5)
                            {
                                List<ObjectId> blocTabRefs = ConSolid3D.Select(acBlkTabRec, acTra);
                                foreach (ObjectId objectId in blocTabRefs)
                                {
                                    ConSolid3D.Explode(objectId, acTra, acBlkTabRec);
                                }
                            }

                            double materialTicknes = 0;
                            List<ObjectId> regionObjectIds = ConRegion.Select(acBlkTabRec, acTra);
                            if (regionObjectIds != null)
                            {
                                materialTicknes = Cnc.RegionSet(regionObjectIds, acDb, acTra);

                                List<ObjectId> circleObjectIds = ConCircle.Select(acBlkTabRec, acTra);
                                if (circleObjectIds != null && materialTicknes > 0)
                                {
                                    Cnc.CirclesSet(circleObjectIds, materialTicknes, acDb, acTra);
                                }

                                List<ObjectId> lineObjectIds = ConLine.Select(acBlkTabRec, acTra);
                                if (lineObjectIds != null && materialTicknes > 0)
                                {
                                    Cnc.LinesSet(lineObjectIds, materialTicknes, acDb, acTra);
                                }

                            }
                        }
                    }
                    acTra.Commit();
                }
            }
        }

        [CommandMethod("Cam_In_Region")]
        public static void Cam_InRegion()
        {
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;

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
                                materialTicknes = Cnc.RegionSet(regionObjectIds, acDb, acTra);

                                List<ObjectId> circleObjectIds = ConCircle.Select(acBlkTabRec, acTra);
                                if (circleObjectIds != null && materialTicknes > 0)
                                {
                                    List<ObjectId> lineObjectIds = ConLine.Select(acBlkTabRec, acTra);
                                    if (lineObjectIds != null && materialTicknes > 0)
                                    {
                                        foreach (ObjectId regionObjectId in regionObjectIds)
                                        {
                                            ObjectIdCollection surfaceObjectIdCollection = new();

                                            Region? region = acTra.GetObject(regionObjectId, OpenMode.ForWrite) as Region;
                                            if (region != null)
                                            {
                                                ConRegion.InCircleColor(circleObjectIds, region, acTra);
                                                ConRegion.InLineColor(lineObjectIds, region, acTra);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    acTra.Commit();
                }
            }
        }
        [CommandMethod("Cam_Set_ALL_ByLayer")]
        public static void CamSetALLByLayer()
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
                            int Quantity = ConBlockTableRecord.setAllEtityColorBayLayer(acBlkTabRec, acTra);
                            ed.WriteMessage($"\nQuantity: {Quantity}");
                        }
                    }
                    acTra.Commit();
                }
            }
        }

        [CommandMethod("Cam_Export")]
        public static void Export()
        {
            string exportFolder = PopUp.OpenFolderDialog();
            int drawCount = 0;

            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDb = acDoc.Database;
            Editor ed = acDoc.Editor;

            using DocumentLock acLockDoc = acDoc.LockDocument();
            if (acDb != null)
            {
                ed.WriteMessage($"\n{exportFolder}");
                string docName = System.IO.Path.GetFileNameWithoutExtension(acDoc.Name);
                string namePrefix = $"{docName}";

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
                                materialTicknes = Cnc.RegionSet(regionObjectIds, acDb, acTra);

                                List<ObjectId> circleObjectIds = ConCircle.Select(acBlkTabRec, acTra);
                                if (circleObjectIds != null && materialTicknes > 0)
                                {
                                    Cnc.CirclesSet(circleObjectIds, materialTicknes, acDb, acTra);
                                }

                                List<ObjectId> lineObjectIds = ConLine.Select(acBlkTabRec, acTra);
                                if (lineObjectIds != null && materialTicknes > 0)
                                {
                                    Cnc.LinesSet(lineObjectIds, materialTicknes, acDb, acTra);
                                }

                                foreach (ObjectId regionObjectId in regionObjectIds)
                                {
                                    ObjectIdCollection surfaceObjectIdCollection = new();

                                    Region? region = acTra.GetObject(regionObjectId, OpenMode.ForWrite) as Region;
                                    if (region != null)
                                    {
                                        (string name, string namePostfix, string RegionNorlamV) = Name.SurfaceFileName(region, ++drawCount);
                                        ed.WriteMessage($"\n{name}_{namePostfix}");

                                        surfaceObjectIdCollection.Add(regionObjectId);

                                        int count = 0;
                                        count = ConRegion.InCircle(circleObjectIds, surfaceObjectIdCollection, region, acTra);
                                        ed.WriteMessage($"\nIN circle: {count}");

                                        count = 0;
                                        count = ConRegion.InLine(lineObjectIds, surfaceObjectIdCollection, region, acTra);
                                        ed.WriteMessage($"\nIN line: {count}");

                                        string exportFileName = System.IO.Path.Combine(exportFolder, $"{namePrefix}_{name}_{namePostfix}.dwg");

                                        ConDocument.SaveToNowDrawingAndClose(exportFileName, acDb, surfaceObjectIdCollection);
                                    }
                                }
                            }
                        }
                    }
                    acTra.Commit();
                }
            }

        }

    }
}
