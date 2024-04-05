using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography;
using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;

[assembly: CommandClass(typeof(AutoCad2025.ConLayer))]

namespace AutoCad2025
{
    public class ConLayer
    {
        public static bool Check(string sLayerName, Database acCurDb, Transaction acTrans)
        {
            bool l = false;

            using LayerTable? acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (acLyrTbl == null)
            {
                if (acLyrTbl.Has(sLayerName) == false)
                {
                    l = true;
                    return l;
                }
            }
            return l;

        }

        public static bool CheckCreate(string sLayerName, Database acCurDb, Transaction acTrans, byte[] rgb)
        {
            bool l = false;
            using LayerTable? acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (acLyrTbl != null)
            {
                if (acLyrTbl.Has(sLayerName) == false)
                {
                    using (LayerTableRecord acLyrTblRec = new LayerTableRecord())
                    {
                        acLyrTblRec.Color = Color.FromRgb(rgb[0], rgb[1], rgb[2]);
                        acLyrTblRec.Name = sLayerName;

                        acTrans.GetObject(acCurDb.LayerTableId, OpenMode.ForWrite);

                        acLyrTbl.Add(acLyrTblRec);
                        acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                        l = true;
                    }
                }
                else
                {
                    l = true;
                }
            }
            return l;

        }
    }
}
