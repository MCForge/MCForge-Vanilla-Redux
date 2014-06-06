using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge
{
    public class Inventory
    {
        public List<Slot> slots = new List<Slot>();
        public struct Slot
        {
            public byte ID;
            public uint Amount;
        }
    }
}
