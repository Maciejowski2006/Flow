using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Flow.Player.Messages;

public class PlaylistViewChangedMessage(bool newState) : ValueChangedMessage<bool>(newState);