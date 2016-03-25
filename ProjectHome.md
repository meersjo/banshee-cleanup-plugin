This is a simple plug-in that removes stale tracks from your Banshee library.

Banshee is a media player written using the Mono framework.  More more information, see http://www.banshee-project.org

This plug-in has been tested with the 0.11.3 release.

NOTE: if you are using a release prior to 0.11.3, you will receive a compile error complaining about file CleanupPlugin.cs having invalid arguments.  The Banshee API changed and this plugin has been updated to work with the latest code.

To fix the compile error change line 195 from:

> if (!Banshee.IO.IOProxy.File.Exists(uri))

To:

> if (!Banshee.IO.IOProxy.File.Exists(path))


That's it.  Enjoy.


