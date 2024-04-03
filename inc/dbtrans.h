//////////////////////////////////////////////////////////////////////////////
//
//  Copyright 2024 Autodesk, Inc.  All rights reserved.
//
//  Use of this software is subject to the terms of the Autodesk license 
//  agreement provided at the time of installation or download, or which 
//  otherwise accompanies this software in either electronic or hard copy form.   
//
//////////////////////////////////////////////////////////////////////////////
//
// dbtrans.h
//
//  DESCRIPTION: Header for Transaction Modeler.

#ifndef   _ACDBTRANS_H__
#define   _ACDBTRANS_H__

#include "dbmain.h"
#include "dbapserv.h"
#include "AcDbCore2dDefs.h"     // ACDBCORE2D_PORT

#pragma pack (push, 8)

class AcTransaction;
class AcTransactionReactor;

class AcDbTransactionManager: public AcRxObject
{ 
public:
    ACRX_DECLARE_MEMBERS(AcDbTransactionManager);

    virtual AcTransaction*      startTransaction() = 0;
    virtual Acad::ErrorStatus   endTransaction  () = 0;
    virtual Acad::ErrorStatus   abortTransaction() = 0;

    virtual int                 numActiveTransactions() = 0;
/// <summary> Returns the number of objects opened by all transactions /// </summary>
/// <returns> Zero if no objects are opened, otherwise an int value > 0.</returns>
/// <remarks> More efficient than calling getAllObjects() and checking array size.
///           Objects open in multiple transactions may be counted more than once</remarks>
///
    virtual int                 numOpenedObjects() = 0;
    virtual AcTransaction*      topTransaction() = 0;
    virtual Acad::ErrorStatus   addNewlyCreatedDBRObject(AcDbObject* obj,
                                     bool add = true) = 0;

    virtual Acad::ErrorStatus   getAllObjects(AcArray<AcDbObject *> & objs) = 0;

    virtual void                addReactor(AcTransactionReactor* reactor) = 0;
    virtual void                removeReactor(AcTransactionReactor* reactor) =0;

    virtual Acad::ErrorStatus   getObject(AcDbObject*& obj, AcDbObjectId id,
                                          AcDb::OpenMode mode = AcDb::kForRead, 
                                          bool openErasedObj = false) = 0; 

    // Template allowing callers to pass in any pointer type.
    // To help avoid unsafe (AcDbObject * &) casts.
    //
    template<class ObjType> Acad::ErrorStatus getObject(ObjType *& pDerived,
                                AcDbObjectId id, AcDb::OpenMode mode = AcDb::kForRead,
                                bool openErasedObject = false)
    {
        AcDbObject *pObj = nullptr;
        Acad::ErrorStatus es = this->getObject(pObj, id, mode, openErasedObject);
        if (es != Acad::eOk || pObj == nullptr)
            pDerived = nullptr;
        else {
            pDerived = ObjType::cast(pObj);
            // Note that if cast fails, the object is still open in the transaction
            if (pDerived == nullptr)
                es = Acad::eNotThatKindOfClass;
        }
        return es;
    }

    virtual Acad::ErrorStatus   markObjectPageable(AcDbObject* pObject) = 0;
    virtual Acad::ErrorStatus   queueForGraphicsFlush() = 0;

};


class AcTransaction: public AcRxObject
{ 
public:
    ACRX_DECLARE_MEMBERS(AcTransaction);
    virtual 
    Acad::ErrorStatus getObject(AcDbObject*& obj, 
                                AcDbObjectId   objectId, AcDb::OpenMode mode = AcDb::kForRead,
                                bool openErasedObject = false) = 0;

    // Template allowing callers to pass in any pointer type.
    // To help avoid unsafe (AcDbObject * &) casts.
    //
    template<class ObjType> Acad::ErrorStatus getObject(ObjType *& pDerived,
                                AcDbObjectId   objectId, AcDb::OpenMode mode = AcDb::kForRead,
                                bool openErasedObject = false)
    {
        AcDbObject *pObj = nullptr;
        Acad::ErrorStatus es = this->getObject(pObj, objectId, mode, openErasedObject);
        if (es != Acad::eOk || pObj == nullptr)
            pDerived = nullptr;
        else {
            pDerived = ObjType::cast(pObj);
            // Note that if cast fails, the object is still open in the transaction
            if (pDerived == nullptr)
                es = Acad::eNotThatKindOfClass;
        }
        return es;
    }


    virtual 
    Acad::ErrorStatus markObjectPageable(AcDbObject* pObject) = 0;

/// <summary> Returns the number of objects opened by this transactions /// </summary>
/// <returns> Zero if no objects are opened, otherwise an int value > 0.</returns>
/// <remarks> More efficient than calling getAllObjects() and checking array size</remarks>
///
    virtual int                 numOpenedObjects() = 0;
    virtual Acad::ErrorStatus   getAllObjects(AcArray<AcDbObject *> & objs) = 0;
};

class AcTransactionReactor: public AcRxObject
//
// Reactor for transaction management.
//
{
public:
    ACRX_DECLARE_MEMBERS(AcTransactionReactor);
    ACDBCORE2D_PORT virtual ~AcTransactionReactor();

    virtual void transactionAboutToStart (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void transactionStarted      (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void transactionAboutToEnd   (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void transactionEnded        (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void transactionAboutToAbort (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void transactionAborted      (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void endCalledOnOutermostTransaction (int& /*numTransactions*/,
        AcDbTransactionManager* ) {}
    virtual void objectIdSwapped (const AcDbObject* /*pTransResObj*/,
        const AcDbObject* /*pOtherObj*/,
        AcDbTransactionManager* ) {}
};

inline AcDbTransactionManager* acdbTransactionManagerPtr()
{
    return acdbHostApplicationServices()->workingTransactionManager();
}

#define acdbTransactionManager  acdbTransactionManagerPtr()

#pragma pack (pop)

#endif  // _ACDBTRANS_H__
