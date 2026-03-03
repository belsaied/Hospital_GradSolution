using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class BedsByRoomSpecification :BaseSpecifications<Bed,int>
    {
        public BedsByRoomSpecification(int roomId) : base(b => b.RoomId == roomId)
        {
            AddInclude(b => b.Room);
            AddOrderBy(b => b.BedNumber);
        }

    }
}
