Behavior = RegisterBehavior("Giggle")
Behavior.scores = {
    normal = 30,
    curious = 60,
    afraid = -500,
    hostile = 30,
}
Behavior.data = {
    secondary = true,
    menuEntry = "Sound/Giggle",
    flags = { "giggle" },
    agent = {
        type = { "giantess", "player" }
    },
    target = {
        type = { "oneself" }
    },
    settings = {
        {"soundFile", "Sound Clip", "string", "giggle.wav"}
    }
}

function Behavior:Start()
    if not self.agent.dict.audio_source then -- I'm using dict to circumvent how the AI starts behaviors over and over, removing ties with self. values
        self.agent.dict.audio_source = AudioSource:new(self.agent.bones.head) -- to create an audiosource, you have to pass a transform or entity as argument
                                                                              -- current restriction is one audiosource per bone. If the bone already has one 
                                                                              -- AudioSource, that is the one that will return.
        self.agent.dict.audio_source.clip = self.soundFile -- WAV and OGG files supported, must be found in the /Sounds folder
        self.agent.dict.audio_source.loop = false        -- Set this to true to set whether a clip will replay when it ends
        self.agent.dict.audio_source.spatialBlend = 1    -- 0 makes it 2D (for example for background music), and 1 makes it 3d (the source of the sounds is in the game space)
        self.agent.dict.audio_source.maxDistance = 3     -- 2m from the character point of view
        self.agent.dict.audio_source.pitch = 0.95        -- adjust the pitch.. 1 is normal    
        
        self.agent.dict.audio_source:Play()     -- Load the sound (by playing it) but stop it immediately
        self.agent.dict.audio_source:Stop()     -- This avoids the little sound glitch where it's loud and sounds jaggy for a few frames
        self.agent.dict.t = Time.time + 0.1     -- Tiny timer to allow for the loading to happen before playing the sound for real in Update()
    end
    if not self.agent.dict.audio_source.isPlaying or not self.agent.ai.IsAIEnabled() then -- If no AI, play from start right away even if already playing.
        self.playSound = true                   -- Used for bypassing the issue with AI where it can load behaviors 2 or more times in a row,
    end                                         -- playing the file from start without letting it finish. (AI tries to call new behaviors once every frame)
end 

function Behavior:Update()
    if self.playSound then -- Lets the clip play once, then it checks if it ended to stop the behavior
        if Time.time > self.agent.dict.t then -- After one 10th of a second, allow the clip to play for real
            self.agent.dict.audio_source:Play()
            self.playSound = false  -- Prevent looping this block
        end
    else
        if not self.agent.dict.audio_source.isPlaying then -- Stop the behavior to clean it from memory once it's not used
            self.agent.ai.StopSecondaryBehavior("giggle")  -- Stops any behaviors with flags matching "giggle". See line 10 ^
        end
    end
end

function Behavior:Exit() -- This will trigger everytime the script is opened when it's already running, 
    if not self.agent.dict.audio_source.isPlaying and not self.playSound then -- Delete audio source once the clip has played.
        self.agent.dict.audio_source = nil
    end
end

Behavior2 = RegisterBehavior("Giggle OnDemand")
Behavior2.data = {
    secondary = true,
    menuEntry = "Sound/Giggle OnDemand",
    flags = { "giggleKey" },
    agent = {
        type = { "giantess", "player" }
    },
    target = {
        type = { "oneself" }
    },
    settings = {
        {"soundFile", "Sound Clip", "string", "giggle.wav"},
        {"gKey", "Keybind", "keybind", "l"}
    }
}

function Behavior2:Start()
    self.audio_source = AudioSource:new(self.agent.bones.head) -- to create an audiosource, you have to pass a transform or entity as argument
                                                                            -- current restriction is one audiosource per bone. If the bone already has one 
                                                                            -- AudioSource, that is the one that will return.
    self.audio_source.clip = self.soundFile -- WAV and OGG files supported, must be found in the /Sounds folder
    self.audio_source.loop = false        -- Set this to true to set whether a clip will replay when it ends
    self.audio_source.spatialBlend = 1    -- 0 makes it 2D (for example for background music), and 1 makes it 3d (the source of the sounds is in the game space)
    self.audio_source.maxDistance = 4     -- 2m from the character point of view
    self.audio_source.pitch = 0.95        -- adjust the pitch.. 1 is normal    
    self.audio_source:Play()     -- Load the sound (by playing it) but stop it immediately
    self.audio_source:Stop()     -- This avoids the little sound glitch where it's loud and sounds jaggy for a few frames
    
    self.playSound = false
    self.audioScaleMod = false

    log("Giggle on Demand activated.")
    log("Press ["..string.upper(self.gKey).."] or [Middle Mouse Click] to play the sound!")
    log("Press [Ctrl]+["..string.upper(self.gKey).."] to enable audio pitch and volume related to scale.")
    log("Press [Ctrl]+[Shift]+["..string.upper(self.gKey).."] to stop this behavior.")
end

function Behavior2:Update()
    if self.audio_source.isPlaying then
		-- Audio Modulation, changes pitch of sound according to GTS scale
		if self.audioScaleMod then
			self.audio_source.pitch = ((0.8 * (1 / math.sqrt((self.agent.scale * 1000 / 155) + 1))) + 0.7)	-- Adjusts the pitch in relation to the size of the giantess
			self.audio_source.volume = (1.9 - self.audio_source.pitch)
		else
			self.audio_source.pitch = 0.95
			self.audio_source.volume = 0.7
		end
	else
		self.playSound = false
	end

    --==| KEYBINDS |==--
    if not Input.GetKey("left ctrl") and (Input.GetKeyDown(self.gKey) or Input.GetMouseButtonDown(2)) then -- Play sound
        self.playSound = not self.playSound
        if self.playSound then
            self.audio_source:Play()
        else
            self.audio_source:Stop()
        end
    elseif not Input.GetKey("left shift") and Input.GetKey("left ctrl") and Input.GetKeyDown(self.gKey) then -- Enable Audio Modulation
        self.audioScaleMod = not self.audioScaleMod
        if self.audioScaleMod then
            log("Enabled audio modulation related to GTS size.")
        else
            log("Disabled audio modulation related to GTS size.")
        end
    elseif Input.GetKey("left ctrl") and Input.GetKey("left shift") and Input.GetKeyDown(self.gKey) then -- Stop behavior
        self.agent.ai.StopSecondaryBehavior("giggleKey")
    end 
end

function Behavior2:Exit()
	self.audio_source:Stop()
end