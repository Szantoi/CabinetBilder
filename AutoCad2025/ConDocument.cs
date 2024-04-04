using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AutoCad2025;


[assembly: CommandClass(typeof(AutoCad2025.ConDocument))]

namespace AutoCad2025
{
    public class ConDocument
    {
        public static void SaveAsActiveDrawing(string strDWGName, Document acDoc)
        {

            object obj = Application.GetSystemVariable("DWGTITLED");

            if (System.Convert.ToInt16(obj) == 0)
            {
                strDWGName = "c:\\MyDrawing.dwg";
            }

            acDoc.Database.SaveAs(strDWGName, true, DwgVersion.Current,
                                  acDoc.Database.SecurityParameters);
        }

        public static void SaveToNowDrawingAndClose(string strDWGName, Database acDbCur, ObjectIdCollection objectIdCollection)
        {

            string strTemplatePath = "acad.dwt";

            DocumentCollection acDocMgrNew = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            Document acDocNew = acDocMgrNew.Add(strTemplatePath);
            Database acDbNew = acDocNew.Database;

            using (DocumentLock acLckDoc = acDocNew.LockDocument())
            {
                using (Transaction acTrans = acDbNew.TransactionManager.StartTransaction())
                {
                    ConDatabase.ObjCopyDbToDb(objectIdCollection, acDbCur, acDbNew, acTrans);
                    acTrans.Commit();
                }
            }
            SaveAsActiveDrawing(strDWGName, acDocNew);
            acDocNew.CloseAndDiscard();
        }
        public static void SaveToNowDrawingAndOpen(string strDWGName, Database acDbCur, ObjectIdCollection objectIdCollection)
        {

            string strTemplatePath = "acad.dwt";

            DocumentCollection acDocMgrNew = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager;

            Document acDocNew = acDocMgrNew.Add(strTemplatePath);
            Database acDbNew = acDocNew.Database;

            using (DocumentLock acLckDoc = acDocNew.LockDocument())
            {
                using (Transaction acTrans = acDbNew.TransactionManager.StartTransaction())
                {
                    ConDatabase.ObjCopyDbToDb(objectIdCollection, acDbCur, acDbNew, acTrans);
                    acTrans.Commit();
                }
            }
            SaveAsActiveDrawing(strDWGName, acDocNew);
        }

        public static void DrawingSaved()
        {
            object obj = Application.GetSystemVariable("DBMOD");

            if (System.Convert.ToInt16(obj) != 0)
            {
                Document acDoc = Application.DocumentManager.MdiActiveDocument;
                acDoc.Database.SaveAs(acDoc.Name, true, DwgVersion.Current, acDoc.Database.SecurityParameters);
            }
        }

    }
}
