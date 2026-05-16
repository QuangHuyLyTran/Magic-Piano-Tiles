Magic Tiles - Technical Assessment & Handover Report

Magic Tiles is a high-quality rhythm game prototype for mobile, built in Unity. The focus was on delivering a polished gameplay experience, smooth visuals, and optimized performance for mobile devices.

Tech Stack & Tools
- Engine: Unity (supports PC and mobile)
- UI & Text: TextMeshPro (uGUI)
- Animation & Effects: DOTween (Demigiant)
- Audio: Unity AudioEngine set up for low latency
- Optimization: Unity Profiler & Frame Debugger

Project Overview & Criteria

1. Core Gameplay & Polish
The gameplay feels tight and responsive. Tile generation runs through the LaneManager that keeps everything synced with the music. Timing is key—players get ‘Perfect,’ ‘Great,’ or ‘Cool’ ratings depending on how close they hit the note. Perfect hits bring 100 points, screen zoom, punch scale effects, and extra visual juice. Great offers 80 points with nice audio feedback, while Cool is a quieter 50-point score.

Effects and juice really ramp up the excitement. There’s a custom screen shake for misses and a bigger shake for game over. Menu transitions look smooth, using staggered fade-ins and slide-ups thanks to CanvasGroup and DOTween. When you tap a tile, it vanishes right away—object pooling keeps performance slick, and particle bursts make it feel snappy.

2. UI and VFX Optimization
There’s full technical documentation in OPTIMIZATION.md, but here’s the core: Draw calls dropped by using a single sprite atlas for all particles, then tying them to a unified material. Particle intensity gets handled programmatically, not with extra duplicated levels. Raycast targets are switched off for anything non-interactive, cutting unnecessary physics checks. UI is split into Static and Dynamic canvases, so Canvas rebuilds don’t drag everything down.

3. Code Quality & Performance
A centralized GameManager oversees everything—state transitions, gameplay, pause, and end states are kept decoupled. Memory-wise, the game stays zero-allocation during action, relying heavily on object pooling instead of frequent Instantiate or Destroy operations. Sound settings eliminate lag: SFX are set for ‘Decompress on Load’ and PCM compression, and DSP Buffer Size’s locked for best latency.

4. Bonus Features & Extra Juice
A miss instantly pauses the background music and shakes the screen to drive home the mistake. Recovery just picks up where it left off, keeping everything in sync. The audio paths for BGM and SFX are fully separated, and hit SFX uses PlayOneShot for seamless overlap—so tapping fast means every hit sounds crisp, not chopped. Combo counters? They get punch-scale animations that grow with your streak, making every milestone feel special.

How to Test & Run  
1. Open your project in Unity Editor.
2. Make sure DOTween is ready to go — just head to “Tools” and open the DOTween Utility Panel.
3. Find the `MainScene` in the Scenes folder and load it up.
4. Switch to Game View. For the best layout, pick either 16:9 or a Portrait Mobile resolution.
5. Check out the Stats Panel or hit Profiler (Ctrl + 7) to watch for efficient draw call batching, especially when explosions happen.
6. Hit Play:  
   - Click “Play” on the animated menu.  
   - You can press ESC at any point during gameplay to open up the custom Pause Menu — this freezes TimeScale and pauses the background music.
   - Miss three times to see the Game Over Screen. Or, finish the song and you'll move on to the Next Stage Panel.

AI Collaboration Statement
We used Gemini as an active tech consultant and code reviewer throughout development.  
- Architecture Review: The AI helped double-check state transitions in `GameManager.cs`, especially making sure `Time.timeScale` handled Pause, Game Over, and Main Menu shifts without any hiccups.  
- Optimization Diagnostics: AI was key for spotting performance issues with texture assets. This led us to swap out raw material cloning for a Texture Sheet Animation fix to keep things fast and tidy.  
Overall, teaming up with AI let us ship clean, pro-level code quickly and safely.