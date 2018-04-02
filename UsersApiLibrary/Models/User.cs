using System;

namespace UsersApiLibrary.Models
{
    public class User
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }
    }
}
