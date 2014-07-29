/*
	Copyright © 2009-2014 MCSharp team (Modified for use with MCZall/MCLawl/MCForge/MCForge-Redux)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge
{
    public class Inventory
    {
        public Slot[] slots;

        public Inventory()
        {
            slots = new Slot[65];
            Slot tmp;
            for (int i = 1; i < slots.Length; ++i)
            {
                tmp.ID = (byte)i;
                tmp.Amount = 0;
                slots[i] = tmp;
            }
        }

        public void Add(byte id, int amount)
        {
            if (amount < 1) return;
            if (slots[id].Amount + amount > uint.MaxValue)
            {
                slots[id].Amount = uint.MaxValue;
                return;
            }
            slots[id].Amount += (uint)amount;
            return;
        }

        public bool Remove(byte id, int amount)
        {
            if (amount > slots[id].Amount) { slots[id].Amount = 0; return false; }
            slots[id].Amount -= (uint)amount;
            return true;
        }

        public struct Slot
        {
            public byte ID;
            public uint Amount;

        }
    }
}
