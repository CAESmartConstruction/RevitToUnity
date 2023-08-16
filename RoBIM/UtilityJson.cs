using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;


namespace RoBIM
{
    class UtilityJson
    {   
        static public List<OneElement> getJsonFromInsulationArray(Document doc,Element targetElement)
        {
            List<OneElement> oneElementList = new List<OneElement>();
            Options geomOptions = new Options();
            geomOptions.ComputeReferences = true;
            List<Solid> solids = UtilityJson.GetElementSolids(targetElement, geomOptions, false);
            LocationPoint locationPoint = targetElement.Location as LocationPoint;
            Double RemainingTolerance = UnitUtils.Convert(0.03, UnitTypeId.Centimeters, UnitTypeId.Feet);
            ElementId pickedtypeid = targetElement.GetTypeId();
            ElementType family = doc.GetElement(pickedtypeid) as ElementType;
            int Hnumber = family.LookupParameter("Hnumber").AsInteger();
            int Vnumber = family.LookupParameter("Vnumber").AsInteger();
            double Height = family.LookupParameter("Height").AsDouble();
            double Width = family.LookupParameter("Width").AsDouble();
            double InsulationHeight = family.LookupParameter("InsulationHeight").AsDouble();
            double InsulationWidth = family.LookupParameter("InsulationWidth").AsDouble();
            double thick = family.LookupParameter("thick").AsDouble();
            double VRemainHight=family.LookupParameter("VRemainLength").AsDouble();
            double HRemainWidth = family.LookupParameter("HRemainWidth").AsDouble();
            //string material = family.get_Parameter(BuiltInParameter.material).AsString();
            string material = "";
            FamilyInstance familyInstance = targetElement as FamilyInstance;
            Transform transform = familyInstance.GetTransform();

            Transform transform_Inverse = transform.Inverse;
            XYZ local_Point = transform_Inverse.OfPoint(locationPoint.Point);
            XYZ local_LeftLowPointOnFace = local_Point.Subtract(XYZ.BasisX.Multiply(Width / 2)).Add(XYZ.BasisY.Multiply(thick / 2));
            XYZ local_LocationPoint = local_LeftLowPointOnFace.Add(XYZ.BasisX.Multiply(InsulationWidth / 2)).Add(XYZ.BasisZ.Multiply(InsulationHeight / 2));
            for (int h= 0; h< Hnumber; h++){
                for (int v = 0;v < Vnumber; v++)
                {
                    XYZ xTransition = XYZ.BasisX.Multiply(InsulationWidth).Multiply(h);
                    XYZ zTransition = XYZ.BasisZ.Multiply(InsulationHeight).Multiply(v);
                    XYZ elementlocal_LocationPoint= local_LocationPoint.Add(xTransition).Add(zTransition);
                    if (elementlocal_LocationPoint == null) elementlocal_LocationPoint = local_LocationPoint;
                    string elementName = targetElement.Name.ToString();
                    Elements elementsJson = new Elements();
                    
                    Insulation oneElement = new Insulation();
                    oneElement.ElementType = "Generic Model";

                    oneElement.ElementName = elementName;
                    oneElement.Material = material;
                    oneElement.insulationLocation = new InsulationLocation();
                    oneElement.insulationLocation.LocationPoint = transform.OfPoint(elementlocal_LocationPoint);

                    oneElement.insulationSize = new InsulationSize();
                    oneElement.insulationSize.Height = family.LookupParameter("InsulationHeight").AsDouble();
                    oneElement.insulationSize.Width = family.LookupParameter("InsulationWidth").AsDouble();
                    oneElement.insulationSize.Thick = family.LookupParameter("thick").AsDouble();

                    oneElement.productionReference=new ProductionReference();
                    oneElement.productionReference.Position = elementlocal_LocationPoint;
                    oneElement.productionReference.Direction = XYZ.BasisY;
                    oneElement.productionReference.ProductionMethod = ProductionMethod.VacuumGripper.ToString();
                    oneElementList.Add(oneElement);
                }
                
            }
            if (VRemainHight > RemainingTolerance)
            {
                for (int h = 0; h < Hnumber; h++)
                {
                    XYZ xTransition = XYZ.BasisX.Multiply(InsulationWidth ).Multiply(h);
                    XYZ zTransition = XYZ.BasisZ.Multiply(InsulationHeight ).Multiply(Vnumber-1).Add(XYZ.BasisX.Multiply(InsulationHeight / 2 + VRemainHight / 2));
                    XYZ elementlocal_LocationPoint = local_LocationPoint.Add(xTransition).Add(zTransition);
                    string elementName = targetElement.Name.ToString();
                    Elements elementsJson = new Elements();

                    Insulation oneElement = new Insulation();
                    oneElement.ElementType = "Generic Model";

                    oneElement.ElementName = elementName;
                    oneElement.Material = material;
                    oneElement.insulationLocation = new InsulationLocation();
                    oneElement.insulationLocation.LocationPoint = transform.OfPoint(elementlocal_LocationPoint);
                    
                    oneElement.insulationSize = new InsulationSize();
                    oneElement.insulationSize.Height = VRemainHight;
                    oneElement.insulationSize.Width = family.LookupParameter("InsulationWidth").AsDouble();
                    oneElement.insulationSize.Thick = family.LookupParameter("thick").AsDouble();
                    oneElement.productionReference = new ProductionReference();
                    oneElement.productionReference.Position = elementlocal_LocationPoint;
                    oneElement.productionReference.Direction = XYZ.BasisY;
                    oneElement.productionReference.ProductionMethod = ProductionMethod.VacuumGripper.ToString();
                    oneElementList.Add(oneElement);
                }
                    
            }
            if (HRemainWidth > RemainingTolerance)
            {
                for (int v = 0; v < Vnumber; v++)
                {
                    XYZ xTransition = XYZ.BasisZ.Multiply(InsulationWidth).Multiply(Hnumber - 1).Add(XYZ.BasisX.Multiply(InsulationWidth / 2 + HRemainWidth / 2));
                    XYZ zTransition = XYZ.BasisX.Multiply(InsulationHeight).Multiply(v);
                    XYZ elementlocal_LocationPoint = local_LocationPoint.Add(xTransition).Add(zTransition);
                    string elementName = targetElement.Name.ToString();
                    Elements elementsJson = new Elements();

                    Insulation oneElement = new Insulation();
                    oneElement.ElementType = "Generic Model";

                    oneElement.ElementName = elementName;
                    oneElement.Material = material;
                    oneElement.insulationLocation = new InsulationLocation();
                    oneElement.insulationLocation.LocationPoint = transform.OfPoint(elementlocal_LocationPoint);

                    oneElement.insulationSize = new InsulationSize();
                    oneElement.insulationSize.Height = family.LookupParameter("InsulationHeight").AsDouble();
                    oneElement.insulationSize.Width = HRemainWidth;
                    oneElement.insulationSize.Thick = family.LookupParameter("thick").AsDouble();
                    oneElement.productionReference = new ProductionReference();
                    oneElement.productionReference.Position = elementlocal_LocationPoint;
                    oneElement.productionReference.Direction = XYZ.BasisY;
                    oneElement.productionReference.ProductionMethod = ProductionMethod.VacuumGripper.ToString();
                    oneElementList.Add(oneElement);
                }
            }
            if (VRemainHight > RemainingTolerance && HRemainWidth > RemainingTolerance)
            {
                XYZ xTransition = XYZ.BasisX.Multiply(InsulationWidth).Multiply(Hnumber - 1).Add(XYZ.BasisX.Multiply(InsulationWidth / 2 + HRemainWidth / 2));
                XYZ zTransition = XYZ.BasisZ.Multiply(InsulationHeight).Multiply(Vnumber - 1).Add(XYZ.BasisX.Multiply(InsulationHeight / 2 + VRemainHight / 2));
                XYZ elementlocal_LocationPoint = local_LocationPoint.Add(xTransition).Add(zTransition);
                string elementName = targetElement.Name.ToString();
                Elements elementsJson = new Elements();

                Insulation oneElement = new Insulation();
                oneElement.ElementType = "Generic Model";

                oneElement.ElementName = elementName;
                oneElement.Material = material;
                oneElement.insulationLocation = new InsulationLocation();
                oneElement.insulationLocation.LocationPoint = transform.OfPoint(elementlocal_LocationPoint);

                oneElement.insulationSize = new InsulationSize();
                oneElement.insulationSize.Height = VRemainHight;
                oneElement.insulationSize.Width = family.LookupParameter("InsulationWidth").AsDouble();
                oneElement.insulationSize.Thick = family.LookupParameter("thick").AsDouble();
                oneElement.productionReference = new ProductionReference();
                oneElement.productionReference.Position = elementlocal_LocationPoint;
                oneElement.productionReference.Direction = XYZ.BasisY;
                oneElement.productionReference.ProductionMethod = ProductionMethod.VacuumGripper.ToString();
                oneElementList.Add(oneElement);
            }



            return oneElementList;
        }
       
