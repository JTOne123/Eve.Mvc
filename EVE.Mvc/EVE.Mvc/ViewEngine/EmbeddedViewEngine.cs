﻿using EVE.Mvc.Composition;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EVE.Mvc
{

    public class EmbeddedViewEngine : IViewEngine
    {
        public IEnumerable<EmbeddedResourceOption> options;
        private ConcurrentDictionary<string, string> resourceDictionary;
        private ConcurrentDictionary<string, string> ResourceDictionary
        {
            get
            {
                return resourceDictionary ?? (resourceDictionary = new ConcurrentDictionary<string, string>());
            }
        }

        public EmbeddedViewEngine():this(new List<EmbeddedResourceOption>() { 
                new EmbeddedResourceOption()
                {
                    Extension = ".html"
                },
                new EmbeddedResourceOption()
                {
                    Extension = ".cshtml"
                }
            })
        {
           
        }

        public EmbeddedViewEngine(IEnumerable<EmbeddedResourceOption> options)
        {

            this.options = options;
            //InitializeResourceDictionary();
        }
        
        #region resource index

        private void InitializeResourceDictionary()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in assemblies)
            {
                ExtractViewResourcesInfoFromAssembly(item);
            }


        }

        private void ExtractViewResourcesInfoFromAssembly(Assembly asm)
        {
            var resNames = asm.GetManifestResourceNames();
            foreach (var item in resNames)
            {
                if (IsResourceView(item))
                {
                    ResourceDictionary[item] = asm.FullName;
                }
            }
        }

        private bool IsResourceView(string resourceName)
        {
            foreach (var item in options)
            {
                if (resourceName.EndsWith(item.Extension))
                    return true;
            }
            return false;
        }
        #endregion

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            var views = AppMefContainer.Container.GetExports<EmbeddedView>(partialViewName);
            if (views != null && views.Count() > 0)
            {
                var view = views.First().Value;
                view.ViewName = partialViewName;
                view.AssemblyName = resourceDictionary[partialViewName];
                return new ViewEngineResult(view, this);
            }
            return new ViewEngineResult(new string[] { partialViewName });
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var views = AppMefContainer.Container.GetExports<EmbeddedView>(viewName);
            if (views != null && views.Count() > 0)
            {
                var view = views.First().Value;
                view.ViewName = viewName;
                view.MasterName = masterName;
                view.AssemblyName = resourceDictionary[viewName];
                return new ViewEngineResult(view, this);
            }
            return new ViewEngineResult(new string[] { masterName, viewName });
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {

        }
    }

    public class EmbeddedResourceOption
    {
        public string Extension { get; set; }

    }
}