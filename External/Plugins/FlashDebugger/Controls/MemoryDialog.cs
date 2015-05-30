using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using flash.tools.debugger;
using FlashDebugger.Debugger;
using System.Collections.Generic;

namespace FlashDebugger
{
    public class MemoryDialog : Form
    {
        private Button mExploreButton;
        private TextBox mTextBox;

        private Session mSession;

        private Boolean mCreateDebugStrings;

        private List<MemoryValue> mMemoryValueList;

        private Dictionary<String, MemoryValue> mDefiningClassList;
        private Dictionary<String, MemoryValue> mNameSpaceList;
        private Dictionary<String, MemoryValue> mQualifiedNameList;
        private Dictionary<String, MemoryValue> mTypeList;
        private Dictionary<String, MemoryValue> mValueList;

        public MemoryDialog()
        {
            InitializeComponent();
            InitializeValues();
        }

        private void InitializeValues()
        {
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
            this.SuspendLayout();
            // 
            // mExploreButton
            // 
            this.mExploreButton.Location = new System.Drawing.Point(346, 388);
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
            this.mTextBox.Size = new System.Drawing.Size(447, 310);
            this.mTextBox.TabIndex = 1;
            // 
            // MemoryDialog
            // 
            this.ClientSize = new System.Drawing.Size(471, 423);
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
            int j;
            
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

            Boolean isNew = true;
            String definingClass = curVar.getDefiningClass();
            String nameSpace = curVar.getNamespace();
            String qualName = curVar.getQualifiedName();
            String varType = typeName;
            String val = curVal + "";

            /*
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
            MemoryValue newMemVal = new MemoryValue(defClass, nameSpace, qualName, varType, val);
            
            mMemoryValueList.Add(newMemVal);

            /*
            mDefiningClassList.Add(defClass, newMemVal);
            mNameSpaceList.Add(nameSpace, newMemVal);
            mQualifiedNameList.Add(qualName, newMemVal);
            mTypeList.Add(varType, newMemVal);
            mValueList.Add(val, newMemVal);
            */
        }

        /// <summary>
        /// Closes the about dialog
        /// </summary>
        private void DialogCloseClick(Object sender, EventArgs e)
        {
            this.Close();
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
    }
    
}
