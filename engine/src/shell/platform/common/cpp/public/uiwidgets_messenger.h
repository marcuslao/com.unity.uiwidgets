

#ifndef UIWIDGETS_SHELL_PLATFORM_COMMON_CPP_PUBLIC_UIWIDGETS_MESSENGER_H_
#define UIWIDGETS_SHELL_PLATFORM_COMMON_CPP_PUBLIC_UIWIDGETS_MESSENGER_H_

#include <stddef.h>
#include <stdint.h>

#include "uiwidgets_export.h"
#include "runtime/mono_api.h"

typedef struct UIWidgetsDesktopMessenger* UIWidgetsDesktopMessengerRef;

typedef struct _UIWidgetsPlatformMessageResponseHandle
    UIWidgetsDesktopMessageResponseHandle;

typedef void (*UIWidgetsDesktopBinaryReply)(const uint8_t* data, size_t data_size,
                                          void* user_data);

typedef struct {
  size_t struct_size;

  const char* channel;

  const uint8_t* message;

  size_t message_size;

  const UIWidgetsDesktopMessageResponseHandle* response_handle;
} UIWidgetsDesktopMessage;

typedef void (*UIWidgetsDesktopMessageCallback)(
    UIWidgetsDesktopMessengerRef /* messenger */,
    const UIWidgetsDesktopMessage* /* message*/, void* /* user data */);

bool UIWidgetsDesktopMessengerSend(
    UIWidgetsDesktopMessengerRef messenger, const char* channel,
    const uint8_t* message, const size_t message_size);

bool UIWidgetsDesktopMessengerSendWithReply(
    UIWidgetsDesktopMessengerRef messenger, const char* channel,
    const uint8_t* message, const size_t message_size,
    const UIWidgetsDesktopBinaryReply reply, void* user_data);

void UIWidgetsDesktopMessengerSendResponse(
    UIWidgetsDesktopMessengerRef messenger,
    const UIWidgetsDesktopMessageResponseHandle* handle, const uint8_t* data,
    size_t data_length);

UIWIDGETS_API(void) UIWidgetsDesktopMessengerSetCallback(
    UIWidgetsDesktopMessengerRef messenger, const char* channel,
    UIWidgetsDesktopMessageCallback callback, void* user_data);

#endif
