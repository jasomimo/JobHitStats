using System.Collections.Generic;
namespace JobHitStats
{
    interface IDataStorage
    {
        void StoreData(JobPortal portal, IDictionary<Technology, uint> jobOffers);

        void DeleteRow(JobPortal portal, int id);
    }
}
