namespace VulkanCubes {
    using System;
    using System.IO;
    using SDL2;
    using VulkanCore.Samples;
    public class AppHost : IVulkanAppHost {

        string title;
        VulkanApp app;
        IntPtr window;
        int width;
        int height;

        public AppHost(string title, int width, int height, VulkanApp app) {
            this.title = title;
            this.app = app;
            this.width = width;
            this.height = height;
        }

        public void Init() {
            if (SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING) != 0) {
                throw new Exception(SDL.SDL_GetError());
            }

            window = SDL.SDL_CreateWindow(title, 10, 10, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_VULKAN | SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (window == IntPtr.Zero) {
                throw new Exception(SDL.SDL_GetError());
            }
        }

        public void Run() {

        }

        public void Dispose() { throw new NotImplementedException(); }
        public IntPtr WindowHandle => window;
        public IntPtr InstanceHandle { get; }
        public int Width => width;
        public int Height => height;
        public Platform Platform => Platform.SDL2;
        public Stream Open(string path) { throw new NotImplementedException(); }
    }
}