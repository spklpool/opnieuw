using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
namespace Opnieuw.Addin
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2
	{
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            object _localToolWindowControlObject = null;

            //This guid must be unique for each different tool window,
            // but you may use the same guid for the same tool window.
            //This guid can be used for indexing the windows collection,
            // for example: applicationObject.Windows.Item(guidstr)
            String guidstr = "{DF0E627D-E830-4f8e-8F4E-89E6B9CF04FC}";
            EnvDTE80.Windows2 windows2 = (EnvDTE80.Windows2)_applicationObject.Windows;
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            _windowToolWindow = windows2.CreateToolWindow2(_addInInstance, asm.Location, "Opnieuw.Addin.TestUserControl", "Opnieuw Tool window", guidstr, ref _localToolWindowControlObject);
            _windowToolControl = (TestUserControl)_localToolWindowControlObject;

            OutputWindow outputWindow = (OutputWindow)_applicationObject.Windows.Item(Constants.vsWindowKindOutput).Object;
            _outputWindowPane = outputWindow.OutputWindowPanes.Add("DTE Event Information - C# Event Watcher");

            //Set the picture displayed when the window is tab docked
            //_windowToolWindow.SetTabPicture(Resource.ToolWindowBitmap.GetHbitmap());

            EnvDTE.Events events = _applicationObject.Events;
            _documentEvents = (EnvDTE.DocumentEvents)events.get_DocumentEvents(null);
            _documentEvents.DocumentClosing += new _dispDocumentEvents_DocumentClosingEventHandler(this.DocumentClosing);
            _documentEvents.DocumentOpened += new _dispDocumentEvents_DocumentOpenedEventHandler(this.DocumentOpened);
            _documentEvents.DocumentOpening += new _dispDocumentEvents_DocumentOpeningEventHandler(this.DocumentOpening);
            _documentEvents.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(this.DocumentSaved);

            _solutionEvents = (EnvDTE.SolutionEvents)events.SolutionEvents;
            _solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(this.AfterClosing);
            _solutionEvents.BeforeClosing += new _dispSolutionEvents_BeforeClosingEventHandler(this.BeforeClosing);
            _solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(this.Opened);
            _solutionEvents.ProjectAdded += new _dispSolutionEvents_ProjectAddedEventHandler(this.ProjectAdded);
            _solutionEvents.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler(this.ProjectRemoved);
            _solutionEvents.ProjectRenamed += new _dispSolutionEvents_ProjectRenamedEventHandler(this.ProjectRenamed);
            _solutionEvents.QueryCloseSolution += new _dispSolutionEvents_QueryCloseSolutionEventHandler(this.QueryCloseSolution);
            _solutionEvents.Renamed += new _dispSolutionEvents_RenamedEventHandler(this.Renamed);

            //When using the hosting control, you must set visible to true before calling HostUserControl,
            // otherwise the UserControl cannot be hosted properly.
            _windowToolWindow.Visible = true;
        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            if (_documentEvents != null) {
                _documentEvents.DocumentClosing -= new _dispDocumentEvents_DocumentClosingEventHandler(this.DocumentClosing);
                _documentEvents.DocumentOpened -= new _dispDocumentEvents_DocumentOpenedEventHandler(this.DocumentOpened);
                _documentEvents.DocumentOpening -= new _dispDocumentEvents_DocumentOpeningEventHandler(this.DocumentOpening);
                _documentEvents.DocumentSaved -= new _dispDocumentEvents_DocumentSavedEventHandler(this.DocumentSaved);
            }

            if (_solutionEvents != null) {
                _solutionEvents.AfterClosing -= new _dispSolutionEvents_AfterClosingEventHandler(this.AfterClosing);
                _solutionEvents.BeforeClosing -= new _dispSolutionEvents_BeforeClosingEventHandler(this.BeforeClosing);
                _solutionEvents.Opened -= new _dispSolutionEvents_OpenedEventHandler(this.Opened);
                _solutionEvents.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler(this.ProjectAdded);
                _solutionEvents.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler(this.ProjectRemoved);
                _solutionEvents.ProjectRenamed -= new _dispSolutionEvents_ProjectRenamedEventHandler(this.ProjectRenamed);
                _solutionEvents.QueryCloseSolution -= new _dispSolutionEvents_QueryCloseSolutionEventHandler(this.QueryCloseSolution);
                _solutionEvents.Renamed -= new _dispSolutionEvents_RenamedEventHandler(this.Renamed);
            }
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}

        //DocumentEvents
        public void DocumentClosing(EnvDTE.Document document)
        {
            //System.Windows.Forms.MessageBox.Show("Document closing: " + document.Path + document.Name);
        }

        public void DocumentOpened(EnvDTE.Document document)
        {
            //System.Windows.Forms.MessageBox.Show("Document opened: " + document.Path + document.Name);
        }

        public void DocumentOpening(string documentPath, bool ReadOnly)
        {
            //String[] fileNames = {documentPath};
            //_windowToolControl.PopulateTree(fileNames);
        }

        public void DocumentSaved(EnvDTE.Document document)
        {
            //System.Windows.Forms.MessageBox.Show("Document saved: " + document.Path + document.Name);
        }

        //SolutionEvents
        public void AfterClosing()
        {
//            _outputWindowPane.OutputString("SolutionEvents, AfterClosing\n");
        }

        public void BeforeClosing()
        {
//            _outputWindowPane.OutputString("SolutionEvents, BeforeClosing\n");
        }

        public void Opened()
        {
            foreach (EnvDTE.Project project in _applicationObject.Solution.Projects) {
                foreach (EnvDTE.ProjectItem projectItem in project.ProjectItems) {
                    for (short i = 0; i < projectItem.FileCount; i++) {
                        String fileName = projectItem.get_FileNames(i);
                        if (fileName.EndsWith(".cs")) {
                            String[] fileNames = new String[1];
                            fileNames[0] = fileName;
                            _windowToolControl.PopulateTree(fileNames);
                        }
                    }
                }
            }
        }

        public void ProjectAdded(EnvDTE.Project project)
        {
//            _outputWindowPane.OutputString("SolutionEvents, ProjectAdded\n");
//            _outputWindowPane.OutputString("\tProject: " + project.UniqueName + "\n");
        }

        public void ProjectRemoved(EnvDTE.Project project)
        {
//            _outputWindowPane.OutputString("SolutionEvents, ProjectRemoved\n");
//            _outputWindowPane.OutputString("\tProject: " + project.UniqueName + "\n");
        }

        public void ProjectRenamed(EnvDTE.Project project, string oldName)
        {
//            _outputWindowPane.OutputString("SolutionEvents, ProjectRenamed\n");
//            _outputWindowPane.OutputString("\tProject: " + project.UniqueName + "\n");
        }

        public void QueryCloseSolution(ref bool cancel)
        {
//            _outputWindowPane.OutputString("SolutionEvents, QueryCloseSolution\n");
        }

        public void Renamed(string oldName)
        {
//            _outputWindowPane.OutputString("SolutionEvents, Renamed\n");
        }

        private OutputWindowPane _outputWindowPane;
        private EnvDTE.DocumentEvents _documentEvents;
        private EnvDTE.SolutionEvents _solutionEvents;
		private DTE2 _applicationObject;
        private AddIn _addInInstance;
        private Window _windowToolWindow;
        private TestUserControl _windowToolControl;
	}
}