Spurt = RegisterBehavior("Grow (Spurt)")
Spurt.data = {
    menuEntry = "Size/Grow (Spurt)",
    flags = { "grow" },
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "oneself" }
    }
}


GROW_DURATION = 1
GROW_SPEED = 0.2
IDLE_DURATION = 5

function Spurt:Start()
    self.growStart = Time.time
    self.growing = true
end

function Spurt:Update()
    if self.growing then
        self.agent.scale = self.agent.scale * (1 + GROW_SPEED * Time.deltaTime)

        if Time.time >= self.growStart + GROW_DURATION then
            self.growing = false
            self.growStart = Time.time + IDLE_DURATION
        end
    else

        if Time.time >= self.growStart then
            self.growing = true
        end

    end
end

SpurtSingle = RegisterBehavior("Grow (Single Spurt)")
SpurtSingle.scores = {
    normal = 30,
    curious = 60,
    afraid = -500,
    hostile = 30,
}
SpurtSingle.data = {
    menuEntry = "Size/Grow (Single Spurt)",
    flags = { "ai grow" },
    agent = {
        type = { "giantess", "player" }
    },
    target = {
        type = { "oneself" }
    },
    settings = {
		{ "fRateString", "Grow Speed", "string", "0.06" },
        { "durationString", "Shortest Duration", "string", "1"},
        { "soundClip", "Use Sound Clip (filename):", "string", "none"},
        { "loopClip", "Loop Sound", "bool", true},
        { "spurtAnim", "Play Animation", "bool", true}
	}
}

function SpurtSingle:updateSFX()
    self.mPitch = ((self.agent.scale/5)/(0.04/self.agent.scale)*self.agent.scale/(self.agent.scale*self.agent.scale*math.sqrt(self.agent.scale))
	/(math.sqrt(math.sqrt(math.sqrt(math.sqrt(self.agent.scale)))) * (math.sqrt(math.sqrt(math.sqrt(self.agent.scale))))))
	self.agent.dict.sSpurtAudio.pitch = (self.agent.scale * (1.05 / (self.agent.scale / 0.2 / self.mPitch)) * (1 / math.sqrt((self.agent.scale * 1000 / 125) + 1))) + 0.65
	self.vol = (1.7 - self.agent.dict.sSpurtAudio.pitch) * (Mathf.Clamp(self.fRate , 0, 1.5) / 1.875 + 0.2)
	self.agent.dict.sSpurtAudio.minDistance = (1000 * self.agent.scale / self.pScale / (300 + self.agent.scale / self.pScale)) / 2 * 0.0025
end

function SpurtSingle:Start()
    self.fRate = tonumber(self.fRateString)
    self.duration = tonumber(self.durationString)
    self.mRate = 0
    self.cRate = 0
    self.growStart = Time.time
    self.growing = true
    self.pScale = self.agent.scale
    math.randomseed(os.time())
    self.durationMult = math.random(1, (2 * math.random(1,2)))
    if self.soundClip ~= "none" then
        self.mVol = 0
	    self.cVol = 0
	    self.tVol = 0
        self.agent.dict.sSpurtAudio = AudioSource:new(self.agent.bones.spine)
        self.agent.dict.sSpurtAudio.spatialBlend = 1
        self.agent.dict.sSpurtAudio.clip = self.soundClip
        self:updateSFX()
    	self.agent.dict.sSpurtAudio.loop = self.loopClip		
	    self.agent.dict.sSpurtAudio.volume = 0
    end
     if self.spurtAnim then
        self.agent.ai.StopAction()
        self.OGtransitionD = self.agent.animation.transitionDuration
        self.OGanimSpeed = self.agent.animation.speedMultiplier
        self.agent.animation.SetSpeed(0.4)
        self.agent.animation.transitionDuration = 0.4
        self.canLook = self.agent.CanLookAtPlayer
        self.agent.CanLookAtPlayer = false
        self.agent.animation.Set("Defeat")
        self.agent.Wait(self.durationMult * self.duration * 0.8)
    end
    if not self.agent.ai.IsAIEnabled() then
        log("Growing a bit during "..self.durationMult * self.duration.." seconds.")
    end
end

