namespace CustomerLogIn
{
    using Microsoft.Azure.Cosmos.Table;

    public class Entity : TableEntity
    {
        public Entity()
        {
        }


        public Entity(string lastName, string firstName)
        {
            PartitionKey = lastName;
            RowKey = firstName;
        }


        public Entity(string lastName, string firstName, string passwordParam)
        {
            PartitionKey = lastName;
            RowKey = firstName;
            Password = passwordParam;
        }


        public Entity(string lastName, string firstName, string passwordParam, string favoritesParam)
        {
            PartitionKey = lastName;
            RowKey = firstName;
            Password = passwordParam;
            Favorites = null;
        }


        public string Password { get; set; }
        public string Favorites { get; set; }


    }
}
