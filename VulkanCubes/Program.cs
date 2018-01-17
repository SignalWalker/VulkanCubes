﻿using System;

namespace VulkanCubes {
    using System.Runtime.CompilerServices;
    using VulkanCore;
    using VulkanCore.Khr;
    using VulkanCore.Samples;

    class Program : VulkanApp  {

        public static void Main(string[] args) {
            AppHost host = new AppHost("VulkanCubes", new Program());
            host.Init();
            host.Run();
        }

        protected override void RecordCommandBuffer(CommandBuffer cmdBuffer, int imageIndex) {
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
        }
    }
}
