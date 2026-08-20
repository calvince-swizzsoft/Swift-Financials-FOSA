# Instant Messaging API

## Purpose

This module provides authenticated, persistent direct and group conversations between application users. It replaces the legacy global in-memory SignalR sample, which accepted a client-supplied sender name, broadcast every message to every connected user, and lost history when the application restarted.

The current delivery transport is short polling: the client refreshes conversations every five seconds and requests only messages newer than its last message every three seconds. This works with the existing Web API 2/IIS host without adding a SignalR runtime. The persisted API can later be paired with SignalR notifications without changing its authorization or storage model.

## Access control

- Authentication is required for every endpoint.
- The role must contain `SystemPermissionTypes.InstantMessagingAccess`.
- The sender is always taken from the authenticated principal; request bodies cannot impersonate another user.
- Only conversation participants can read, send, or update read state.
- New participants must be existing, unlocked application users.

## Endpoints

Base route: `/api/messaging/instant-messages`

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/contacts?text=&pageIndex=0&pageSize=100` | Search eligible application users. The signed-in user is excluded. |
| GET | `/conversations` | List the current user's conversations, latest message, participant count, and unread count. |
| POST | `/conversations` | Create a direct or group conversation. Body: `{ participantUserNames: [], title: "" }`. A duplicate direct conversation returns the existing ID. |
| GET | `/conversations/{id}/messages?afterId=0&pageSize=100` | Read messages. Supply `afterId` for incremental polling. |
| POST | `/conversations/{id}/messages` | Send `{ body: "..." }`. Body length is limited to 4,000 characters. |
| POST | `/conversations/{id}/read` | Move the current participant's read watermark to the current UTC time. |

Success responses use `{ success, message, data }`. Validation failures return `400`, non-participants receive `403`, and missing conversations receive `404` where applicable.

## Persistence

Install [`install-instant-messaging.sql`](../database/install-instant-messaging.sql) against the business database. The idempotent script creates:

- `swiftFin_InstantMessageConversations`
- `swiftFin_InstantMessageConversationParticipants`
- `swiftFin_InstantMessages`

Messages are append-only through this API. Conversation membership is fixed after creation in this first release. File attachments, editing/deleting messages, typing indicators, and presence are intentionally not claimed as supported.

## Frontend mapping

Navigation module code `26010` maps to `/Messaging/InstantMessaging`. Grant `InstantMessagingAccess` to the intended roles through role-permission administration before testing the screen.
