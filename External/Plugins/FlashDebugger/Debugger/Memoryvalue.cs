using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using flash.tools.debugger;

namespace FlashDebugger.Debugger
{
    class MemoryValue
    {
        public String pDefiningClass;
        public String pNameSpace;
        public String pQualifiedName;
        public String pType;
        public String pValue;

        public MemoryValue(String defClass, String nameSpace, String qualName, String varType, String val)
        {
            pDefiningClass = defClass;
            pNameSpace = nameSpace;
            pQualifiedName = qualName;
            pType = varType;
            pValue = val;
        }
    }
}
