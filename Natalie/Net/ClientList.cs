namespace DKSY.Natalie.Net
{
    class ClientList
        : System.Collections.ObjectModel.KeyedCollection<string, Client>
    {
        protected override string GetKeyForItem(Client item)
        {
            return item.ID;
        }
    }
}
