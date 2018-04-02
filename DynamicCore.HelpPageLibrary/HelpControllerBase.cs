using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.DynamicController.Common.Helpers;
using DynamicCore.HelpPageLibrary.Extensions;
using DynamicCore.HelpPageLibrary.Models;
using DynamicCore.HelpPageLibrary.Views;

namespace DynamicCore.HelpPageLibrary
{
    [ApiExplorerSettings( IgnoreApi = true )]
    public abstract class HelpControllerBase : DynamicController.Common.DynamicControllerBase
    {
        public string HelpPageRouteName = "DefaultApi";

        [HttpGet]
        public virtual HttpResponseMessage Index()
        {
            var config = HttpConfigurationImporter.ImportConfigurationFromPath( ConfigurationManager.AppSettings[ DynamicCoreConstants.DynamicControllerDllPath ] );

            config.SetDocumentationProvider( new XmlDocumentationProvider( ConfigurationManager.AppSettings[ DynamicCoreConstants.DynamicControllerDllPath ] ) );

            var apiDescriptions = config.Services.GetApiExplorer().ApiDescriptions.Where( a =>
                 ControllersHelper.IsControllerEnabled( a.ActionDescriptor.ActionBinding.ActionDescriptor
                     .ControllerDescriptor.ControllerName ) == true ).ToList();

            Collection<ApiDescription> filteredApiDescriptions = new Collection<ApiDescription>( apiDescriptions );

            Index template = new Index
            {
                Model = filteredApiDescriptions ,
                ApiLinkFactory = apiName =>
                    {
                        string controllerName = Regex.Replace( GetType().Name , "controller" , "" , RegexOptions.IgnoreCase );
                        return Url.Route( HelpPageRouteName , new
                        {
                            controller = controllerName ,
                            apiId = apiName
                        } );
                    }
            };

            string helpPage = template.TransformText();
            return new HttpResponseMessage
            {
                Content = new StringContent( helpPage , Encoding.UTF8 , "text/html" )
            };
        }

        [HttpGet]
        public virtual HttpResponseMessage Api( string apiId )
        {
            if( !String.IsNullOrEmpty( apiId ) )
            {
                var config = HttpConfigurationImporter.ImportConfigurationFromPath( ConfigurationManager.AppSettings[ "DynamicControllerDllPath" ] );

                config.SetDocumentationProvider( new XmlDocumentationProvider( ConfigurationManager.AppSettings[ DynamicCoreConstants.DynamicControllerDllPath ] ) );

                HelpPageApiModel apiModel = config.GetHelpPageApiModel( apiId );
                if( apiModel != null )
                {
                    string controllerName = Regex.Replace( GetType().Name , "controller" , "" , RegexOptions.IgnoreCase );
                    Api template = new Api
                    {
                        Model = apiModel ,
                        HomePageLink = Url.Link( HelpPageRouteName , new
                        {
                            controller = controllerName
                        } )
                    };
                    string helpPage = template.TransformText();
                    return new HttpResponseMessage
                    {
                        Content = new StringContent( helpPage , Encoding.UTF8 , "text/html" )
                    };
                }
            }

            return Request.CreateErrorResponse( HttpStatusCode.NotFound , "API not found." );
        }
    }
}