using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using flash.tools.debugger;
using FlashDebugger.Debugger;
using FlashDebugger.Controls;
using System.Collections.Generic;
using System.Collections;

namespace FlashDebugger
{
    public class MemoryDialog : Form
    {
        private Button mExploreButton;
        private TextBox mTextBox;

        private Session mSession;

        private Boolean mCreateDebugStrings;

        private List<MemoryValue> mMemoryValueList;

        /*
        private Dictionary<String, MemoryValue> mDefiningClassList;
        private Dictionary<String, MemoryValue> mNameSpaceList;
        private Dictionary<String, MemoryValue> mQualifiedNameList;
        private Dictionary<String, MemoryValue> mTypeList;
        */

        private ListView mMemoryListView;
        private ColumnHeader defColumn;
        private ColumnHeader nameColumn;
        private ColumnHeader qualNameColumn;
        private ColumnHeader typeColumn;
        private ColumnHeader valueColumn;
        private Dictionary<String, MemoryValue> mValueList;
        private OpenTK.GLControl DataScene;

        private Boolean mSortDir;

        private DataVisualizer mDataVisualizer;

        public MemoryDialog()
        {
            InitializeComponent();
            Initialize3D();
            InitializeValues();
        }

        private void Initialize3D()
        {
            mDataVisualizer = new DataVisualizer(DataScene);
        }

