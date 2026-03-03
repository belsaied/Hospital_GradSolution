using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class AllWardsWithRoomsSpecification :BaseSpecifications<Ward,int>
    {
        public AllWardsWithRoomsSpecification() : base()
        {
            AddInclude(w => w.Rooms);
            AddOrderBy(w => w.Name);
        }

    }
}
