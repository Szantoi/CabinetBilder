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
// CREATED BY: Mei Jiayin                            March 2010
//
// DESCRIPTION: 
//
//////////////////////////////////////////////////////////////////////////////

#pragma once

#include "AcDbAssocArrayParameters.h"
#include "acdbassocactionbody.h"

class AcDbImpAssocArrayCommonParameters;
class AcDbVertexRef;
class AcDbFaceRef;

/// <summary>
/// AcDbAssocArrayCommonParameters class provides interface to manipulate array 
/// parameters that are common to Rectangular, Path and Polar Array. 
/// Its derived classes implement logic for providing array item
/// transforms as well as grip manipulation. 
/// </summary>
///
class  AcDbAssocArrayCommonParameters : public AcDbAssocArrayParameters
{
public:
    ACRX_DECLARE_MEMBERS_EXPIMP(AcDbAssocArrayCommonParameters, ACDBCORE2D_PORT);

    /// <summary> Default destructor.</summary>
    ///
    ACDBCORE2D_PORT ~AcDbAssocArrayCommonParameters(void);

    /// <summary> 
    /// Gets the level count of the array.
    /// If there is no expression for a particular param,
    /// expression string and the evaluatorId are empty strings.
    /// </summary>
    /// <param name="expression">  The returned expression. </param>
    /// <param name="evaluatorId"> The returned evaluatorId. </param>
    /// <returns> Returns level count. </returns>
    ///
    ACDBCORE2D_PORT int levelCount(AcString& expression, AcString& evaluatorId) const;
    ACDBCORE2D_PORT inline int levelCount() const
    {
        AcString expression, evaluatorId;
        return levelCount(expression, evaluatorId);
    }
    ACDBCORE2D_PORT inline int levelCount(AcString& expression) const
    {
        AcString evaluatorId;
        return levelCount(expression, evaluatorId);
    }
    /// <summary> 
    /// Gets the level spacing of the array.
    /// If there is no expression for a particular param,
    /// expression string and the evaluatorId are empty strings.
    /// </summary>
    /// <param name="expression">  The returned expression. </param>
    /// <param name="evaluatorId"> The returned evaluatorId. </param>
    /// <returns> Returns level spacing. </returns>
    ///
    ACDBCORE2D_PORT double levelSpacing(AcString& expression, AcString& evaluatorId) const;
    ACDBCORE2D_PORT inline double levelSpacing() const
    {
        AcString expression, evaluatorId;
        return levelSpacing(expression, evaluatorId);
    }
    ACDBCORE2D_PORT inline double levelSpacing(AcString& expression) const
    {
        AcString evaluatorId;
        return levelSpacing(expression, evaluatorId);
    }
    /// <summary> 
    /// Gets the row count of the array.
    /// If there is no expression for a particular param,
    /// expression string and the evaluatorId are empty strings.
    /// </summary>
    /// <param name="expression">  The returned expression. </param>
    /// <param name="evaluatorId"> The returned evaluatorId. </param>
    /// <returns> Returns row count. </returns>
    ///
    ACDBCORE2D_PORT int rowCount(AcString& expression, AcString& evaluatorId) const;
    ACDBCORE2D_PORT inline int rowCount() const
    {
        AcString expression, evaluatorId;
        return rowCount(expression, evaluatorId);
    }    
    ACDBCORE2D_PORT inline int rowCount(AcString& expression) const
    {
        AcString evaluatorId;
        return rowCount(expression, evaluatorId);
    }    
    /// <summary> 
    /// Gets the row elevation of the array.
    /// If there is no expression for a particular param,
    /// expression string and the evaluatorId are empty strings.
    /// </summary>
    /// <param name="expression">  The returned expression. </param>
    /// <param name="evaluatorId"> The returned evaluatorId. </param>
    /// <returns> Returns row elevation. </returns>
    ///
    ACDBCORE2D_PORT double rowElevation(AcString& expression, AcString& evaluatorId ) const;
    ACDBCORE2D_PORT inline double rowElevation() const
    {
        AcString expression, evaluatorId;
        return rowElevation(expression, evaluatorId);
    }      
    ACDBCORE2D_PORT inline double rowElevation(AcString& expression) const
    {
        AcString evaluatorId;
        return rowElevation(expression, evaluatorId);
    }      
    /// <summary> 
    /// Gets the row spacing of the array.
    /// If there is no expression for a particular param,
    /// expression string and the evaluatorId are empty strings.
    /// </summary>
    /// <param name="expression">  The returned expression. </param>
    /// <param name="evaluatorId"> The returned evaluatorId. </param>
    /// <returns> Returns row spacing. </returns>
    ///
    ACDBCORE2D_PORT double rowSpacing(AcString& expression, AcString& evaluatorId) const;
    ACDBCORE2D_PORT inline double rowSpacing() const
    {
        AcString expression, evaluatorId;
        return rowSpacing(expression, evaluatorId);
    }  
    ACDBCORE2D_PORT inline double rowSpacing(AcString& expression) const
    {
        AcString evaluatorId;
        return rowSpacing(expression, evaluatorId);
    }  
    /// <summary> 
    /// Sets the level count of the array.
    /// If there is no expression for the parameter, the expression string and 
    /// the evaluatorId must be empty strings. 
    /// </summary>
    /// <param name="nLevels">      The level count to set. </param>
    /// <param name="expression">   The expression to set. </param>
    /// <param name="evaluatorId">  The evaluatorId to set. </param>
    /// <param name="errorMessage"> The returned error message for the individual expression. </param>
    /// <returns> Acad::ErrorStatus. </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setLevelCount(int nLevels, const AcString& expression = AcString(), 
        const AcString& evaluatorId = AcString(), AcString& errorMessage = dummyString());
    
    /// <summary> 
    /// Sets the level spacing of the array.
    /// If there is no expression for the parameter, the expression string and 
    /// the evaluatorId must be empty strings. 
    /// </summary>
    /// <param name="offset">       The level spacing to set. </param>
    /// <param name="expression">   The expression to set. </param>
    /// <param name="evaluatorId">  The evaluatorId to set. </param>
    /// <param name="errorMessage"> The returned error message for the individual expression. </param>
    /// <returns> Acad::ErrorStatus. </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setLevelSpacing(double offset, const AcString& expression = AcString(), 
        const AcString& evaluatorId = AcString(), AcString& errorMessage = dummyString());

    /// <summary> 
    /// Sets the row count of the array.
    /// If there is no expression for the parameter, the expression string and 
    /// the evaluatorId must be empty strings. 
    /// </summary>
    /// <param name="nRows">        The row count to set. </param>
    /// <param name="expression">   The expression to set. </param>
    /// <param name="evaluatorId">  The evaluatorId to set. </param>
    /// <param name="errorMessage"> The returned error message for the individual expression. </param>
    /// <returns> Acad::ErrorStatus. </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setRowCount(int nRows, const AcString& expression = AcString(), 
        const AcString& evaluatorId = AcString(), AcString& errorMessage = dummyString());

    /// <summary> 
    /// Sets the row elevation of the array.
    /// If there is no expression for the parameter, the expression string and 
    /// the evaluatorId must be empty strings. 
    /// </summary>
    /// <param name="elevation">    The row elevation to set. </param>
    /// <param name="expression">   The expression to set. </param>
    /// <param name="evaluatorId">  The evaluatorId to set. </param>
    /// <param name="errorMessage"> The returned error message for the individual expression. </param>
    /// <returns> Acad::ErrorStatus. </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setRowElevation(double elevation, const AcString& expression = AcString(), 
        const AcString& evaluatorId = AcString(), AcString& errorMessage = dummyString());

    /// <summary> 
    /// Sets the row spacing of the array.
    /// If there is no expression for the parameter, the expression string and 
    /// the evaluatorId must be empty strings. 
    /// </summary>
    /// <param name="offset">       The row spacing to set. </param>
    /// <param name="expression">   The expression to set. </param>
    /// <param name="evaluatorId">  The evaluatorId to set. </param>
    /// <param name="errorMessage"> The returned error message for the individual expression. </param>
    /// <returns> Acad::ErrorStatus. </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setRowSpacing(double offset, const AcString& expression = AcString(), 
        const AcString& evaluatorId = AcString(), AcString& errorMessage = dummyString());

    /// <summary>
    /// Obtains array base plane geometry definition if there was any base
    /// plane information was set to the parameters. Base plane can be defined
    /// using normal vector or an AcDbFaceRef and base point vertex ref.
    /// </summary>
    /// <param name="basePoint">  The returned base point, default is origin. 
    /// </param>
    /// <param name="normal">  The returned normal to the base plane at the 
    /// base point. The default value is Z-Axis. </param>
    /// <param name="pFace">  The returned base plane geometry, if NULL is 
    /// passed it won't return the face ref. If base plane was not set it
    /// will return empty face ref. </param>
    /// <returns> Acad::ErrorStatus </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus getBasePlane(AcDbVertexRef& basePoint, 
        AcGeVector3d& normal, AcDbFaceRef* pFace = NULL) const;

    /// <summary>
    /// Sets array base plane geometry definition. Base plane is surface or
    /// plane information on which array is laid out.
    /// </summary>
    /// <param name="basePoint">  The input base point geometry. </param>
    /// <param name="normal">  The input normal to define the base plane.
    /// This parameter is ignored if pFace is provided. </param>
    /// <param name="pFace">  The input base plane geometry. If NULL is 
    /// passed the base plane is defined based on input normal. </param>
    /// <returns> Acad::ErrorStatus </returns>
    ///
    ACDBCORE2D_PORT Acad::ErrorStatus setBasePlane(const AcDbVertexRef& basePoint, 
        const AcGeVector3d& normal, const AcDbFaceRef* pFace = NULL);

    /// <summary>
    /// Obtains canonical form of given item locator. The given locator may be
    /// counted wrt start or end, but the returned canonical form is always
    /// counted from start of the array pattern.
    /// </summary>
    /// <param name="locator">  The input item locator or spatial index. </param>
    /// <returns> Canonical form of input locator. </returns>
    ///
    ACDBCORE2D_PORT AcDbItemLocator getCanonicalForm(const AcDbItemLocator& locator) const override;

protected:
    friend class AcDbImpAssocArrayCommonParameters;
    ACDBCORE2D_PORT explicit AcDbAssocArrayCommonParameters(AcDbImpAssocArrayCommonParameters* pSelf);
};
