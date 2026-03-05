using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class BedsByRoomSpecification_ById : BaseSpecifications<Bed, int>
    {
        public BedsByRoomSpecification_ById(int bedId) : base(b => b.Id == bedId)
        {
            AddInclude(b => b.Room);
            AddInclude("Room.Ward");
        }
    }

}
