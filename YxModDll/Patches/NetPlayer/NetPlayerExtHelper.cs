using Multiplayer;
using System.Runtime.CompilerServices;

public static class NetPlayerExtHelper
{
    private static readonly ConditionalWeakTable<NetPlayer, NetPlayerReflectionAccessor> _ext = new ConditionalWeakTable<NetPlayer, NetPlayerReflectionAccessor>();

    public static NetPlayerReflectionAccessor GetAccessor(this NetPlayer player)
        => _ext.GetOrCreateValue(player);
}
