# AssetsCopier
Maybe this isn't the best name for this repository. 
It is a tool that essentially takes a list of filesystem (sources, destinations), and mirrors all files (matching any of a specified list of extension filters) in realtime from the source to the destination. It is written in C# and handles create, delete, rename, and update. It is based on .NET's FileSystemWatcher object. You can install it as a service. You configure source/destination lists with file filters in an App.config file. It logs all events to the EventLog under Applications > AssetsCopier.

This was originally written as a cache invalidation tool at DealerOn - whenever changes were made to a filesystem for a particular site, a cache reset would be triggered. Then it was adapted to what it is now, which is a sort of secure FTP-to-assets-server proxy. Basically rather than opening up assets servers to the whole world, we provide a secure FTP environment. We run this service on the FTP server, expose certain directories tied to secure FTP user accounts for particular clients, and whenever they'd upload assets, the assets would get sent to the assets server. Then they could make use of the assets on their site(s). 

A future branch of this code would probably just provide developers hooks that they could register for various sources that would have different actions - like webhooks, file system operations, 3rd party library functions, etc, and be configurable through a secure web interface. It also needs better logging options/ability than just EventLog.

TODO: Add code documentation..
