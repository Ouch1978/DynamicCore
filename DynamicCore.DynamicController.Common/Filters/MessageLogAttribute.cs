using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http.Filters;
using DynamicCore.DynamicController.Common.Constants;
using DynamicCore.DynamicController.Common.Extensions;
using DynamicCore.DynamicController.Common.Models;
using log4net;

namespace DynamicCore.DynamicController.Common.Filters
{
    public class MessageLogAttribute : ActionFilterAttribute
    {
        private static readonly ILog _messageLogger = LogManager.GetLogger( "MessagesLogger" );

        private static readonly ILog _applicationLogger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

        private const string TimeStampHeader = "TimeStampForMessageLog";

        public override void OnActionExecuting( System.Web.Http.Controllers.HttpActionContext actionContext )
        {
            base.OnActionExecuting( actionContext );

            try
            {
                actionContext.Request.Headers.Add( TimeStampHeader , DateTime.Now.ToString() );
            }
            catch( Exception ex )
            {
                _applicationLogger.ErrorFormat( ex.StackTrace );
            }

        }

        public override async void OnActionExecuted( HttpActionExecutedContext actionExecutedContext )
        {
            base.OnActionExecuted( actionExecutedContext );

            if( DynamicCoreConstants.IsMessageLogEnabled == true )
            {
                MessageLog messageLog = new MessageLog();

                try
                {

                    if( actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName.ToLower() != "help" )
                    {
                        messageLog.RequestMethod = actionExecutedContext.ActionContext.Request.Method.Method;

                        messageLog.RequestUrl = actionExecutedContext.ActionContext.Request.RequestUri.AbsoluteUri;

                        messageLog.RequestContent = await actionExecutedContext.ActionContext.Request.Content.ReadAsStringAsync();

                        messageLog.RequestIp = actionExecutedContext.ActionContext.Request.GetClientIpAddress();

                        if( actionExecutedContext.ActionContext.Request.Headers.Contains( TimeStampHeader ) == true )
                        {
                            IEnumerable<string> timeStampString;

                            actionExecutedContext.ActionContext.Request.Headers.TryGetValues( TimeStampHeader , out timeStampString );

                            var stampString = timeStampString as string[] ?? timeStampString.ToArray();

                            if( stampString.Any() )
                            {
                                messageLog.RequestTimestamp = DateTime.Parse( stampString.First() );

                                messageLog.ResponseTimeCost = ( long ) ( DateTime.Now - messageLog.RequestTimestamp ).TotalMilliseconds;
                            }
                        }

                        if( actionExecutedContext.Response.IsSuccessStatusCode == true )
                        {
                            messageLog.ResponseContent = await actionExecutedContext.Response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            messageLog.ResponseContent = string.Format( "HTTP Error: {0} - {1}" , actionExecutedContext.Response.StatusCode , actionExecutedContext.Response.ReasonPhrase );
                        }

                        _messageLogger.Info( messageLog.ToString() );
                    }
                }
                catch( Exception ex )
                {
                    _messageLogger.Info( messageLog.ToString() );

                    _applicationLogger.ErrorFormat( ex.StackTrace );

                }
            }
        }
    }
}
