using System;
using SDL2;

class Switch2Emu
{
    byte[] memory = new byte[1024 * 1024 * 64];
    ulong[] X = new ulong[31];
    ulong PC = 0x1000;
    ulong SP = 0x2000;
    bool running = false;

    bool flagN = false;
    bool flagZ = false;
    bool flagC = false;
    bool flagV = false;

    IntPtr window;
    IntPtr renderer;
    bool windowOpen = true;

    // Boot screen timer
    bool bootScreenDone = false;
    uint bootStartTime = 0;

    void Initialize()
    {
        Console.WriteLine("Ubelisk - Nintendo Switch 2 EMU");
        Console.WriteLine("================================");

        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL failed to start: {SDL.SDL_GetError()}");
            return;
        }

        window = SDL.SDL_CreateWindow(
            "Ubelisk - Nintendo Switch 2 EMU",
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            1280, 720,
            SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN
        );

        if (window == IntPtr.Zero)
        {
            Console.WriteLine($"Window creation failed: {SDL.SDL_GetError()}");
            return;
        }

        renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        bootStartTime = SDL.SDL_GetTicks();

        Console.WriteLine("Window created successfully!");
        Console.WriteLine($"PC: 0x{PC:X}");
        Console.WriteLine($"SP: 0x{SP:X}");
        Console.WriteLine("CPU Ready!");
    }

    void HandleEvents()
    {
        SDL.SDL_Event e;
        while (SDL.SDL_PollEvent(out e) != 0)
        {
            if (e.type == SDL.SDL_EventType.SDL_QUIT)
            {
                windowOpen = false;
                running = false;
            }

            // Skip boot screen on any key press
            if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (!bootScreenDone) bootScreenDone = true;
            }
        }
    }

    void RenderBootScreen()
    {
        // Dark background
        SDL.SDL_SetRenderDrawColor(renderer, 10, 10, 15, 255);
        SDL.SDL_Rect bg = new SDL.SDL_Rect { x = 0, y = 0, w = 1280, h = 720 };
        SDL.SDL_RenderFillRect(renderer, ref bg);

        // Left red bar
        SDL.SDL_SetRenderDrawColor(renderer, 230, 0, 18, 255);
        SDL.SDL_Rect leftBar = new SDL.SDL_Rect { x = 200, y = 280, w = 30, h = 160 };
        SDL.SDL_RenderFillRect(renderer, ref leftBar);

        // Right blue bar
        SDL.SDL_SetRenderDrawColor(renderer, 0, 100, 220, 255);
        SDL.SDL_Rect rightBar = new SDL.SDL_Rect { x = 1050, y = 280, w = 30, h = 160 };
        SDL.SDL_RenderFillRect(renderer, ref rightBar);

        // Center white bar (like the Switch dock)
        SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
        SDL.SDL_Rect centerBar = new SDL.SDL_Rect { x = 240, y = 340, w = 800, h = 40 };
        SDL.SDL_RenderFillRect(renderer, ref centerBar);

        // Small red accent top
        SDL.SDL_SetRenderDrawColor(renderer, 230, 0, 18, 255);
        SDL.SDL_Rect topAccent = new SDL.SDL_Rect { x = 540, y = 260, w = 200, h = 10 };
        SDL.SDL_RenderFillRect(renderer, ref topAccent);

        // Small blue accent bottom
        SDL.SDL_SetRenderDrawColor(renderer, 0, 100, 220, 255);
        SDL.SDL_Rect bottomAccent = new SDL.SDL_Rect { x = 540, y = 450, w = 200, h = 10 };
        SDL.SDL_RenderFillRect(renderer, ref bottomAccent);

        SDL.SDL_RenderPresent(renderer);
    }

    void RenderMainScreen()
    {
        // Left side - Nintendo red
        SDL.SDL_SetRenderDrawColor(renderer, 230, 0, 18, 255);
        SDL.SDL_Rect leftSide = new SDL.SDL_Rect { x = 0, y = 0, w = 640, h = 720 };
        SDL.SDL_RenderFillRect(renderer, ref leftSide);

        // Right side - Nintendo blue
        SDL.SDL_SetRenderDrawColor(renderer, 0, 100, 220, 255);
        SDL.SDL_Rect rightSide = new SDL.SDL_Rect { x = 640, y = 0, w = 640, h = 720 };
        SDL.SDL_RenderFillRect(renderer, ref rightSide);

        SDL.SDL_RenderPresent(renderer);
    }

    uint FetchInstruction()
    {
        uint instruction = (uint)(
            memory[PC] |
            (memory[PC + 1] << 8) |
            (memory[PC + 2] << 16) |
            (memory[PC + 3] << 24)
        );
        PC += 4;
        return instruction;
    }

    void ExecuteInstruction(uint instruction)
    {
        uint op = (instruction >> 24) & 0xFF;

        switch (op)
        {
            case 0xD2:
                int reg = (int)(instruction & 0x1F);
                ulong value = (instruction >> 5) & 0xFFFF;
                X[reg] = value;
                break;

            case 0x91:
                int rd = (int)(instruction & 0x1F);
                int rn = (int)((instruction >> 5) & 0x1F);
                ulong imm = (instruction >> 10) & 0xFFF;
                X[rd] = X[rn] + imm;
                break;

            case 0xD4:
                Console.WriteLine("HLT - CPU Halted");
                running = false;
                break;

            default:
                running = false;
                break;
        }
    }

    void Run()
    {
        memory[0x1000] = 0x40;
        memory[0x1001] = 0x05;
        memory[0x1002] = 0x80;
        memory[0x1003] = 0xD2;
        memory[0x1004] = 0x00;
        memory[0x1005] = 0x00;
        memory[0x1006] = 0x00;
        memory[0x1007] = 0xD4;

        running = true;

        while (windowOpen)
        {
            HandleEvents();

            // Show boot screen for 3 seconds
            if (!bootScreenDone)
            {
                uint elapsed = SDL.SDL_GetTicks() - bootStartTime;
                if (elapsed > 3000) bootScreenDone = true;
                RenderBootScreen();
            }
            else
            {
                if (running)
                {
                    uint instruction = FetchInstruction();
                    ExecuteInstruction(instruction);
                }
                RenderMainScreen();
            }

            SDL.SDL_Delay(16);
        }

        SDL.SDL_DestroyRenderer(renderer);
        SDL.SDL_DestroyWindow(window);
        SDL.SDL_Quit();
        Console.WriteLine("Emulator closed.");
    }

    static void Main(string[] args)
    {
        Switch2Emu emu = new Switch2Emu();
        emu.Initialize();
        emu.Run();
    }
}