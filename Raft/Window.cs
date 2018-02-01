﻿namespace Raft {
    using System;
    using SDL2;
    using VulkanCore;
    using VulkanCore.Khr;
    using VulkanCubes;

    public class Window {

        static bool SDLInit = false;

        protected internal IntPtr handle;
        string title;

        public Window(string title, int width, int height) {
            if (!SDLInit) { InitSDL(); }

            this.title = title;
            handle = SDL.SDL_CreateWindow(title, 50, 50, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN
                                                                    | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            if (handle == IntPtr.Zero) { throw new SDLException(); }
        }

        public SurfaceKhr GetSurface(Instance inst) {
            SDL.SDL_Vulkan_CreateSurface(handle, inst, out IntPtr surf);

            return inst.CreateXlibSurfaceKhr(new XlibSurfaceCreateInfoKhr(IntPtr.Zero, handle));
        }

        public static void InitSDL() {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) != 0) { throw new SDLException(); }
            SDLInit = true;
        }

    }
}