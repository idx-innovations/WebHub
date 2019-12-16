//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'Nancy' framework
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Conventions;

//import internal namespace(s) required
using IDX.WebHub.Typing;
using IDX.WebHub.Typing.Configuration;
using IDX.WebHub.Nancy.StaticContent;
using IDX.WebHub.API.Modules;

namespace IDX.WebHub.Nancy
{
    public class RuntimeBootstrapper : DefaultNancyBootstrapper
    {

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="args"></param>
        public RuntimeBootstrapper(ref Host host)
        {

            //set the variable(s)
            Hub.Context = new HubContext(ref host);

        }

        /// <summary>
        /// override the module loading
        /// </summary>
        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {

                List<ModuleRegistration> modules = new List<ModuleRegistration>();

                foreach (Lazy<IWebModule, IWebModuleMetadata> module in Hub.Context.ModuleExtensions)
                {

                    modules.Add(new ModuleRegistration(module.Value.GetType()));

                }

                return modules;

            }

        }

        /// <summary>
        /// override the application startup method
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pipelines"></param>
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {

            base.ApplicationStartup(container, pipelines);

            //add request logging
            pipelines.BeforeRequest += (ctx) => {

                Hub.Context.Inform("Route '" + ctx.Request.Url + "' requested");

                return null;

            };

            //get the modules registered
            List<INancyModule> modules = new List<INancyModule>();

            foreach(ModuleRegistration registration in this.Modules)
            {

                //add the module to the list
                modules.Add(GetModule(container, registration.ModuleType));               

            }

            Hub.Context.Modules = modules.ToArray();

            //prompt the modules found
            foreach (Lazy<IWebModule, IWebModuleMetadata> module in Hub.Context.ModuleExtensions)
            {

                Hub.Context.Inform("Web module '" + module.Metadata.Name + "' ('" + module.Metadata.ID + "') found", true);

            }

        }

        /// <summary>
        /// override the method for configuring the conventions (e.g. for static content)
        /// </summary>
        /// <param name="nancyConventions"></param>
        /// <remarks>based on the articles found at 'https://github.com/NancyFx/Nancy/wiki/Managing-static-content' and 'https://stackoverflow.com/questions/13270163/how-to-serve-static-content-in-nancy'</remarks>
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {

            //add the static content (from host arguments)
            //'Content' will be enabled by default
            foreach (Typing.Configuration.StaticContentConfiguration config in Hub.Context.HostArguments.GetConfiguration<Typing.Configuration.StaticContentConfiguration>())
            {

                if(ConventionBuilder.IsFile(config.Path)) //add a static file route
                {

                    nancyConventions.StaticContentsConventions.Add(ConventionBuilder.AddFile(config.Route, config.Path));
                    Hub.Context.Inform("Static content file '" + config.Path + "' added to route '" + config.Route + "'", true);

                }
                else //add a static directory route
                {

                    nancyConventions.StaticContentsConventions.Add(ConventionBuilder.AddDirectory(config.Route, config.Path));
                    Hub.Context.Inform("Static content directory '" + config.Path + "' added to route '" + config.Route + "'", true);

                }

            }

            //initialize the conventions
            base.ConfigureConventions(nancyConventions);

        }

    }

}
