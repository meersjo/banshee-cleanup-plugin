MCS_FLAGS = -debug
ASSEMBLY = Cleanup.dll
SOURCES = \
	$(srcdir)/Log.cs \
	$(srcdir)/CleanupPlugin.cs

cleanup_resources = \
	CleanupMenu.xml 

cleanup_resources_build = $(foreach res,$(cleanup_resources), $(addprefix -resource:,$(srcdir)/$(res)),$(notdir $(res)))


plugindir = $(PLUGINDIR)
plugin_SCRIPTS = $(ASSEMBLY) $(ASSEMBLY).mdb

all: $(ASSEMBLY)

$(ASSEMBLY): $(SOURCES)
	$(MCS) $(MCS_FLAGS) -out:$(ASSEMBLY) -target:library -nowarn:0169 -r:System.Data -r:System.Web -r:System.Xml -r:Mono.Posix $(cleanup_resources_build) $(BANSHEE_LIBS) $(GTKSHARP_LIBS) $(SOURCES)

EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY) $(ASSEMBLY).mdb 
DISTCLEANFILES = Makefile.in 