        static public OneElement getJsonFromStructuralFraming(Document doc,Element targetElement)
        {   

            List<XYZ> section = new List<XYZ>();
            List<XYZ> location = new List<XYZ>();
            Options geomOptions = new Options();
            StringBuilder st = new StringBuilder();
            FamilySymbol elementSymbol = null;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_StructuralFraming);
            collector.OfClass(typeof(FamilySymbol));
            foreach (FamilySymbol symbol in collector)
            {
                if (symbol.Name == targetElement.Name)
                //Name 是type name ,FamilyName是FamilyType Name

                {
                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }
                    elementSymbol = symbol;
                }
            }
      
            FamilyInstance familyInstance = targetElement as FamilyInstance;
            


            List<Solid> solids = UtilityJson.GetElementSolids(targetElement, geomOptions, false);
           
            
            Transform transform = familyInstance.GetTransform();
            st.AppendLine("transform.get_Basis(X): (" + transform.BasisX.ToString().ToString() + ")");
            st.AppendLine("transform.get_Basis(Y): (" + transform.BasisY.ToString().ToString() + ")");
            st.AppendLine("transform.get_Basis(Z): (" + transform.BasisZ.ToString().ToString() + ")");
          
            st.AppendLine("transform.get_Basis(0): (" + transform.get_Basis(0).ToString().ToString() + ")");
            st.AppendLine("transform.get_Basis(1): (" + transform.get_Basis(1).ToString().ToString() + ")");
            st.AppendLine("transform.get_Basis(2): (" + transform.get_Basis(2).ToString().ToString() + ")");
            st.AppendLine("transform.Origin: (" + transform.Origin.ToString().ToString() + ")");