        private void InitializeValues()
        {
            mSortDir = false;
            mCreateDebugStrings = false;
            mMemoryValueList = new List<MemoryValue>();
        }

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent() 
        {
            this.mExploreButton = new System.Windows.Forms.Button();
            this.mTextBox = new System.Windows.Forms.TextBox();
            this.mMemoryListView = new System.Windows.Forms.ListView();
            this.defColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.qualNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.valueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DataScene = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // mExploreButton
            // 
            this.mExploreButton.Location = new System.Drawing.Point(972, 656);
            this.mExploreButton.Name = "mExploreButton";
            this.mExploreButton.Size = new System.Drawing.Size(113, 23);
            this.mExploreButton.TabIndex = 0;
            this.mExploreButton.Text = "Explore Session";
            this.mExploreButton.UseVisualStyleBackColor = true;
            this.mExploreButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // mTextBox
            // 
            this.mTextBox.Location = new System.Drawing.Point(12, 12);
            this.mTextBox.Multiline = true;
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mTextBox.Size = new System.Drawing.Size(310, 310);
            this.mTextBox.TabIndex = 1;
            // 
            // mMemoryListView
            // 
            this.mMemoryListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.defColumn,
            this.nameColumn,
            this.qualNameColumn,
            this.typeColumn,
            this.valueColumn});
            this.mMemoryListView.FullRowSelect = true;
            this.mMemoryListView.GridLines = true;
            this.mMemoryListView.Location = new System.Drawing.Point(328, 12);
            this.mMemoryListView.Name = "mMemoryListView";
            this.mMemoryListView.Size = new System.Drawing.Size(732, 310);
            this.mMemoryListView.TabIndex = 3;
            this.mMemoryListView.UseCompatibleStateImageBehavior = false;
            this.mMemoryListView.View = System.Windows.Forms.View.Details;
            this.mMemoryListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnColumnClick);
            // 
            // defColumn
            // 
            this.defColumn.Text = "DefiningClass";
            this.defColumn.Width = 80;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Name Space";
            this.nameColumn.Width = 100;
            // 
            // qualNameColumn
            // 
            this.qualNameColumn.Text = "Qualified Name";
            this.qualNameColumn.Width = 140;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            this.typeColumn.Width = 160;
            // 
            // valueColumn
            // 
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 100;
            // 
            // DataScene
            // 
            this.DataScene.BackColor = System.Drawing.Color.Black;
            this.DataScene.Location = new System.Drawing.Point(12, 328);
            this.DataScene.Name = "DataScene";
            this.DataScene.Size = new System.Drawing.Size(510, 351);
            this.DataScene.TabIndex = 4;
            this.DataScene.VSync = false;
            this.DataScene.Load += new System.EventHandler(this.DataScene_Load);
            this.DataScene.Paint += new System.Windows.Forms.PaintEventHandler(this.DataScene_Paint);
            this.DataScene.Resize += new System.EventHandler(this.DataScene_Resize);
            // 
            // MemoryDialog
            // 
            this.ClientSize = new System.Drawing.Size(1131, 691);
            this.Controls.Add(this.DataScene);
            this.Controls.Add(this.mMemoryListView);
            this.Controls.Add(this.mTextBox);
            this.Controls.Add(this.mExploreButton);
            this.Name = "MemoryDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        public void SetSession(Session session)
        {
            mSession = session;
        }

        public void ExploreSession()
        {
            int i;
           
            Frame[] frames = mSession.getFrames();

            System.Text.StringBuilder data = new System.Text.StringBuilder();
            String strData = "";

            for (i = 0; i < frames.Length; ++i)
            {
                strData = ExploreThis(frames[i]);
                data.Append(strData);

                strData = ExploreLocals(frames[i]);
                data.Append(strData);
            }

            if (mCreateDebugStrings)
            {
                mTextBox.Text = data.ToString();
            }
            else
            {
                PrintStats();
                PopulateListView();
            }
        }

        private void PopulateListView()
        {
            /*
             curItem = new ListViewItem(serverList[i].name);
                mItems.Add(curItem);

                mItems[i].SubItems.Add(0 + "");
                mItems[i].SubItems.Add(serverList[i].serverURL);
                mItems[i].SubItems.Add(0 + "");
                mItems[i].SubItems.Add(0 + "");
                mItems[i].SubItems.Add(0 + "");
                mItems[i].SubItems.Add(0 + "");

                ServerView.Items.Add(mItems[i]);

            */

            int i;
            ListViewItem curItem;
            MemoryValue curMemItem;

            for(i = 0; i < mMemoryValueList.Count; ++i)
            {
                curMemItem = mMemoryValueList[i];
                curItem = new ListViewItem(curMemItem.pDefiningClass);
                curItem.SubItems.Add(curMemItem.pNameSpace);
                curItem.SubItems.Add(curMemItem.pQualifiedName);
                curItem.SubItems.Add(curMemItem.pType);
                curItem.SubItems.Add(curMemItem.pValue);

                mMemoryListView.Items.Add(curItem);
            }
        }

        private void PrintStats()
        {
            int i;

            try
            {
                Dictionary<String, int> definingClassDict = new Dictionary<string, int>();
                Dictionary<String, int> nameSpaceDict = new Dictionary<string, int>();
                Dictionary<String, int> qualNameDict = new Dictionary<string, int>();
                Dictionary<String, int> typeDict = new Dictionary<string, int>();
                Dictionary<String, int> valDict = new Dictionary<string, int>();

                MemoryValue curVal;
                for (i = 0; i < mMemoryValueList.Count; ++i)
                {
                    curVal = mMemoryValueList[i];

                    if (curVal.pDefiningClass != null && !definingClassDict.ContainsKey(curVal.pDefiningClass))
                    {
                        definingClassDict.Add(curVal.pDefiningClass, 0);
                    }

                    if (curVal.pNameSpace != null && !nameSpaceDict.ContainsKey(curVal.pNameSpace))
                    {
                        nameSpaceDict.Add(curVal.pNameSpace, 0);
                    }

                    if (curVal.pQualifiedName != null && !qualNameDict.ContainsKey(curVal.pQualifiedName))
                    {
                        qualNameDict.Add(curVal.pQualifiedName, 0);
                    }

                    if (curVal.pType != null && !typeDict.ContainsKey(curVal.pType))
                    {
                        typeDict.Add(curVal.pType, 0);
                    }

                    if (curVal.pValue != null && !valDict.ContainsKey(curVal.pValue))
                    {
                        valDict.Add(curVal.pValue, 0);
                    }
                }

                System.Text.StringBuilder stats = new System.Text.StringBuilder();

                stats.Append("Num classes: " + definingClassDict.Count + "\r\n");

                foreach (KeyValuePair<String, int> entry in definingClassDict)
                {
                    // do something with entry.Value or entry.Key
                    stats.Append("  Class: " + entry.Key + "\r\n");
                }

                stats.Append("Num name spaces: " + nameSpaceDict.Count + "\r\n");
                stats.Append("Num Qualified: " + qualNameDict.Count + "\r\n");
                stats.Append("Num Unique types: " + typeDict.Count + "\r\n");

                mTextBox.Text = stats.ToString();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.InnerException.StackTrace);
            }
        }


        private String ExploreThis(Frame curFrame)
        {
            int i;
            int memberCount = 0;

            Variable thisRef;

            Variable curVar;
            thisRef = curFrame.getThis(mSession);

            String data = "";

            if (thisRef != null)
            {
                Value thisValue = thisRef.getValue();


                if (thisValue != null)
                {
                    memberCount = thisValue.getMemberCount(mSession);
                }

                if (memberCount != 0)
                {
                    Variable[] members = thisValue.getMembers(mSession);

                    for (i = 0; i < memberCount; ++i)
                    {
                        curVar = members[i];
                        Value curVal = curVar.getValue();

                        if (curVal != null)
                        {
                            Variable[] nestedMembers = curVal.getMembers(mSession);

                            if (nestedMembers.Length != 0)
                            {
                                data += AppendNestedMembers(nestedMembers);
                            }
                        }

                        long id = -1;
                        if(curVal != null)
                        {
                            id = curVal.getId();
                        }

                        data += AppendInfo(id, curVar, curVal);
                    }
                }
            }

            return data;
        }

        private String ExploreLocals(Frame curFrame)
        {
            int i; 
            
            Variable[] locals;

            Variable curVar;
            locals = curFrame.getLocals(mSession);

            String data = "";

            for (i = 0; i < locals.Length; ++i)
            {
                curVar = locals[i];
                Value curVal = curVar.getValue();

                if(curVal != null)
                {
                    Variable[] nestedMembers = curVal.getMembers(mSession);

                    if (nestedMembers.Length != 0)
                    {
                        data += AppendNestedMembers(nestedMembers);
                    }
                }

                long id = -1;
                if (curVal != null)
                {
                    id = curVal.getId();
                }

                data += AppendInfo(id, curVar, curVal);
            }

            return data;
        }

        private String AppendNestedMembers(Variable[] nestedMembers)
        {
            int i;
            String data = "";
            Variable curNested;
            Value nestedVal;
            long nestedId;

            for (i = 0; i < nestedMembers.Length; ++i)
            {
                curNested = nestedMembers[i];
                nestedVal = curNested.getValue();

                nestedId = -1;

                if (nestedVal != null)
                {
                    nestedId = nestedVal.getId();
                }

                AppendInfo(nestedId, curNested, nestedVal);                
            }

            return data;
        }

        private String AppendInfo(long id, Variable curVar, Value curVal)
        {
            String typeName = "";

            if(curVal != null)
            {
                typeName = curVal.getTypeName();
            }

            String data = "";
            
            if(mCreateDebugStrings)
            {
                data += "id: " + id + " variable: " + curVar.getName() + " value: " + curVar.getValue() + "\r\n";
                data += "  info : \r\n";
                data += "    attributes: " + curVar.getAttributes() + " ,\r\n    definingClass: " + curVar.getDefiningClass() + " ,\r\n    hashCode: " + curVar.GetHashCode() + " ,\r\n    isolatedId: " + curVar.getIsolateId() + " ,\r\n    level: " + curVar.getLevel() + " ,\r\n    namespace: " + curVar.getNamespace() + " ,\r\n    qualifiedName: " + curVar.getQualifiedName() + " ,\r\n    scope: " + curVar.getScope() + " ,\r\n    type: " + typeName + "\r\n";
            }

            String definingClass = curVar.getDefiningClass();
            String nameSpace = curVar.getNamespace();
            String qualName = curVar.getQualifiedName();
            String varType = typeName;
            String val = curVal + "";
            
            /*
            Boolean isNew = true;
            isNew = (isNew && !mDefiningClassList.ContainsKey(definingClass));
            isNew = (isNew && !mNameSpaceList.ContainsKey(nameSpace));
            isNew = (isNew && !mQualifiedNameList.ContainsKey(qualName));
            isNew = (isNew && !mTypeList.ContainsKey(varType));
            isNew = (isNew && !mValueList.ContainsKey(val));

            if(isNew)
            {
              
            */

            AddToLists(id, definingClass, nameSpace, qualName, varType, val);
            
            /*}*/

            return data;
        }

        private void AddToLists(long id, String defClass, String nameSpace, String qualName, String varType, String val)
        {
            int i;

            MemoryValue newMemVal = new MemoryValue(defClass, nameSpace, qualName, varType, val);

            MemoryValue curVal;
            Boolean unique = true;

            for (i = 0; i < mMemoryValueList.Count; ++i)
            {
                curVal = mMemoryValueList[i];

                if(IsEqual(newMemVal, curVal))
                {
                    unique = false;
                    break;
                }
            }


            if(unique)
            {
                mMemoryValueList.Add(newMemVal);
            }

            /*
            mDefiningClassList.Add(defClass, newMemVal);
            mNameSpaceList.Add(nameSpace, newMemVal);
            mQualifiedNameList.Add(qualName, newMemVal);
            mTypeList.Add(varType, newMemVal);
            mValueList.Add(val, newMemVal);
            */
        }

        private Boolean IsEqual(MemoryValue a, MemoryValue b)
        {
            int equalCount = 0;
            int numProperties = 5;

            if (a.pDefiningClass != null && b.pDefiningClass != null && a.pDefiningClass.Equals(b.pDefiningClass))
            {
                equalCount++;
            }

            if (a.pNameSpace != null && b.pNameSpace != null && a.pNameSpace.Equals(b.pNameSpace))
            {
                equalCount++;
            }

            if (a.pQualifiedName != null && b.pQualifiedName != null && a.pQualifiedName.Equals(b.pQualifiedName))
            {
                equalCount++;
            }

            if (a.pType != null && b.pType != null && a.pType.Equals(b.pType))
            {
                equalCount++;
            }

            if (a.pValue != null && b.pValue != null && a.pValue.Equals(b.pValue))
            {
                equalCount++;
            }


            return equalCount == numProperties;
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        private void DialogCloseClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        public void OnColumnClick(Object sender, ColumnClickEventArgs e)
        {
            mSortDir = !mSortDir;
            mMemoryListView.ListViewItemSorter = (IComparer)new MemoryValueItemComparer(e.Column, mSortDir);
            mMemoryListView.Sort();
        }

        /// <summary>
        /// Shows the about dialog
        /// </summary>
        public static new void Show()
        {
            MemoryDialog aboutDialog = new MemoryDialog();
            aboutDialog.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExploreSession();
        }

        #endregion

        private void DataScene_Load(object sender, EventArgs e)
        {
            mDataVisualizer.HandleLoad(e);
        }

        private void DataScene_Paint(object sender, PaintEventArgs e)
        {
            mDataVisualizer.Control_Paint(sender, e);
        }

        private void DataScene_Resize(object sender, EventArgs e)
        {
            mDataVisualizer.Control_Resize(sender, e);
        }
    }
    
}
