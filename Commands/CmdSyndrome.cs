using System;
using System.Threading;

namespace MCForge.Commands
{
	public class CmdSyndrome : Command
	{
		public override string name { get { return "syndrome"; } }
		public override string shortcut { get { return "sy"; } }
		public override string type { get { return "other"; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

		public override void Use(Player p, string message)
		{
			if (!p.ExtraData.ContainsKey("syndrome"))
			{
				p.ExtraData.Add("syndrome", false);
			}
			if (!(bool)p.ExtraData["syndrome"])
			{
				if (message != "")
				{
					Player who = Player.Find(message);
					if (who == null || who.hidden)
					{
						Player.SendMessage(p, message + " is not online right now.");
					}
					else if (who.group.Permission > p.group.Permission)
					{
						Player.SendMessage(p, who.name + " will overpower you, command stopped.");
					}
					else if (who.level != p.level)
					{
						Player.SendMessage(p, who.name + " is in a different map.");
					}
					else
					{
						Player.SendMessage(p, "Successfully took control over " + who.name);
						Player.SendMessage(who, p.name + " is using his magic powers on you!");
						p.ExtraData["syndrome"] = true;
						double distance = Math.Sqrt(Math.Pow(Math.Abs(who.pos[0] - p.pos[0]), 2) + Math.Pow(Math.Abs(who.pos[1] - p.pos[1]), 2) + Math.Pow(Math.Abs(who.pos[2] - p.pos[2]), 2));
						distance /= 32;
						Thread synThread = new Thread(new ThreadStart(delegate
						                                              {
							while ((bool)p.ExtraData["syndrome"] && !p.disconnected)
							{
								if (p.level != who.level || who.disconnected)
								{
									p.ExtraData["syndrome"] = false;
									Player.SendMessage(p, "He is out of your reach now.");
								}
								Thread.Sleep(50);
								double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
								double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
								double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);
								double d = Math.Cos(((double)(p.rot[1]) / 256) * 2 * Math.PI);
								ushort x = (ushort)Math.Round((p.pos[0] / 32) + (double)(a * distance * d));
								ushort y = (ushort)Math.Round((p.pos[1] / 32 + 1) + (double)(c * distance));
								ushort z = (ushort)Math.Round((p.pos[2] / 32) + (double)(b * distance * d));
								if (x > p.level.width)
								{
									x = (ushort)(p.level.width - 1);
								}
								if (y > p.level.height)
								{
									y = (ushort)(p.level.height - 1);
								}
								if (z > p.level.depth)
								{
									z = (ushort)(p.level.depth - 1);
								}
								x *= 32; x += 16;
								y *= 32; y += 32;
								z *= 32; z += 16;
								who.rot[0] = (byte)(p.rot[0] - 128);
								who.rot[1] = (byte)-p.rot[1];
								unchecked { who.SendPos((byte)-1, x, y, z, who.rot[0], who.rot[1]); }
							}
						}));
						synThread.Start();

					}
				}
				else
				{
					Help(p);
				}
			}
			else
			{
				p.ExtraData["syndrome"] = false;
				Player.SendMessage(p, "Syndrome powers have worn off");
			}
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/syndrome [name] -- control someone by the direction you look");
		}
	}
}