            Transform transform_Inverse = transform.Inverse;

            LocationCurve locationcurve = targetElement.Location as LocationCurve;
            
            XYZ direction = (locationcurve.Curve.GetEndPoint(1) - locationcurve.Curve.GetEndPoint(0)).Normalize();
            double Length = (targetElement.get_Parameter(BuiltInParameter.STRUCTURAL_FRAME_CUT_LENGTH).AsDouble());
            double startExtension= (targetElement.get_Parameter(BuiltInParameter.START_EXTENSION).AsDouble());
            double endExtension = (targetElement.get_Parameter(BuiltInParameter.END_EXTENSION).AsDouble());

            ElementId pickedtypeid = targetElement.GetTypeId();
            Element family = doc.GetElement(pickedtypeid);
            double ProductMethodTransition;
            string ProductMethodDirection;
            if (family.LookupParameter("ProductMethodTransition") != null)
                ProductMethodTransition = family.LookupParameter("ProductMethodTransition").AsDouble();
            else
                ProductMethodTransition = 0;

            if (family.LookupParameter("ProductMethodDirection") != null)
                ProductMethodDirection = family.LookupParameter("ProductMethodDirection").AsString();
            else
                ProductMethodDirection = "";
           
            ElementType elementType = targetElement as ElementType;
            string SteelComponentFamilyName = family.Name.ToString();
            
             //MessageBox.Show("Name :" + elementName);
             //1561
             XYZ startPoint = locationcurve.Curve.GetEndPoint(0).Subtract(direction.Multiply(startExtension));
            //MessageBox.Show("startExtension :" + startExtension.ToString());
            XYZ endPoint = startPoint.Add(direction.Multiply(Length));
            //MessageBox.Show("length :" + (Length.ToString()));
            
