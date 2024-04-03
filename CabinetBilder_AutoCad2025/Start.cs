using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


[assembly: CommandClass(typeof(CabinetBilder_AutoCad2025.Start))]
namespace CabinetBilder_AutoCad2025
{
    public class Start
    {
        [CommandMethod("Hello")]
        public static void Hello()
        {
            Document acDocCor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acDbCor = acDocCor.Database;

            ObjectIdCollection acObjIdColl = new ObjectIdCollection();

            using Transaction acTraCor = acDbCor.TransactionManager.StartTransaction();

            BlockTable acBlkTblCor = acTraCor.GetObject(acDbCor.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord msBlkRecCor = acTraCor.GetObject(acBlkTblCor[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            Editor ed = acDocCor.Editor;

            ed.WriteMessage("Hello World ... ");
        }
    }
}
