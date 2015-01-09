# AssetsCopier
Maybe this isn't the best name for this repository. 
It is a tool that essentially takes a list of filesystem (sources, destinations), and mirrors everything in realtime from the source to the destination. It is written in C# and handles create, delete, rename, and update. It is based on .NET's FileSystemWatcher object. You can install it as a service. You configure source/destination lists with file filters in an App.config file. It logs all events to the EventLog under Applications > AssetsCopier.

This was originally written as a cache invalidation tool at DealerOn - whenever changes were made to a filesystem for a particular site, a cache reset would be triggered. Then it was adapted to what it is now, which is a sort of secure FTP-to-assets-server proxy. Basically rather than opening up assets servers to the whole world, we provide a secure FTP environment. Certain clients would get a special FTP login and directory, and assets they upload would get sent to the assets server for their site, and served. 

A future branch of this code would probably just provide developers hooks that they could register for various sources that would have different actions - like webhooks, file system operations, 3rd party library functions, etc.

TODO: Add code documentation..
