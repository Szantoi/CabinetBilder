using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AutoCad2025;

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
                            while (ConBlockReference.Contain(acBlkTabRec, acTra))
                            {
                                List<ObjectId> blocTabRefs = ConBlockReference.Select(acBlkTabRec, acTra);
                                foreach (ObjectId objectId in blocTabRefs)
                                {
                                    ConBlockReference.Explode(objectId, acTra, acBlkTabRec);
                                }
                            }

                            while (ConSolid3D.Contain(acBlkTabRec, acTra))
                            {
                                List<ObjectId> blocTabRefs = ConSolid3D.Select(acBlkTabRec, acTra);
                                foreach (ObjectId objectId in blocTabRefs)
                                {
                                    ConSolid3D.Explode(objectId, acTra, acBlkTabRec);
                                }
                            }
                        }
                    }
                    acTra.Commit();
                }
            }
        }

        [CommandMethod("Cam_Export")]
        public static void Export()
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


                            foreach (ObjectId regionObjectId in regionObjectIds)
                            {
                                ObjectIdCollection surfaceObjectIdCollection = new ObjectIdCollection();

                                Region? region = acTra.GetObject(regionObjectId, OpenMode.ForRead) as Region;
                                if (region != null)
                                {
                                    surfaceObjectIdCollection.Add(regionObjectId);
                                    ed.WriteMessage($"\n{region.Normal}");
                                    foreach(ObjectId circleObjectId in circleObjectIds)
                                    {
                                        Circle? circle = acTra.GetObject(circleObjectId, OpenMode.ForRead) as Circle;
                                        if (circle != null)
                                        {
                                            if(ConRegion.IsPointInsideRegionExtent(circle.Center, region))
                                            {
                                                surfaceObjectIdCollection.Add(circleObjectId);
                                            }
                                        }
                                    }
                                    ConDatabase.ObjCopyDbToDb(surfaceObjectIdCollection, acDbCur, acDbNew, acTrans);

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
