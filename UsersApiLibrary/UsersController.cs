using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DynamicCore.DynamicController.Common;
using DynamicCore.DynamicController.Common.Helpers;
using DynamicCore.DynamicController.Common.Models;
using UsersApiLibrary.Models;

namespace UsersApiLibrary
{
    [RoutePrefix( "api/users" )]
    public class UsersController : DynamicControllerBase
    {
        public UsersController()
        {
        }

        readonly User[] _users = new User[]  
        {  
            new User { Id = 1, Name = "Tomato Soup1", Category = "Groceries", Birthday = new DateTime(1980,1,1) },  
            new User { Id = 2, Name = "Yo-yo1" , Category = "Toys", Birthday = new DateTime(1985,12,2)},  
            new User { Id = 3, Name = "Hammer1", Category = "Hardware", Birthday = new DateTime(1983,5,17)}  
        };

        /// <summary>
        /// GetAllUsers test
        /// </summary>
        /// <returns>1231231</returns>
        public IEnumerable<User> GetAllUsers()
        {
            //string value = CustomConfigurationHelper.GetAppSettingValue( "Test" );

            //string value2 = CustomConfigurationHelper.GetSectionSettingValue( "Test" , "Test2" );

            string json = SerializerHelper.Serialize( _users );

            var jsonObject = SerializerHelper.Deserialize<User[]>( json );

            string xml = SerializerHelper.Serialize( _users , MessageMediaTypes.Xml );

            var xmlObject = SerializerHelper.Deserialize<User[]>( xml , MessageMediaTypes.Xml );

            return _users;
        }

        /// <summary>
        /// GetUserById test
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>123</returns>
        [Route( "id:int" )]
        public User GetUserById( int id )
        {
            var product = _users.FirstOrDefault( ( p ) => p.Id == id );
            if( product == null )
            {
                throw new HttpResponseException( HttpStatusCode.NotFound );
            }
            return product;
        }

        public IEnumerable<User> GetUsersByCategory( string category )
        {
            return _users.Where( p => string.Equals( p.Category , category ,
                StringComparison.OrdinalIgnoreCase ) );
        }
    }}
