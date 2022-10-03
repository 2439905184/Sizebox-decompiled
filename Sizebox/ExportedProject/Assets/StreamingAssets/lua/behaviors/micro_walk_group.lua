GroupWalk = RegisterBehavior("Walk Here (Group)")
GroupWalk.data = {
    menuEntry = "Walk/Walk Here (in Group)",
    agent = {
        type = { "micro" } 
    },
    target = {
        type = { "none" }
    }
}

walkAnimation = "Walk"
followBehavior = "xFollow"
idleAnimation = "Idle"

function GroupWalk:Start()
    -- Typical walk behavior
    self.agent.animation.Set(walkAnimation)
    self.agent.moveTo(self.cursorPoint)
    self.agent.animation.Set(idleAnimation)
    -- Find micros nearby to form a group
    self.microGroup = self.agent.senses.GetMicrosInRadius(50)

    -- Iterate throught the micro list, and assign the Follow behavior
    for i = 1, #self.microGroup do
        local micro = self.microGroup[i]
        if not micro.isPlayer() then 
            micro.ai.SetBehavior(followBehavior, self.agent)
        end
    end
end

function GroupWalk:Update()
    -- Wait until the movement has finished
    if not self.agent.ai.IsActionActive() then
        -- First stop the group behavior
        for i = 1, #self.microGroup do
            local micro = self.microGroup[i]
            -- We do some additional checks for micros that died before reaching the target
            if micro and not micro.isDead() and micro.ai then 
                micro.ai.StopBehavior()
            end
        end
        -- Then stop the leader behavior
        self.agent.ai.StopBehavior()
    end
end

function GroupWalk:End()
    self.agent.animation.Set(idleAnimation)    
end


