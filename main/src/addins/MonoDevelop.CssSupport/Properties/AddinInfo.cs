using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
	"MonoDevelop.CssSupport", 
	Namespace = "MonoDevelop.CssSupport",
	Version = "1.0"
)]

[assembly:AddinName ("Css Support")]
[assembly:AddinCategory ("Web Development")]
[assembly:AddinDescription ("Support for Css documents.")]
[assembly:AddinAuthor ("Harsimran Bath")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
