﻿using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;


namespace Bookly.Mvc
{
    public interface IEmbeddedPlugin
    {
         void InitiaizeEmbeddedFileSystem(IAppBuilder app);

        //void InitializeControllerFactory();
        
        void InitializeViews(string PluginDirectory);
        
        void RegisterRoutes(RouteCollection routes);
    }

    public interface IEmbeddedFileSystemInfo
    {
        string Path { get; set; }
        string baseNameSpace { get; set; }
    }
}
