	/// <summary>
	/// Unity context
	/// </summary>
	public class Context: AS2Context.Context
	{
		public Context(UnitySettings initSettings)
		}
		#endregion
		
		#region classpath management
		/// <summary>
		/// Classpathes & classes cache initialisation
		/// </summary>
		public override void BuildClassPath()
			// external version definition
			
			// Class pathes
			classPath = new List<PathModel>();
			// add external pathes
			List<PathModel> initCP = classPath;
			classPath = new List<PathModel>();
			if (contextSetup.Classpath != null)
			{
			}
			// add user pathes from settings
			{
				foreach(string cpath in settings.UserClasspath) AddPath(cpath.Trim());
			}
			// add initial pathes
			foreach(PathModel mpath in initCP) AddPath(mpath);
			
		}
		/// <summary>
		/// Update Flash intrinsic known vars
		/// </summary>
		protected override void UpdateTopLevelElements()
		{
		    MemberModel special;
		    if (special != null)
		    {
		    }
		    if (special != null) 
		    {
			    if (!extends.IsVoid()) special.Type = extends.QualifiedName;
		    }
		}
		
		/// <summary>
		/// Prepare JS intrinsic known vars/methods/classes
		/// </summary>
		protected override void InitTopLevelElements()
		{
			// not found
			else
			{
			}
			topLevel.Members.Sort();
		}
		#endregion
		
		#region command line compiler
		/// <summary>
		/// Run MTASC compiler in the current class's base folder with current classpath
		/// </summary>
		/// <param name="append">Additional comiler switches</param>
		public override void RunCMD(string append)
		{
				return;
			{
				return;
			}
			
			SetStatusText(settings.CheckSyntaxRunning);
			
			try 
			{
				// save modified files if needed
				if (outputFile != null) MainForm.CallCommand("SaveAllModified", null);
				else MainForm.CallCommand("SaveAllModified", ".as");
				
				// prepare command
				if (append == null || append.IndexOf("-swf-version") < 0)
				// classpathes
				foreach(PathModel aPath in classPath)
				
				// run
				MainForm.CallCommand("RunProcessCaptured", command+" "+append);
			}
			catch (Exception ex)
			{
			}*/
		}
		
		/// <summary>
		/// Calls RunCMD with additional parameters taken from the classes @mtasc doc tag
		/// </summary>
		public override bool BuildCMD(bool failSilently)
		{
			// check if @mtasc is defined
			Match mCmd = null;
			
			if (CurrentModel.Version != 2 || mCmd == null || !mCmd.Success) 
			{
				if (!failSilently)
				{
					MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidForQuickBuild"));
				}
				return false;
			}
			
			// build command
			string command = mCmd.Groups["params"].Value.Trim();
			
			// run
			RunCMD(command);
			return true;*/
	}