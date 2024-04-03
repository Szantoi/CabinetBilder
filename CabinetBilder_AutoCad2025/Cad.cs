using AutoCad2025;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ViewModel.PointCloudManager;
using CabinetBilder_UI;
using System.Collections.Generic;
using System.Security.Policy;

[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.Cad))]
namespace CabinetBilder_AutoCad2025
{
    internal class Cad
    {
        [CommandMethod("ExportBlocks")]
        public static void ExportBlocks()
        {
            string exportFolder = PopUp.OpenFolderDialog();

            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDbCur = acDocMgrCur.Database;
            Editor ed = acDocMgrCur.Editor;

            using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
            using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();
            if (acTraCor != null)
            {
                BlockTable? acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (acBlkTblCor != null)
                {
                    BlockTableRecord? acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                    if (acBlkTblRecCur != null)
                    {
                        List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, acTraCor);
                        ed.WriteMessage($"\n{blockRefIds.Count}");

                        ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
                    }
                }
                acTraCor.Commit();
            }
        }

        [CommandMethod("ExportBlocksWoodComponent")]
        public static void ExportBlocksWoodComponent()
        {
            string exportFolder = PopUp.OpenFolderDialog();
            string layerName = "WoodComponent";

            Document acDocMgrCur = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (acDocMgrCur != null)
            {
                Database acDbCur = acDocMgrCur.Database;
                if (acDbCur != null)
                {
                    Editor ed = acDocMgrCur.Editor;

                    using DocumentLock acLockDocCur = acDocMgrCur.LockDocument();
                    using Transaction acTraCor = acDbCur.TransactionManager.StartTransaction();
                    if (acTraCor != null)
                    {
                        BlockTable? acBlkTblCor = acTraCor.GetObject(acDbCur.BlockTableId, OpenMode.ForRead) as BlockTable;
                        if (acBlkTblCor != null)
                        {
                            BlockTableRecord? acBlkTblRecCur = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                            if (acBlkTblRecCur != null)
                            {
                                LayerTable? lt = acTraCor.GetObject(acDbCur.LayerTableId, OpenMode.ForRead) as LayerTable;
                                if (lt != null)
                                {
                                    List<ObjectId> blockRefIds = ConBlockReference.Select(acBlkTblRecCur, layerName, lt, acTraCor);
                                    ed.WriteMessage($"\n{blockRefIds.Count}");

                                    ConBlockReference.BlocksToDwgs(blockRefIds, exportFolder, acDbCur, acTraCor);
                                }
                            }
                        }
                        acTraCor.Commit();
                    }
                }
            }
        }
    }
}

