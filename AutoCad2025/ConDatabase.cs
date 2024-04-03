using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Diagnostics;

[assembly: CommandClass(typeof(AutoCad2025.ConDatabase))]

namespace AutoCad2025
{
    public class ConDatabase
    {
        public static void ObjCopyDbToDb(ObjectIdCollection acObjIdColl, Database acDbCur, Database acDbNew, Transaction acTrans)
        {
            BlockTable? acBlkTblDocNew;
            acBlkTblDocNew = acTrans.GetObject(acDbNew.BlockTableId, OpenMode.ForRead) as BlockTable;
            if (acBlkTblDocNew != null)
            {
                BlockTableRecord? acBlkTblRecNew;
                acBlkTblRecNew = acTrans.GetObject(acBlkTblDocNew[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                if (acBlkTblRecNew != null)
                {
                    IdMapping acIdMap = new();
                    acDbCur.WblockCloneObjects(acObjIdColl, acBlkTblRecNew.ObjectId, acIdMap, DuplicateRecordCloning.Ignore, false);
                }
            }
        }
    }
}
