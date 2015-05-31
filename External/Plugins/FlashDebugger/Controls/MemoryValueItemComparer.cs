using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlashDebugger.Controls
{
    //C#
    // Implements the manual sorting of items by column.
    class MemoryValueItemComparer : IComparer
    {
        private Boolean mSortDir;
        private int mColumn;
        public MemoryValueItemComparer()
        {
            mColumn = 0;
        }

        public MemoryValueItemComparer(int column, Boolean sortDir)
        {
            mSortDir = sortDir;
            mColumn = column;
        }

        public int Compare(object x, object y)
        {
            int returnVal = -1;

            if (mSortDir)
            {
                returnVal = String.Compare(((ListViewItem)x).SubItems[mColumn].Text,
                ((ListViewItem)y).SubItems[mColumn].Text);
            }
            else
            {
                returnVal = String.Compare(((ListViewItem)y).SubItems[mColumn].Text,
                ((ListViewItem)x).SubItems[mColumn].Text);
            }

            return returnVal;
        }
    }
}
