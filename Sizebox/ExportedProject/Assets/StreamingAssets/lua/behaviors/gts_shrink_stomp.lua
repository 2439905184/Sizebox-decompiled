ShrinkStomp = RegisterBehavior("Shrink & Stomp")
ShrinkStomp.scores = {
    hostile = 100,
}
ShrinkStomp.data = {
    agent = {
        type = { "giantess" }
    },
    target = {
        type = { "micro" }
    },
    settings = {
        {"limitDistance", "AI: Only target nearby micros", "bool", true},
        {"mercifulMode", "Merciful Mode", "bool", true} --should look for a new target after failed stomp attempt?
    }
}

shrinkRatio = 0.07 -- micro to gts target ratio size
idleAnimation = "Idle 2"
exitIdleAnimation = "Idle 4"
walkAnimation = "Walk"
WAITING, WALKING, STOMPING = 0, 1, 2

stompAnimModule = require "stomp_anim"

function ShrinkStomp:Start()
    --If AI, prep for stuck check
    if self.agent.ai.IsAIEnabled() then
        if self.limitDistance and self.agent.DistanceTo(self.target) > self.agent.scale * 4 then --If target chosen by AI is too far, choose closest one
            self.target = self.agent.findClosestMicro()
        end
        
        --It's pointless to continue without a target to stomp, so let's make sure we have one
        if not self.target then self.targetGone = true self.agent.ai.StopBehavior() return end
        
        self.lastScale = self.agent.scale
        self.lastDistance = self.agent.DistanceTo(self.target)
        self.timer = Time.time
        self.timerScaleAdd = Mathf.Clamp(6 + self.agent.scale / 20, 4, 10)
    end
    self.state = WAITING
    self.waitTime = 0
end

function ShrinkStomp:Update()

    if self.targetGone then return end --If no target in Start(), don't run Update()

    if not self.target or self.target.isDead() or not self.target.IsTargettable() then --Check for dead/missing target, except when Stomping
        if self.state ~= STOMPING or (self.state == STOMPING and self.stomped) then
            if self.agent.ai.IsAIEnabled() or self.agent.ai.HasQueuedBehaviors() then --If the giantess has the AI mode active or another behavior queued, then I want her to stop and do another thing
                self.targetGone = true
                self.agent.ai.StopBehavior()
                return
            else
                if self.target and self.target.isDead() then self.target = nil self.agent.ai.StopAction() end
                self.state = WAITING
            end
        end
    end

    if self.state == WAITING and self.waitTime < Time.time then
        --log("waiting")
        if not self.target or not self.target.IsTargettable() then --No targets: Wait
            self.target = self.agent.findClosestMicro()
            self.agent.animation.Set(idleAnimation)
            self.state = WAITING
            if self.target then self.waitTime = Time.time else self.waitTime = Time.time + 1 end
            return
        else
            self.agent.LookAt(self.target)
            self.agent.animation.Set(walkAnimation) -- found target, go get em
            self.state = WALKING
            self.timer = Time.time
            if self.stomped and not self.target.isDead() then self.stomped = false end
        end
    end

    if self.state == WALKING then
        --if not logwalk then logwalk = true log("walking") logwait = false logstomp = false end
        local separation = (self.agent.scale + self.target.scale) * 0.2 -- how far are we
        local targetDirection = self.target.position - self.agent.position
        local sizeRatio = self.agent.scale * shrinkRatio / self.target.scale -- how much to shrink them to achieve target size ratio

        -- Log("dist " .. targetDirection.magnitude .. " separation " .. separation)

        -- are they in shrink range
        if sizeRatio < 0.95 and targetDirection.magnitude < separation * 2.4 then
            self.agent.ai.StopAction()
            self.agent.animation.Set(idleAnimation)
            self.target.Grow(sizeRatio - 1, 2.3)
            self.state = WAITING
            self.waitTime = Time.time + 2.31
        -- are they in stomp range
        elseif sizeRatio >= 0.95 and targetDirection.magnitude < separation * 1.4 then
            self.agent.ai.StopAction()
            self.agent.animation.Set(stompAnimModule.getRandomStompAnim())
            self.agent.Stomp(self.target)
            self.state = STOMPING
        elseif not self.agent.ai.IsActionActive() then
            self.agent.MoveTo(self.target)
        else
            --log("stuckcheck")
            --If using AI, check if entity gets stuck while walking; if it gets stuck, stop behavior.
            if not self.target.isDead() and self.lastScale and Time.time > self.timer + self.timerScaleAdd then --self.lastScale = if AI
                if self.target.animation.Get() == "Run" or self.target.animation.Get() == "Running" then
                    self.distanceCheck = self.agent.DistanceTo(self.lastPosition) < self.agent.scale * 0.5
                else
                    self.distanceCheck = self.lastDistance - self.agent.DistanceTo(self.target) < self.agent.scale * 0.5
                end

                if self.distanceCheck then
                    --log("stuck")
                    self.agent.ai.StopBehavior()
                    return
                else
                    --log("not stuck!")
                    if self.lastScale and self.agent.scale ~= self.lastScale then
                        self.lastScale = self.agent.scale
                    end
                    self.lastPosition = self.agent.transform.position
                    self.lastDistance = self.agent.DistanceTo(self.target)
                    self.timer = Time.time
                end
            end
        end
    end

    if self.state == STOMPING then
        --if not logstomp then logstomp = true log("stomping") logwait = false logwalk = false end
        if not self.agent.ai.IsActionActive() then
            self.stomped = true
            self.state = WAITING
            self.waitTime = Time.time + 2
            if self.mercifulMode then
                self.target = nil
            end
        end
    end
end


function ShrinkStomp:Exit()
    if (not self.agent.ai.IsAIEnabled() or self.targetGone) and self.agent.animation.Get() ~= exitIdleAnimation then 
        self.agent.animation.Set(exitIdleAnimation)
    end
end
