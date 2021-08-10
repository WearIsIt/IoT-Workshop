namespace ProjectFunctions
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


        public string Password { get; set; }

    }
}