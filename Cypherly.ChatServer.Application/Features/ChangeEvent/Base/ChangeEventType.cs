namespace Cypherly.ChatServer.Application.Features.ChangeEvent;

public enum ChangeEventType
{
    ProfilePictureChanged,
    DisplayNameChanged,
    StatusChanged,
    FriendRequestSent,
    FriendRequestAccepted,
    FriendRequestRejected,
    FriendRemoved,
    UserBlocked,
    UserUnblocked,
}