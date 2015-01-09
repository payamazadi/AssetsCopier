using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Permissions;
using System.ServiceProcess;

namespace AssetsCopier
{
	public partial class AssetsCopier : ServiceBase
	{
		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
		private Dictionary<string, DateTime> _changedFileTimes = new Dictionary<string, DateTime>(); 
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		public AssetsCopier()
		{
			InitializeComponent();
			eventLog1 = new System.Diagnostics.EventLog {Source = "AssetsCopier"};
		}

		public void Begin()
		{
			OnStart(null);
		}

		protected override void OnStart(string[] args)
		{
			LogIt("Starting service.");

			try
			{
				var folders = ConfigurationManager.GetSection("watch") as FoldersSection;

				if (folders == null || folders.Directories.Count == 0)
				{
					LogIt("Must specify a list of directories in folders AppSetting");
					Stop();
					return;
				}

				foreach (var folder in folders.Directories)
				{
					WatchPath(folder as DirectorySetting);
				}
			}
			catch (Exception e)
			{
				LogIt("Failure - " + e);
			}
		}


		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private void WatchPath(DirectorySetting directory)
		{
			try
			{
				LogIt(String.Format("watching Path: '{0}' : '{1}'", directory.Path, directory.Filter));

				if (!String.IsNullOrWhiteSpace(directory.Filter))
				{
					var filters = directory.Filter.Split('|');
					foreach (var filter in filters)
					{
						CreateWatcher(directory.Path, filter, directory.Prepend, directory.DestinationRoot);
					}
				}
				else
				{
					CreateWatcher(directory.Path, String.Empty, directory.Prepend, directory.DestinationRoot);
				}
			}

			catch (Exception e)
			{
				LogIt(String.Format("Unable to watch directory: '{0}', ", directory.Path) + e);
				Stop();
			}
		}

		private void CreateWatcher(string path, String filter, string prepend, string destinationRoot)
		{
			try
			{
				var watcher = new FileSystemWatcherWithDestination(path, destinationRoot);

				if (!String.IsNullOrWhiteSpace(filter))
				{
					watcher.Filter = filter;
				}

				watcher.IncludeSubdirectories = true;
				watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite |
															 NotifyFilters.Size | NotifyFilters.DirectoryName;

				watcher.Changed += (sender, args) => OnChanged(sender, args, prepend);
				watcher.Created += (sender, args) => OnChanged(sender, args, prepend);
				watcher.Deleted += (sender, args) => OnChanged(sender, args, prepend);
				watcher.Renamed += (sender, args) => OnChanged(sender, args, prepend);

				watcher.Error += (sender, eventArgs) =>
												 LogIt((String.Format("Error watching directory '{0}', ", path) + eventArgs.GetException()));

				watcher.EnableRaisingEvents = true;

				_watchers.Add(watcher);
			}
			catch (Exception e)
			{
				LogIt(String.Format("Unable to create watcher for path '{0}' with filter '{1}'", path, filter) + e);
				Stop();
			}
		}

		private void OnChanged(object sender, FileSystemEventArgs e, string prepend)
		{
			var watcher = sender as FileSystemWatcherWithDestination;
			if (watcher == null) return;

			LogIt("file " + e.FullPath + " may have changed with event " + e.ChangeType);

			DateTime lastWriteTime = DateTime.MinValue;
			DateTime lastChange = File.GetLastWriteTime(e.FullPath);

			if (_changedFileTimes.ContainsKey(e.FullPath))
				lastWriteTime = _changedFileTimes[e.FullPath];
			else _changedFileTimes.Add(e.FullPath, DateTime.MinValue);

			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Created:
				case WatcherChangeTypes.Changed:
					if (lastWriteTime != lastChange)
					{
						LogIt("Attempting to copy " + e.FullPath + " to " + watcher.DestinationRoot + "\\" +
									e.Name);

						MimicStructure(e.FullPath, prepend, watcher.DestinationRoot);
						File.Copy(e.FullPath, watcher.DestinationRoot + "\\" + e.Name, true);
					}
					else
					{
						eventLog1.WriteEntry("Ignored event " + e.ChangeType + " for changed file " + e.FullPath);
					}
					break;
				case WatcherChangeTypes.Deleted:
					LogIt("Attempting to delete " + e.FullPath + " from " + watcher.DestinationRoot + "\\" + e.Name);
					File.Delete(watcher.DestinationRoot + "\\" + e.Name);
					break;
				case WatcherChangeTypes.Renamed:
			    var f = (RenamedEventArgs) e;
				  LogIt("Need to rename.." + f.OldFullPath + ", " + f.FullPath);
          File.Delete(watcher.DestinationRoot + "\\" + f.OldName);
          File.Copy(e.FullPath, watcher.DestinationRoot + "\\" + e.Name, true);
					break;
					// ReSharper disable RedundantEmptyDefaultSwitchBranch
				default:
					break;
					// ReSharper restore RedundantEmptyDefaultSwitchBranch
			}
			_changedFileTimes[e.FullPath] = lastChange;
		}

		static void MimicStructure(string source, string prepend, string destinationRoot)
		{
			if (source.Equals(prepend))
				return;

			string furthest = source.Substring(0, source.LastIndexOf("\\", StringComparison.Ordinal));
			string destination = furthest.Replace(prepend, destinationRoot);

			if (Directory.Exists(destination))
				return;
			MimicStructure(furthest.Substring(0, furthest.LastIndexOf("\\", StringComparison.Ordinal) + 1), prepend, destinationRoot);
			Console.WriteLine("Would like to create " + destination);
			Directory.CreateDirectory(destination);
		}

		protected override void OnStop()
		{
			LogIt("Stopping service.");
		}

		private void LogIt(String toLog)
		{
			eventLog1.WriteEntry(toLog);
			Console.WriteLine(toLog);
		}
	}
}
