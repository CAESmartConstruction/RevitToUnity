using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace RoBIM
{
    public class Elements
    {
        public List<OneElement> StructuralFramingList
        {
            get;
            set;
        }
        public List<OneElement> InsulationList
        {
            get;
            set;
        }
        public List<OneElement> ScrewList
        {
            get;
            set;
        }
    }
}
