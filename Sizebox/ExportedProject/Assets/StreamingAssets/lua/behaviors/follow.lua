Follow = RegisterBehavior("xFollow")
Follow.data = {
    menuEntry = "Walk/Follow",
    description = "Follow a target.",
    tags = "movement, macro, micro",
    agent = {
        type = { "humanoid" } 
    },
    target = {
        type = { "humanoid" }
    }
}

walkAnimation = "Walk"
idleAnimation = "Idle"

function Follow:Start()
    local targetSeparation = -0.15 + Mathf.clamp(self.target.scale / self.agent.scale, 0.25, 0.75)
    self.agent.LookAt(self.target)
    self.agent.animation.Set(walkAnimation)
    self.agent.seek(self.target, 0, targetSeparation)
    self.waited = false
end

function Follow:Update()
    if not self.agent.ai.IsActionActive() then
        if not self.target or not self.target.IsTargettable() then
            self.agent.ai.StopBehavior()
            return
        end

        if self.waited then
            self:Start()
        else
            self.agent.animation.SetAndWait(idleAnimation)
            self.waited = true
        end
    end
end

function Follow:Exit()
    self.agent.animation.Set(idleAnimation)
end