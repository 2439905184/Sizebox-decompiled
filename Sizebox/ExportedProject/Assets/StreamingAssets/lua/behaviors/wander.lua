Wander = RegisterBehavior("Wander")
Wander.scores = {
    normal = 40
}
Wander.data = {
    menuEntry = "Walk/Wander",
    agent = {
        type = { "humanoid" }
    },
    target = {
        type = { "none" }
    },
	tags = "movement",
    settings = {
        {"wanderMinDuration", "Min Duration (seconds)", "string", "5"},
        {"wanderMaxDuration", "Max Duration (seconds)", "string", "15"}
    }
}

animation = "Walk" -- name of the walk animation
idleAnimation = "Idle"

function Wander:Start()
    --[[self.stop = false -- i added a stop variable to end the behavior.. this is custom for this script
    if self.agent.ai.IsAIEnabled() and ((self.agent.isGiantess() and not self.activateOnGTS) or (self.agent.isMicro() and not self.activateOnMicros)) then --If not enabled on GTS or Micro and entity fits, skip behavior.
        self.stop = true
    end]]
    math.randomseed(os.time())
end

function Wander:Update()
    if not self.agent.ai.IsActionActive() then
        if self.stop then
            self.agent.ai.StopBehavior() -- if you use Update() you must manually tell when to end the behavior, after this the Exit() method will run
        else
            self.agent.animation.Set(animation) -- set the walk animation
            local timer = math.random(tonumber(self.wanderMinDuration), tonumber(self.wanderMaxDuration)) -- choose a random number between minTime and maxTime
            self.agent.wander(timer)

            -- if the giantess has the AI mode active or another behavior queued, 
            -- then i want her to stop and do another thing
            -- it will stop in the next loop, so i can make sure it runs at least once
            self.stop = self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() 
        end
    end
end

function Wander:Exit()
    if not self.agent.ai.IsAIEnabled() then
        self.agent.animation.Set(idleAnimation)
    end
end