function SpurtSingle:Update()    
    if self.fRate ~= tonumber(self.fRateString) then self.fRate = tonumber(self.fRateString) end
    if self.duration ~= tonumber(self.durationString) then self.duration = tonumber(self.durationString) end
    if self.growing then
        if self.cRate <= self.fRate then
            self.mRate = self.mRate + self.fRate / 465 * (Time.deltaTime / 0.0333) / (0.0333 / Time.deltaTime)
            self.cRate = self.cRate + self.mRate
        end

        self.agent.scale = self.agent.scale * (1 + self.cRate * Time.deltaTime)

        if Time.time >= self.growStart + self.duration * self.durationMult then
            self.growing = false
        end

        --IF USING A SOUND CLIP
        if self.soundClip ~= "none" then
            if not self.playing then -- Prevents starting the clip continuously as this is in an Update loop
                self.playing = true  -- ''
                self.agent.dict.sSpurtAudio:Play() -- Start the audio clip
            end

            --adjust max volume with scale
            self:updateSFX()

            --fade-in
            if self.loopClip then
                if self.agent.dict.sSpurtAudio.volume < self.vol then
                    self.tVol = (self.duration / 4) / Time.deltaTime
                    self.mVol = self.mVol + 1
                    self.cVol = self.cVol + self.mVol
                    self.agent.dict.sSpurtAudio.volume = self.vol * self.cVol / (self.tVol * (self.tVol + 1) / 2)
                    --log("Volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                else
                    --log("Final volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                end
            else
                self.agent.dict.sSpurtAudio.volume = self.vol
            end
        end
    else
        if self.mRate > 0 and self.cRate > 0 then
            self.cRate = self.cRate - self.mRate
            self.mRate = self.mRate - self.fRate / 465 * (Time.deltaTime / 0.0333) / (0.0333 / Time.deltaTime)
        else
            self.cRate = 0
            self.mRate = 0
            if not self.playing then self.agent.ai.StopSecondaryBehavior("ai grow") end
        end

        self.agent.scale = self.agent.scale * (1 + self.cRate * Time.deltaTime)

        if self.soundClip ~= "none" then
            --adjust max volume with scale
            self:updateSFX()

            --fade-out
            if self.loopClip then
                if self.agent.dict.sSpurtAudio.volume > 0 then
                    self.tVol = (self.duration / 4) / Time.deltaTime
                    self.cVol = self.cVol - self.mVol
                    self.mVol = self.mVol - 1
                    self.agent.dict.sSpurtAudio.volume = self.vol * self.cVol / (self.tVol * (self.tVol + 1) / 2)
                    --log("Volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                else
                    --log("Final volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                    self.agent.dict.sSpurtAudio:Pause()
                    self.playing = false
                end
            else
                self.playing = false
            end
        end
    end
end

function SpurtSingle:Exit()
    if self.spurtAnim then
        self.agent.animation.transitionDuration = self.OGtransitionD
        self.agent.animation.SetSpeed(self.OGanimSpeed)
        self.agent.CanLookAtPlayer = self.canLook
    end
    if not self.agent.ai.IsAIEnabled() then
        self.agent.ai.SetBehavior("Idle")
    end
end

mSpurtSingle = RegisterBehavior("Grow micro (S. Spurt)")
mSpurtSingle.data = {
    menuEntry = "Size/Grow micro (S. Spurt)",
    flags = { "grow" },
    agent = {
        type = { "micro" }
    },
    target = {
        type = { "oneself" },
    },
    settings = {
		{ "fRateString", "Grow Speed", "string", "0.06" },
        { "durationString", "Shortest Duration", "string", "1"},
        { "soundClip", "Use Sound Clip (filename):", "string", "none"},
        { "loopClip", "Loop Sound", "bool", true},
        { "spurtAnim", "Play Animation", "bool", true}
	}
}

function mSpurtSingle:updateSFX()
    self.mPitch = ((self.agent.scale/5)/(0.04/self.agent.scale)*self.agent.scale/(self.agent.scale*self.agent.scale*math.sqrt(self.agent.scale))
	/(math.sqrt(math.sqrt(math.sqrt(math.sqrt(self.agent.scale)))) * (math.sqrt(math.sqrt(math.sqrt(self.agent.scale))))))
	self.agent.dict.sSpurtAudio.pitch = (self.agent.scale * (1.05 / (self.agent.scale / 0.2 / self.mPitch)) * (1 / math.sqrt((self.agent.scale * 1000 / 125) + 1))) + 0.65
	self.vol = (1.7 - self.agent.dict.sSpurtAudio.pitch) * (Mathf.Clamp(self.fRate , 0, 1.5) / 1.875 + 0.2)
	self.agent.dict.sSpurtAudio.minDistance = (1000 * self.agent.scale / self.pScale / (300 + self.agent.scale / self.pScale)) / 2 * 0.0025
end

function mSpurtSingle:Start()
    self.fRate = tonumber(self.fRateString)
    self.duration = tonumber(self.durationString)
    self.mRate = 0
    self.cRate = 0
    self.growStart = Time.time
    self.growing = true
    math.randomseed(os.time())
    self.durationMult = math.random(1, (2 * math.random(1,2)))
    if self.soundClip ~= "none" then
        self.mVol = 0
	    self.cVol = 0
	    self.tVol = 0
        self.agent.dict.sSpurtAudio = AudioSource:new(self.agent.bones.spine)
        self.agent.dict.sSpurtAudio.spatialBlend = 1
        self.agent.dict.sSpurtAudio.clip = self.soundClip
        self:updateSFX()
    	self.agent.dict.sSpurtAudio.loop = self.loopClip		
	    self.agent.dict.sSpurtAudio.volume = 0
    end
    if self.spurtAnim then
        self.agent.ai.StopAction()
        self.OGtransitionD = self.agent.animation.transitionDuration
        self.OGanimSpeed = self.agent.animation.speedMultiplier
        self.agent.animation.SetSpeed(0.4)
        self.agent.animation.transitionDuration = 0.4
        self.agent.animation.Set("Defeat")
        self.agent.Wait(self.durationMult * self.duration * 0.8)
    end
    if not self.agent.ai.IsAIEnabled() then
        log("Growing a bit over "..self.durationMult * self.duration.." seconds.")
    end
end

function mSpurtSingle:Update()    
    if self.fRate ~= tonumber(self.fRateString) then self.fRate = tonumber(self.fRateString) end
    if self.duration ~= tonumber(self.durationString) then self.duration = tonumber(self.durationString) end
    if self.growing then
        if self.cRate <= self.fRate then
            self.mRate = self.mRate + self.fRate / 465 * (Time.deltaTime / 0.0333) / (0.0333 / Time.deltaTime)
            self.cRate = self.cRate + self.mRate
        end

        self.agent.scale = self.agent.scale * (1 + self.cRate * Time.deltaTime)

        if Time.time >= self.growStart + self.duration * self.durationMult then
            self.growing = false
        end

        --IF USING A SOUND CLIP
        if self.soundClip ~= "none" then
            if not self.playing then -- Prevents starting the clip continuously as this is in an Update loop
                self.playing = true  -- ''
                self.agent.dict.sSpurtAudio:Play() -- Start the audio clip
            end

            --adjust max volume with scale
            self:updateSFX()

            --fade-in
            if self.loopClip then
                if self.agent.dict.sSpurtAudio.volume < self.vol then
                    self.tVol = (self.duration / 4) / Time.deltaTime
                    self.mVol = self.mVol + 1
                    self.cVol = self.cVol + self.mVol
                    self.agent.dict.sSpurtAudio.volume = self.vol * self.cVol / (self.tVol * (self.tVol + 1) / 2)
                    --log("Volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                else
                    --log("Final volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                end
            else
                self.agent.dict.sSpurtAudio.volume = self.vol
            end
        end
    else
        if self.mRate > 0 and self.cRate > 0 then
            self.cRate = self.cRate - self.mRate
            self.mRate = self.mRate - self.fRate / 465 * (Time.deltaTime / 0.0333) / (0.0333 / Time.deltaTime)
        else
            self.cRate = 0
            self.mRate = 0
            if not self.playing then self.agent.ai.StopSecondaryBehavior("grow") end
        end
        
        self.agent.scale = self.agent.scale * (1 + self.cRate * Time.deltaTime)
        
        if self.soundClip ~= "none" then
            --adjust max volume with scale
            self:updateSFX()

            --fade-out
            if self.loopClip then
                if self.agent.dict.sSpurtAudio.volume > 0 then
                    self.tVol = (self.duration / 4) / Time.deltaTime
                    self.cVol = self.cVol - self.mVol
                    self.mVol = self.mVol - 1
                    self.agent.dict.sSpurtAudio.volume = self.vol * self.cVol / (self.tVol * (self.tVol + 1) / 2)
                    --log("Volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                else
                    --log("Final volume: "..self.agent.dict.sSpurtAudio.volume.." / "..self.vol)
                    self.agent.dict.sSpurtAudio:Pause()
                    self.playing = false
                end
            else
                self.playing = false
            end
        end
    end
end

function mSpurtSingle:Exit()
    if self.spurtAnim then
        self.agent.animation.transitionDuration = self.OGtransitionD
        self.agent.animation.SetSpeed(self.OGanimSpeed)
    end
    if self.spurtAnim and not self.agent.ai.IsAIEnabled() then
        self.agent.ai.SetBehavior("Idle")
    end
end
