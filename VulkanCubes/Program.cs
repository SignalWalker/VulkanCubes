using System;

namespace VulkanCubes {
    using System.Runtime.CompilerServices;
    using Raft;
    using Raft.Input;
    using SDL2;
    using VulkanCore;
    using VulkanCore.Khr;

    class Program  {

        static Forge forge;
        static bool run = true;

        public static void Main(string[] args) {
            forge = new Forge(1, "VulkanCubes", 100, 100, 1280, 720);
            Run();
        }

        static void Run() {
            RecordCommandBuffer(forge.Context(0).graphicsBuffers[0], 0);
            while (run) {
                Draw();
                foreach (SDL.SDL_Event ev in InputStream.ReadAll()) {
                    if (ev.type == SDL.SDL_EventType.SDL_QUIT) { run = false; }
                }
            }
            Quit();
        }

        static void Draw() {
            Context c = forge.Context(0);
            Semaphore renderFinished = c.Device.CreateSemaphore();
            c.GraphicsQueue.Submit(c.Device.CreateSemaphore(), PipelineStages.Transfer, c.graphicsBuffers[0], renderFinished);
            c.PresentQueue.PresentKhr(renderFinished, c.swapchain, 0);
        }

        public static void RecordCommandBuffer(CommandBuffer cmdBuffer, int imgIndex) {
            Context c = forge.Context(0);
            RenderPassBeginInfo bInfo = new RenderPassBeginInfo(
                c.framebuffers[imgIndex],
                c.pass,
                new Rect2D(0, 0, c.width, c.height),
                new ClearColorValue(new ColorF4(.8f, .2f, .4f, 1)),
                new ClearDepthStencilValue(1, 0));
            cmdBuffer.Begin(new CommandBufferBeginInfo(CommandBufferUsages.SimultaneousUse));
            cmdBuffer.CmdBeginRenderPass(bInfo);
            cmdBuffer.CmdBindPipeline(PipelineBindPoint.Graphics, c.pipeline);
            cmdBuffer.CmdBindDescriptorSets(PipelineBindPoint.Graphics, c.pipelineLayout, 0, c.descriptorSets);
            //cmdBuffer.CmdBindVertexBuffer(c.vbo.buffer);
            //cmdBuffer.CmdBindIndexBuffer(c.index.buffer);
            cmdBuffer.CmdSetViewport(new Viewport(0, 0, c.width, c.height));
            cmdBuffer.CmdClearColorImage(c.swapchainImages[imgIndex], ImageLayout.TransferDstOptimal, new ClearColorValue(new ColorF4(.8f, .58f, .93f, 1f)));
            //cmdBuffer.CmdDrawIndexed(c.index.count);
            cmdBuffer.CmdEndRenderPass();
            cmdBuffer.End();
        }

        public static void Quit() { forge.Quit(); }

        /*protected void RecordCommandBuffer(CommandBuffer cmdBuffer, int imageIndex) {
            var imageSubresourceRange = new ImageSubresourceRange(ImageAspects.Color, 0, 1, 0, 1);

            var barrierFromPresentToClear = new ImageMemoryBarrier(
                SwapchainImages[imageIndex], imageSubresourceRange,
                Accesses.None, Accesses.TransferWrite,
                ImageLayout.Undefined, ImageLayout.TransferDstOptimal);
            var barrierFromClearToPresent = new ImageMemoryBarrier(
                SwapchainImages[imageIndex], imageSubresourceRange,
                Accesses.TransferWrite, Accesses.MemoryRead,
                ImageLayout.TransferDstOptimal, ImageLayout.PresentSrcKhr);

            cmdBuffer.CmdPipelineBarrier(
                PipelineStages.Transfer, PipelineStages.Transfer,
                imageMemoryBarriers: new[] { barrierFromPresentToClear });
            cmdBuffer.CmdClearColorImage(
                SwapchainImages[imageIndex],
                ImageLayout.TransferDstOptimal,
                new ClearColorValue(new ColorF4(0.39f, 0.58f, 0.93f, 1.0f)),
                imageSubresourceRange);
            cmdBuffer.CmdPipelineBarrier(
                PipelineStages.Transfer, PipelineStages.Transfer,
                imageMemoryBarriers: new[] { barrierFromClearToPresent });
        }

        protected override void Draw(Timer timer) {
            // Acquire an index of drawing image for this frame.
            int imageIndex = Swapchain.AcquireNextImage(semaphore: ImageAvailableSemaphore);

            // Submit recorded commands to graphics queue for execution.
            Context.GraphicsQueue.Submit(
                ImageAvailableSemaphore,
                PipelineStages.Transfer,
                CommandBuffers[imageIndex],
                RenderingFinishedSemaphore
            );

            // Present the color output to screen.
            Context.PresentQueue.PresentKhr(RenderingFinishedSemaphore, Swapchain, imageIndex);
        }*/
    }
}
