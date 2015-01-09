using System.IO;

namespace AssetsCopier
{
	class FileSystemWatcherWithDestination : FileSystemWatcher
	{
		private string _destinationRoot = "";

		public FileSystemWatcherWithDestination(string source, string destinationRoot) : base(source)
		{
			_destinationRoot = destinationRoot;
		}
		public string DestinationRoot
		{
			get { return _destinationRoot; }
			set { _destinationRoot = value; }
		}
	}
}
