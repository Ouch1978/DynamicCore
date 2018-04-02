namespace ProductsApiLibrary.Models
{
    /// <summary>
    /// Product definition
    /// </summary>
    public class Product
    {
        /// <summary>
        /// The Id of the Item
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Category
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price
        {
            get;
            set;
        }
    }
}
