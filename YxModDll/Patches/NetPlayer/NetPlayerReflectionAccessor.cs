using Multiplayer;
using System;
using System.Reflection;
namespace YxModDll.Patches
{
    public class NetPlayerReflectionAccessor
    {
        private readonly NetPlayer player;

        private static readonly FieldInfo playDeadField =
            typeof(NetPlayer).GetField("playDead", BindingFlags.Instance | BindingFlags.NonPublic);

        public NetPlayerReflectionAccessor(NetPlayer player)
        {
            this.player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public bool playDead
        {
            get => (bool)playDeadField.GetValue(player);
            set => playDeadField.SetValue(player, value);
        }
    }
}
