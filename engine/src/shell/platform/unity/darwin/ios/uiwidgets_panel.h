#pragma once

#include <mutex>
#include <queue>
#include <map>

#include <flutter/fml/memory/ref_counted.h>
#include "shell/platform/unity/gfx_worker_task_runner.h"
#include "lib/ui/window/pointer_data.h"
#include "runtime/mono_api.h"
#include "cocoa_task_runner.h"
#include "unity_surface_manager.h"

namespace uiwidgets {

enum UIWidgetsWindowType {
  InvalidPanel = 0,
  GameObjectPanel = 1,
  EditorWindowPanel = 2
};

enum UIWidgetsTouchPhase
{
  TouchBegan,
  TouchEnded,
  TouchMoved,
  TouchCancelled
};

class UIWidgetsPanel : public fml::RefCountedThreadSafe<UIWidgetsPanel> {
  FML_FRIEND_MAKE_REF_COUNTED(UIWidgetsPanel);

 public:
  typedef void (*EntrypointCallback)(Mono_Handle handle);

  static fml::RefPtr<UIWidgetsPanel> Create(
      Mono_Handle handle, UIWidgetsWindowType window_type, EntrypointCallback entrypoint_callback);

  ~UIWidgetsPanel();

  void* OnEnable(size_t width, size_t height, float device_pixel_ratio, const char* streaming_assets_path, const char* settings);

  void MonoEntrypoint();

  void OnDisable();

  void* OnRenderTexture(size_t width, size_t height,
                       float dpi);
  
  bool ReleaseNativeRenderTexture();

  int RegisterTexture(void* native_texture_ptr);

  void UnregisterTexture(int texture_id);

  std::chrono::nanoseconds ProcessMessages();

  void ProcessVSync();

  void VSyncCallback(intptr_t baton);

  void SetEventLocationFromCursorPosition(UIWidgetsPointerEvent* event_data);

  void OnKeyDown(int keyCode, bool isKeyDown, int64_t modifier);

  void OnMouseMove(float x, float y, int button);

  void OnScroll(float x, float y, float px, float py);

  void OnMouseDown(float x, float y, int button);

  void OnMouseUp(float x, float y, int button);

  void OnMouseLeave();

  bool NeedUpdateByPlayerLoop();

  bool NeedUpdateByEditorLoop();

 private:
  UIWidgetsPanel(Mono_Handle handle, UIWidgetsWindowType window_type, EntrypointCallback entrypoint_callback);

  void CreateInternalUIWidgetsEngine(size_t width, size_t height, float device_pixel_ratio, const char* streaming_assets_path, const char* settings);

  void dispatchTouches(float x, float y, int button, UIWidgetsTouchPhase evtType);

  static PointerData::Change PointerDataChangeFromUITouchPhase(UIWidgetsTouchPhase phase);

  static PointerData::DeviceKind DeviceKindFromTouchType();

  Mono_Handle handle_;
  EntrypointCallback entrypoint_callback_;
  UIWidgetsWindowType window_type_;

  std::unique_ptr<UnitySurfaceManager> surface_manager_;

  std::unique_ptr<GfxWorkerTaskRunner> gfx_worker_task_runner_;
  std::unique_ptr<CocoaTaskRunner> task_runner_;
  UIWidgetsEngine engine_ = nullptr;

  std::vector<intptr_t> vsync_batons_;

  bool process_events_ = false;
};

}  // namespace uiwidgets
