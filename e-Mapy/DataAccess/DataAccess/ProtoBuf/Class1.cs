using System.Collections.Generic;

namespace eMapy.DataAccess.DataAccess.ProtoBuf
{
    public class Class1
    {
        private List<int> collection = null;
        private object collectionLocker = new object();

        public void AddToList(int something)
        {
            lock (collectionLocker)
            {
                if (collection == null)
                {
                    collection = new List<int>();
                    collection.Add(1);
                }

                collection.Add(something);
            }
        }

        public void Delete(int something)
        {
            lock (collectionLocker)
            {
                if (collection == null)
                {
                    collection = new List<int>();
                    AddToList(1);
                }

                collection.Remove(something);
            }
        }
    }
}