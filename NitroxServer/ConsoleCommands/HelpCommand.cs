﻿using System.Collections.Generic;
using System.Linq;
using NitroxModel.DataStructures.GameLogic;
using NitroxModel.DataStructures.Util;
using NitroxModel.Logger;
using NitroxServer.ConsoleCommands.Abstract;

namespace NitroxServer.ConsoleCommands
{
    internal class HelpCommand : Command
    {
        public HelpCommand() : base("help", Perms.PLAYER, "", "Display help about supported commands")
        {

        }

        public override void RunCommand(string[] args, Optional<Player> player)
        {
            if (player.IsPresent())
            {
                List<string> cmdsText = GetHelpText(player.Get().Permissions);
                cmdsText.ForEach(cmdText => SendServerMessageIfPlayerIsPresent(player, cmdText));
            }
            else
            {
                List<string> cmdsText = GetHelpText(Perms.CONSOLE);
                cmdsText.ForEach(cmdText => Log.Info(cmdText));
            }
        }

        public override bool VerifyArgs(string[] args)
        {
            return args.Length == 0;
        }
        
        private class CommandComparer : IComparer<Command>
        {
            public int Compare(Command x, Command y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        private List<string> GetHelpText(Perms perm)
        {
            // runtime query to avoid circular dependencies
            IEnumerable<Command> commands = NitroxModel.Core.NitroxServiceLocator.LocateService<IEnumerable<Command>>();
            SortedSet<Command> sortedCommands = new SortedSet<Command>(commands.Where(cmd => cmd.RequiredPermLevel <= perm), new CommandComparer());
            return new List<string>(sortedCommands.Select(cmd => cmd.ToHelpText()));
        }
    }
}