            double crossSectionRotation = (targetElement.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE).AsDouble());
            //MessageBox.Show("crossSectionRotation :" + crossSectionRotation.ToString());
            Line originLine = Line.CreateUnbound(transform.Origin, direction);
            startPoint = originLine.Project(startPoint).XYZPoint;
            endPoint = originLine.Project(endPoint).XYZPoint;
            XYZ originPoint = (startPoint + endPoint) / 2;
            XYZ localOriginPoint= transform_Inverse.OfPoint(originPoint);
            foreach (Solid solid in solids)
            {


                if (solid != null)
                {

                    foreach (Face geomFace in solid.Faces)
                    {

                        XYZ geomFaceNormal = geomFace.ComputeNormal(UV.Zero);
                        if (geomFaceNormal.DotProduct(direction) == -1)
                        {

                            IList<CurveLoop> curveLooplist = geomFace.GetEdgesAsCurveLoops();
                            foreach (CurveLoop curveLoop in curveLooplist)
                            {
                                XYZ curveDirection;
                                IEnumerator<Curve> curveLoopenum = curveLoop.GetEnumerator();
                                curveLoopenum.MoveNext();
                                curveDirection = (curveLoopenum.Current.GetEndPoint(1) - curveLoopenum.Current.GetEndPoint(0)).Normalize();
                                XYZ glabalpoint = curveLoopenum.Current.GetEndPoint(0);
                                //XYZ localpoint = useConstantTransformAndOrigin(glabalpoint, constTransform,transform.Origin);
                                XYZ localpoint = transform_Inverse.OfPoint(glabalpoint);
                                section.Add(localpoint);
                                
                                while (curveLoopenum.MoveNext())
                                {

                                    XYZ currentCurveDirection = (curveLoopenum.Current.GetEndPoint(1) - curveLoopenum.Current.GetEndPoint(0)).Normalize();
                                    bool changeDirection = (currentCurveDirection.DotProduct(curveDirection) != 1);
                                     glabalpoint = curveLoopenum.Current.GetEndPoint(0);
                                    if (changeDirection)
                                    {   

                                        //localpoint = useConstantTransformAndOrigin(glabalpoint, constTransform, transform.Origin);
                                        localpoint = transform_Inverse.OfPoint(glabalpoint);
                                        section.Add(localpoint);

                                        curveDirection = currentCurveDirection;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            
            //doc.Delete(element.Id);
            

            SteelComponent oneElement = new SteelComponent();
            oneElement.ElementType = "Structural Framing";
            oneElement.ElementName = SteelComponentFamilyName;
            oneElement.SectionOnStart = section;
            oneElement.structuralLocation = new StructuralLocation();
            oneElement.structuralLocation.StartPoint = startPoint;
            oneElement.structuralLocation.EndPoint = endPoint;
            oneElement.CrossSectionRotation = crossSectionRotation;
            oneElement.instanceTransform = new InstanceTransform();
            oneElement.instanceTransform.BasisX = transform.BasisX;
            oneElement.instanceTransform.BasisY = transform.BasisY;
            oneElement.instanceTransform.BasisZ = transform.BasisZ;
            oneElement.instanceTransform.Origin = transform.Origin;
            oneElement.productionReference=new ProductionReference();
            //MessageBox.Show(ProductMethodTransition.ToString());
            oneElement.productionReference.Position = localOriginPoint.Add(enumProductMethodDirectionToXYZ(ProductMethodDirection).Multiply(ProductMethodTransition));
            
            oneElement.productionReference.Direction = enumProductMethodDirectionToXYZ(ProductMethodDirection);
            oneElement.productionReference.ProductionMethod = getProductionMethod_FromSteelComponentFamilyName(SteelComponentFamilyName).ToString();
            return oneElement;

        }
        public enum EnumProductMethodDirection
        {
            nZ,
            pZ,
            nY,
            pY,
            nX,
            pX,
        }
        public enum EnumSteelComponentFamilyName
        {
            endcap,
            PartB,
            PartD_C_Channel,
            
        }
        static public ProductionMethod getProductionMethod_FromSteelComponentFamilyName(string SteelComponentFamilyName)
        {


            if (SteelComponentFamilyName == EnumSteelComponentFamilyName.endcap.ToString())
                return ProductionMethod.VacuumGripper;
            else if (SteelComponentFamilyName == EnumSteelComponentFamilyName.PartB.ToString())
                return ProductionMethod.None;
            else if (SteelComponentFamilyName == EnumSteelComponentFamilyName.PartD_C_Channel.ToString())
                return ProductionMethod.Gripper;
            else if (SteelComponentFamilyName == "typical steel stud")
                return ProductionMethod.Gripper;
            else
                return ProductionMethod.None;





        }
        static public XYZ enumProductMethodDirectionToXYZ(string productMethodDirection)
        {


            if (productMethodDirection == EnumProductMethodDirection.nZ.ToString())
                return new XYZ(0, 0, -1);
            else if (productMethodDirection == EnumProductMethodDirection.pZ.ToString())
                return new XYZ(0, 0, 1);
            else if (productMethodDirection == EnumProductMethodDirection.nY.ToString())
                return new XYZ(0, -1, 0);
            else if (productMethodDirection == EnumProductMethodDirection.pY.ToString())
                return new XYZ(0, 1, 0);
            else if (productMethodDirection == EnumProductMethodDirection.nX.ToString())
                return new XYZ(-1, 0, 0);
            else if (productMethodDirection == EnumProductMethodDirection.pX.ToString())
                return new XYZ(1, 0, 0);
            else
                return new XYZ(0, 0, 0);





        }

        static public OneElement getJsonFromScrew(Element targetElement)
        {
            List<XYZ> location = new List<XYZ>();
            Options geomOptions = new Options();
            geomOptions.ComputeReferences = true;
            List<Solid> solids = UtilityJson.GetElementSolids(targetElement, geomOptions, false);
            Instance instance = targetElement as Instance;

            Transform transform = instance.GetTransform().Inverse;

            LocationPoint screwLocation = targetElement.Location as LocationPoint;
            LocationCurve screwCurve = targetElement.Location as LocationCurve;

            XYZ startPoint = screwCurve.Curve.GetEndPoint(0);
            XYZ endPoint = screwCurve.Curve.GetEndPoint(1);
            //MessageBox.Show("startpoint :" + (startPoint.ToString()));
            //MessageBox.Show("endpoint :" + (endPoint.ToString()));
            double startExtension = (targetElement.get_Parameter(BuiltInParameter.START_EXTENSION).AsDouble());
            double endExtension = (targetElement.get_Parameter(BuiltInParameter.END_EXTENSION).AsDouble());
            //MessageBox.Show(startPoint.ToString());
            XYZ screwdirection = (screwCurve.Curve.GetEndPoint(0) - screwCurve.Curve.GetEndPoint(1)).Normalize();
            //MessageBox.Show("directin :" + (screwdirection.ToString()));

            string elementName = targetElement.Name.ToString();
            //MessageBox.Show("Name :" + elementName);

            //only for model type can use lication.point
            //XYZ screwPlace = screwLocation.Point;
            //MessageBox.Show("point :" + (screwPlace.ToString()));

            double screwlength = 0.0;
            screwlength = targetElement.LookupParameter("#6_Screw_Length").AsDouble();
            double screwlength_in_mm = UnitUtils.Convert(screwlength, DisplayUnitType.DUT_DECIMAL_FEET,DisplayUnitType.DUT_MILLIMETERS);
            //MessageBox.Show("screwlength(in mm)" + (screwlength_in_mm.ToString()));

            ScrewComponent oneElement = new ScrewComponent();
            oneElement.ElementType = "Screw";
            oneElement.ElementName = elementName;
            //刪掉
            //oneElement.screwLocation = new ScrewLocation();
            //oneElement.screwLocation.ScrewPoint = startPoint;
            //oneElement.screwDirection = screwdirection;
            //刪掉

            oneElement.productionReference = new ProductionReference();

            oneElement.screwLength_in_mm = screwlength_in_mm;
            oneElement.productionReference.Position = endPoint;
            oneElement.productionReference.Direction = screwdirection;
            oneElement.productionReference.ProductionMethod = ProductionMethod.Screw.ToString();
            return oneElement;
        }
        static public  XYZ useConstantTransformAndOrigin(XYZ point, Transform transform ,XYZ origin)
        {
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            //transform basis of the old coordinate system in the new coordinate // system
            XYZ b0 = transform.get_Basis(0);
            XYZ b1 = transform.get_Basis(1);
            XYZ b2 = transform.get_Basis(2);
           

            //transform the origin of the old coordinate system in the new 
            //coordinate system
            double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
            double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
            double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

            return new XYZ(xTemp, yTemp, zTemp);
        }
        static public Transform useXYZaxisAndOrigin(XYZ point, Transform transform, XYZ origin)
        {
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            //transform basis of the old coordinate system in the new coordinate // system
            XYZ b0 = XYZ.BasisX;
            XYZ b1 = XYZ.BasisX;
            XYZ b2 = XYZ.BasisX;


            //transform the origin of the old coordinate system in the new 
            //coordinate system
            double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
            double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
            double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

            return transform;
        }
        /// <summary>
        /// Gets solid objects of given element.
        /// </summary>
        /// <param name="elem">Element to retrieve solid geometry.</param>
        /// <param name="opt">geometry option.</param>
        /// <param name="useOriginGeom4FamilyInstance">indicates whether origin geometry of family instance will be used.</param>
        /// <returns>Solids of the geometry.</returns>
		/// <code_owner> autodesk_adn_JimJia </code_owner>
        static public List<Solid> GetElementSolids(Element elem, Options opt = null, bool useOriginGeom4FamilyInstance = false)
        {
            if (null == elem)
            {
                return null;
            }
            if (null == opt)
              opt = new Options();
            List<Solid> solids = new List<Solid>();
            GeometryElement gElem;
            try
            {
                if (useOriginGeom4FamilyInstance && elem is FamilyInstance)
                {
                    // we transform the geometry to instance coordinate to reflect actual geometry 
                    FamilyInstance fInst = elem as FamilyInstance;
                    
                    gElem = fInst.GetOriginalGeometry(opt);
                    Transform trf = fInst.GetTransform();
                    if (!trf.IsIdentity)
                        gElem = gElem.GetTransformed(trf);
                }
                else
                    gElem = elem.get_Geometry(opt);
                if (null == gElem)
                {
                    return null;
                }
                IEnumerator<GeometryObject> gIter = gElem.GetEnumerator();
                gIter.Reset();
                while (gIter.MoveNext())
                {
                    solids.AddRange(getSolids(gIter.Current));
                }
            }
            catch (Exception ex)
            {
                // In Revit, sometime get the geometry will failed.
                string error = ex.Message;
            }
            return solids;
        }

        /// <summary>
        /// Gets all solid objects from geometry object.
        /// </summary>
        /// <param name="gObj">Geometry object from where to get solids. </param>
        /// <returns>The solids of the geometry object. </returns>
		/// <code_owner> autodesk_adn_JimJia </code_owner>
        static public List<Solid> getSolids(GeometryObject gObj)
        {
            List<Solid> solids = new List<Solid>();
            if (gObj is Solid) // already solid
            {
                Solid solid = gObj as Solid;
                if (solid.Faces.Size > 0 && Math.Abs(solid.Volume) > 0) // skip invalid solid
                    solids.Add(gObj as Solid);
            }
            else if (gObj is GeometryInstance) // find solids from GeometryInstance
            {
                IEnumerator<GeometryObject> gIter2 = (gObj as GeometryInstance).GetInstanceGeometry().GetEnumerator();
                gIter2.Reset();
                while (gIter2.MoveNext())
                {
                    solids.AddRange(getSolids(gIter2.Current));
                }
            }
            else if (gObj is GeometryElement) // find solids from GeometryElement
            {
                IEnumerator<GeometryObject> gIter2 = (gObj as GeometryElement).GetEnumerator();
                gIter2.Reset();
                while (gIter2.MoveNext())
                {
                    solids.AddRange(getSolids(gIter2.Current));
                }
            }
            return solids;
        }
        public static IList<Solid> GetTargetSolids(Element element)
        {
            List<Solid> solids = new List<Solid>();


            Options options = new Options();
            options.DetailLevel = ViewDetailLevel.Fine;
            GeometryElement geomElem = element.get_Geometry(options);
            foreach (GeometryObject geomObj in geomElem)
            {
                if (geomObj is Solid)
                {
                    Solid solid = (Solid)geomObj;
                    if (solid.Faces.Size > 0 && solid.Volume > 0.0)
                    {
                        solids.Add(solid);
                    }
                    // Single-level recursive check of instances. If viable solids are more than
                    // one level deep, this example ignores them.
                }
                else if (geomObj is GeometryInstance)
                {
                    GeometryInstance geomInst = (GeometryInstance)geomObj;
                    GeometryElement instGeomElem = geomInst.GetInstanceGeometry();
                    foreach (GeometryObject instGeomObj in instGeomElem)
                    {
                        if (instGeomObj is Solid)
                        {
                            Solid solid = (Solid)instGeomObj;
                            if (solid.Faces.Size > 0 && solid.Volume > 0.0)
                            {
                                solids.Add(solid);
                            }
                        }
                    }
                }
            }
            return solids;
        }

    }
}


