using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class WardWithRoomsAndBedsSpecification :BaseSpecifications<Ward,int>
    {
        public WardWithRoomsAndBedsSpecification(int wardId) : base(w => w.Id == wardId)
        {
            AddInclude(w => w.Rooms);
            // Deep include Beds inside Rooms is done via ThenInclude in the repository
            AddInclude("Room.Ward");
        }

    }
}
