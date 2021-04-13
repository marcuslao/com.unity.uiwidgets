

#ifndef UIWIDGETS_SHELL_PLATFORM_COMMON_CPP_PUBLIC_UIWIDGETS_PLUGIN_REGISTRAR_H_
#define UIWIDGETS_SHELL_PLATFORM_COMMON_CPP_PUBLIC_UIWIDGETS_PLUGIN_REGISTRAR_H_

#include <stddef.h>
#include <stdint.h>

#include "uiwidgets_export.h"
#include "uiwidgets_messenger.h"



typedef struct UIWidgetsDesktopPluginRegistrar* UIWidgetsDesktopPluginRegistrarRef;

UIWidgetsDesktopMessengerRef
UIWidgetsDesktopRegistrarGetMessenger(UIWidgetsDesktopPluginRegistrarRef registrar);

void UIWidgetsDesktopRegistrarEnableInputBlocking(
    UIWidgetsDesktopPluginRegistrarRef registrar, const char* channel);
#endif