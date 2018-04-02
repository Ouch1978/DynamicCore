using System;
using System.Web.Http.Dispatcher;
using DynamicCore.DynamicController.Common;

namespace DynamicCore.DynamicHostConsole
{
    public class CustomHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
    {
        public CustomHttpControllerTypeResolver()
            : base( IsHttpEndpoint )
        {
        }

        internal static bool IsHttpEndpoint( Type t )
        {
            if( t == null )
            {

                throw new ArgumentNullException( "t" );
            }

            return t.IsClass && t.IsVisible && !t.IsAbstract && typeof( DynamicControllerBase ).IsAssignableFrom( t );
        }
    }
}
